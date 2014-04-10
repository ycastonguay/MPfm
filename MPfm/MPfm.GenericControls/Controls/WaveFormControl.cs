// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using MPfm.Core;
using MPfm.GenericControls.Interaction;
using MPfm.GenericControls.Services;
using MPfm.GenericControls.Services.Events;
using MPfm.GenericControls.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.PeakFiles;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Graphics;
using MPfm.GenericControls.Wrappers;
using TinyIoC;

namespace MPfm.GenericControls.Controls
{
    /// <summary>
    /// The WaveFormScale control displays the scale in minutes/seconds on top of the wave form.
    /// </summary>
    public class WaveFormControl : IControl, IControlMouseInteraction
    {
        private readonly object _locker = new object();
        private IWaveFormCacheService _waveFormCacheService;
        private bool _isMouseDown;
        private float _density;
        private BasicPen _penTransparent;
        private BasicPen _penCursorLine;
        private BasicPen _penSecondaryCursorLine;
        private BasicPen _penMarkerLine;
        private BasicPen _penSelectedMarkerLine;
        private BasicBrush _brushMarkerBackground;
        private BasicBrush _brushSelectedMarkerBackground;
        private Guid _activeMarkerId = Guid.Empty;
        private List<Marker> _markers = new List<Marker>();
        private string _status = "";
        private float _cursorX;
        private float _secondaryCursorX;
		private BasicColor _backgroundColor = new BasicColor(32, 40, 46);        
        private BasicColor _cursorColor = new BasicColor(0, 128, 255);
        private BasicColor _secondaryCursorColor = new BasicColor(255, 255, 255);
        private BasicColor _markerCursorColor = new BasicColor(255, 0, 0);
        private BasicColor _markerSelectedCursorColor = new BasicColor(234, 138, 128);
        private BasicColor _textColor = new BasicColor(255, 255, 255);

        public bool IsLoading { get; private set; }
        public InputInteractionMode InteractionMode { get; set; }
        public BasicRectangle Frame { get; set; }
		public float FontSize { get; set; }
		public string FontFace { get; set; }
		public float LetterFontSize { get; set; }
		public string LetterFontFace { get; set; }

        private long _position;
        public long Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;

                // Don't bother if a peak file is loading
                if (IsLoading)// || _imageCache == null)
                    return;

                // Invalidate cursor
                var rectCursor = new BasicRectangle(_cursorX - 5, 0, 10, Frame.Height);
                OnInvalidateVisualInRect(rectCursor);
            }
        }

        private long _secondaryPosition;
        public long SecondaryPosition
        {
            get
            {
                return _secondaryPosition;
            }
            set
            {
                _secondaryPosition = value;

                // Don't bother if a peak file is loading
                if (IsLoading)
                    return;

                // Invalidate cursor. TODO: When the cursor is moving quickly, it dispappears because of the invalidation.
                //                          Maybe the cursor shouldn't be rendered, but instead be a simple rect over this control?
                var rectCursor = new BasicRectangle(_secondaryCursorX - 25, 0, 50, Frame.Height);
                OnInvalidateVisualInRect(rectCursor);
            }
        }

        private float _zoom = 1.0f;
        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
                // Adjust content offset
                //RefreshWaveFormBitmap();
                OnInvalidateVisual();
            }
        }
        
        private BasicRectangle ContentSize
        {
            get
            {
                //return new BasicRectangle(0, 0, Frame.Width * Zoom, Frame.Height * Zoom);
                return new BasicRectangle(0, 0, Frame.Width * Zoom, Frame.Height);
            }
        }
        
        private BasicPoint _contentOffset = new BasicPoint(0, 0);
        public BasicPoint ContentOffset
        {
            get
            {
                return _contentOffset;
            }
            set
            {
                _contentOffset = value;
                //Console.WriteLine("WaveFormControl - ContentOffset: {0}", _contentOffset);
                OnInvalidateVisual();
            }
        }

        public WaveFormDisplayType DisplayType { get; set; }
        public AudioFile AudioFile { get; private set; }
        public bool ShowSecondaryPosition { get; set; }
        public long Length { get; set; }

        public delegate void ChangePosition(float position);
        public event InvalidateVisual OnInvalidateVisual;
        public event InvalidateVisualInRect OnInvalidateVisualInRect;
        public event ChangePosition OnChangePosition;
        public event ChangePosition OnChangeSecondaryPosition;

        public WaveFormControl()
        {
            Initialize();
        }

        private void Initialize()
        {
            OnInvalidateVisual += () => { };
            OnInvalidateVisualInRect += (rect) => { };
            OnChangePosition += (position) => { };
			FontSize = 12;
			FontFace = "Roboto Light";
			LetterFontSize = 10;
			LetterFontFace = "Roboto";
            Frame = new BasicRectangle();
            DisplayType = WaveFormDisplayType.Stereo;

            _waveFormCacheService = Bootstrapper.GetContainer().Resolve<IWaveFormCacheService>();
            _waveFormCacheService.GeneratePeakFileBegunEvent += HandleGeneratePeakFileBegunEvent;
            _waveFormCacheService.GeneratePeakFileProgressEvent += HandleGeneratePeakFileProgressEvent;
            _waveFormCacheService.GeneratePeakFileEndedEvent += HandleGeneratePeakFileEndedEvent;
            _waveFormCacheService.LoadedPeakFileSuccessfullyEvent += HandleLoadedPeakFileSuccessfullyEvent;
            _waveFormCacheService.GenerateWaveFormBitmapEndedEvent += HandleGenerateWaveFormEndedEvent;
        }

        private void HandleGeneratePeakFileBegunEvent(object sender, GeneratePeakFileEventArgs e)
        {
			//Console.WriteLine("WaveFormControl - HandleGeneratePeakFileBegunEvent");
            RefreshStatus("Generating peak file (0% done)");
        }

        private void HandleGeneratePeakFileProgressEvent(object sender, GeneratePeakFileEventArgs e)
        {
			//Console.WriteLine("WaveFormControl - HandleGeneratePeakFileProgressEvent  (" + e.PercentageDone.ToString("0") + "% done)");
            RefreshStatus("Generating peak file (" + e.PercentageDone.ToString("0") + "% done)");
        }

        private void HandleGeneratePeakFileEndedEvent(object sender, GeneratePeakFileEventArgs e)
        {
			//Console.WriteLine("WaveFormControl - HandleGeneratePeakFileEndedEvent - LoadPeakFile Cancelled: " + e.Cancelled.ToString() + " FilePath: " + e.AudioFilePath);
            //if (!e.Cancelled)
            //    _waveFormRenderingService.LoadPeakFile(new AudioFile(e.AudioFilePath));
        }

        private void HandleLoadedPeakFileSuccessfullyEvent(object sender, LoadPeakFileEventArgs e)
        {
			//Console.WriteLine("WaveFormControl - HandleLoadedPeakFileSuccessfullyEvent");
            if (Frame.Width == 0)
                return;

            // Start generating the first tile
            _waveFormCacheService.GetTile(0, Frame.Height, Frame.Width, Zoom);

            // Start generating all tiles for zoom @ 100%
            //for(int a = 0; a < Frame.Width; a = a + WaveFormCacheService.TileSize)
                //_waveFormCacheService.GetTile(a, Frame.Height, Frame.Width, Zoom);

            IsLoading = false;
            //OnInvalidateVisual();            
        }

        private void HandleGenerateWaveFormEndedEvent(object sender, GenerateWaveFormEventArgs e)
        {
            // Make sure the control isn't drawing when switching bitmaps
            //lock (_locker)
            //{
            //    //IsLoading = false;
            //}

            //Console.WriteLine("WaveFormControl - HandleGenerateWaveFormEndedEvent - e.Width: {0} e.Zoom: {1}", e.Width, e.Zoom);
            //OnInvalidateVisual();
            OnInvalidateVisualInRect(new BasicRectangle(e.OffsetX, 0, e.Width, Frame.Height));
        }

        public void SetActiveMarker(Guid markerId)
        {
            if (markerId != _activeMarkerId)
                InvalidateMarker(_activeMarkerId);

            _activeMarkerId = markerId;
            InvalidateMarker(_activeMarkerId);
        }

        private void InvalidateMarker(Guid markerId)
        {
            try
            {
                var marker = _markers.FirstOrDefault(x => x.MarkerId == markerId);
                if (marker == null)
                    return;

                float xPct = (float)marker.PositionBytes / (float)Length;
                float xMarker = xPct * Frame.Width;
                var rectCursor = new BasicRectangle(xMarker - 5, 0, 25, Frame.Height);
                OnInvalidateVisualInRect(rectCursor);
            }
            catch (Exception ex)
            {
                Console.WriteLine("WaveFormControl - InvalidateMarker - ex: {0}", ex);
            }
        }

        public void SetMarkers(IEnumerable<Marker> markers)
        {
            _markers = markers.ToList();
            OnInvalidateVisual();
        }

        public void SetMarkerPosition(Marker marker)
        {
            var localMarker = _markers.FirstOrDefault(x => x.MarkerId == marker.MarkerId);
            if (localMarker == null)
                return;

            localMarker.Position = marker.Position;
            localMarker.PositionBytes = marker.PositionBytes;
            localMarker.PositionPercentage = marker.PositionPercentage;
            localMarker.PositionSamples = marker.PositionSamples;

            // TODO: Only refresh the old/new marker positions
            OnInvalidateVisual();
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
			//Console.WriteLine("WaveFormControl - LoadPeakFile " + audioFile.FilePath);
            IsLoading = true;
            AudioFile = audioFile;
            RefreshStatus("Loading peak file...");
            _waveFormCacheService.LoadPeakFile(audioFile);
        }

        private void RefreshStatus(string status)
        {
			//Console.WriteLine("WaveFormControl - RefreshStatus - status: {0}", status);            
            //IsLoading = true;
            _status = status;
            OnInvalidateVisual();
        }

        private void DrawStatus(IGraphicsContext context, string status)
        {
            //Console.WriteLine("WaveFormControl - DrawStatus - status: {0}", status);
            context.DrawRectangle(Frame, new BasicBrush(_backgroundColor), new BasicPen());            
			var rectText = context.MeasureText(status, new BasicRectangle(0, 0, Frame.Width, 30), FontFace, FontSize);                
            float x = (context.BoundsWidth - rectText.Width) / 2;
            float y = context.BoundsHeight / 2;
            context.DrawText(status, new BasicPoint(x, y), _textColor, FontFace, FontSize);
        }

        private void DrawWaveFormBitmap(IGraphicsContext context)
        {          
            // Make sure that the bitmap doesn't change during drawing because a bitmap size mismatch can crash the app
            lock (_locker)
            {
                if (_penCursorLine == null)
                {
                    _penTransparent = new BasicPen();
                    _penCursorLine = new BasicPen(new BasicBrush(_cursorColor), 1);
                    _penSecondaryCursorLine = new BasicPen(new BasicBrush(_secondaryCursorColor), 1);
                    _penMarkerLine = new BasicPen(new BasicBrush(_markerCursorColor), 1);
                    _penSelectedMarkerLine = new BasicPen(new BasicBrush(_markerSelectedCursorColor), 1);
                    _brushMarkerBackground = new BasicBrush(_markerCursorColor);
                    _brushSelectedMarkerBackground = new BasicBrush(_markerSelectedCursorColor);
                }

                //// Draw bitmap cache
                ////Console.WriteLine("WaveFormControl - DrawBitmap - Frame.width: {0} Frame.height: {1}", Frame.Width, Frame.Height);
                //BasicRectangle rectImage;
                //if (Zoom != _imageCacheZoom)
                //{
                //    float deltaZoom = Zoom / _imageCacheZoom;
                //    //Console.WriteLine("WaveFormControl - DrawBitmap - Zoom != _imageCacheZoom - Zoom: {0} _imageCacheZoom: {1} deltaZoom: {2}", Zoom, _imageCacheZoom, deltaZoom);
                //    rectImage = new BasicRectangle(ContentOffset.X * (_density * (1 / deltaZoom)), 0, Frame.Width * _density * (1 / deltaZoom), Frame.Height * _density);
                //    //rectImage = new BasicRectangle(ContentOffset.X * (_density * (1 / deltaZoom)), 0, Frame.Width * _density * (1 / deltaZoom), Frame.Height * _density);
                //    //rectImage = new BasicRectangle(ContentOffset.X * (_density * (1 / deltaZoom)), 0, _imageCache.Item2 * _density * (1 / deltaZoom), Frame.Height * _density);
                //}
                //else
                //{
                //    //Console.WriteLine("WaveFormControl - DrawBitmap - Zoom == _imageCacheZoom");
                //    rectImage = new BasicRectangle((ContentOffset.X * Zoom) * (_density * (1 / Zoom)), 0, Frame.Width * _density, Frame.Height * _density);
                //    //rectImage = new BasicRectangle((ContentOffset.X * Zoom) * (_density * (1 / Zoom)), 0, _imageCache.Item2 * _density, Frame.Height * _density);
                //}

                //// Sometimes the control needs to be drawn but the image size in cache does not match the size of the new control, even though we're using thread locking (only on WPF)
                //context.DrawImage(Frame, rectImage, _imageCache);

                BasicPen penSeparator = new BasicPen(new BasicBrush(new BasicColor(255, 0, 0)), 1);
                BasicPen penSeparator2 = new BasicPen(new BasicBrush(new BasicColor(0, 0, 255)), 1);
                BasicPen penSeparator3 = new BasicPen(new BasicBrush(new BasicColor(255, 50, 255)), 1);
                int tileSize = WaveFormCacheService.TileSize;
                float delta = (float) (Zoom/Math.Floor(Zoom));
                int startTile = (int)Math.Floor(ContentOffset.X / ((float)tileSize * delta));
                int numberOfTilesToFillWidth = (int) Math.Ceiling(Frame.Width/tileSize);
                //Console.WriteLine(">>>>>>>>>>> startTile: {0} startTileX: {1} contentOffset.X: {2} contentOffset.X/tileSize: {3} numberOfTilesToFillWidth: {4} firstTileX: {5}", startTile, startTile * tileSize, ContentOffset.X, ContentOffset.X / tileSize, numberOfTilesToFillWidth, (startTile * tileSize) - ContentOffset.X);
                for (int a = startTile; a < startTile + numberOfTilesToFillWidth; a++)
                {
                    float tileX = a * tileSize;
                    //float offsetX = startTile * tileSize;                    
                    float offsetX = 0; // remove this
                    var tile = _waveFormCacheService.GetTile(tileX, Frame.Height, Frame.Width, Zoom);
                    if (tile != null)
                    {
                        //Console.WriteLine("WaveFormControl - Drawing tile {0} x: {1} offsetX: {2} startTile: {3}", a, x, offsetX, startTile);
                        if (tile.Zoom != Zoom)
                        {
                            //    float deltaZoom = Zoom / _imageCacheZoom;
                            //    //Console.WriteLine("WaveFormControl - DrawBitmap - Zoom != _imageCacheZoom - Zoom: {0} _imageCacheZoom: {1} deltaZoom: {2}", Zoom, _imageCacheZoom, deltaZoom);
                            //    rectImage = new BasicRectangle(ContentOffset.X * (_density * (1 / deltaZoom)), 0, Frame.Width * _density * (1 / deltaZoom), Frame.Height * _density);


                            // The tile received for this position doesn't match the current zoom.
                            // tileX is the perfect position for a tile. if the tile zoom doesn't match, this means the original tile x isn't right anymore.
                            // can we have overlaps? when stretching a bitmap
                            // We might have to cut the bitmap a bit? 

                            float deltaZoom = Zoom / tile.Zoom;
                            float x = (tileX - offsetX) * deltaZoom; //(1 / deltaZoom);
                            //float x = Zoom - tile.Zoom >= 0 ? (tileX - offsetX) * deltaZoom : (tileX - offsetX) * (1 / deltaZoom);
                            //float x = (tileX - offsetX);
                            //context.DrawImage(new BasicRectangle(x, 0, tileSize, Frame.Height), tile.Image);
                            context.DrawImage(new BasicRectangle(x - ContentOffset.X, 0, tileSize * deltaZoom, Frame.Height), new BasicRectangle(0, 0, tileSize, Frame.Height), tile.Image);
                            //context.DrawLine(new BasicPoint(x - ContentOffset.X, 0), new BasicPoint(x - ContentOffset.X, Frame.Height), penSeparator2);
                            //context.DrawLine(new BasicPoint(x + (tileSize * deltaZoom) - 1, 0), new BasicPoint(x + (tileSize * deltaZoom) - 1, Frame.Height), penSeparator3);
                        }
                        else
                        {
                            //    rectImage = new BasicRectangle((ContentOffset.X * Zoom) * (_density * (1 / Zoom)), 0, Frame.Width * _density, Frame.Height * _density);
                            //context.DrawImage(new BasicRectangle(x - offsetX, 0, tileSize, Frame.Height), tile.Image)

                            // The tile zoom level fits the current zoom level; no stretching needed
                            float x = tileX - offsetX;
                            context.DrawImage(new BasicRectangle(x - ContentOffset.X, 0, tileSize, Frame.Height), tile.Image);
                            //context.DrawLine(new BasicPoint(x - ContentOffset.X, 0), new BasicPoint(x - ContentOffset.X, Frame.Height), penSeparator2);
                            //context.DrawLine(new BasicPoint(x + tileSize - 1, 0), new BasicPoint(x + tileSize - 1, Frame.Height), penSeparator3);
                        }

                        //context.DrawLine(new BasicPoint(tileX - ContentOffset.X, 0), new BasicPoint(tileX - ContentOffset.X, Frame.Height), penSeparator);
                    }
                    else
                    {
                        //Console.WriteLine("[!!!] Missing bitmap - tileX: {0}", tileX);
                        //context.DrawRectangle(new BasicRectangle(tileX - ContentOffset.X, 0, tileSize, Frame.Height), new BasicBrush(new BasicColor(0, 0, 255)), penSeparator3);
                    }
                }
            }

            //Console.WriteLine("WaveFormControl - DrawWaveFormBitmap");
            int heightAvailable = (int)Frame.Height;

            // Calculate position
            float positionPercentage = (float)Position / (float)Length;
            _cursorX = (positionPercentage * ContentSize.Width) - ContentOffset.X;

            // Draw markers
            for (int a = 0; a < _markers.Count; a++)
            {
                float xPct = (float)_markers[a].PositionBytes / (float)Length;
                float x = (xPct * ContentSize.Width) - ContentOffset.X;

                // Draw cursor line
                var pen = _markers[a].MarkerId == _activeMarkerId ? _penSelectedMarkerLine : _penMarkerLine;
                context.SetPen(pen);
                context.StrokeLine(new BasicPoint(x, 0), new BasicPoint(x, heightAvailable));

                // Draw text
                var rectText = new BasicRectangle(x, 0, 12, 12);
                var brush = _markers[a].MarkerId == _activeMarkerId ? _brushSelectedMarkerBackground : _brushMarkerBackground;
				context.DrawRectangle(rectText, brush, _penTransparent);
                string letter = Conversion.IndexToLetter(a).ToString();
				context.DrawText(letter, new BasicPoint(x + 2, 0), _textColor, LetterFontFace, LetterFontSize);
            }

            // Draw cursor line
            context.SetPen(_penCursorLine);
            context.StrokeLine(new BasicPoint(_cursorX, 0), new BasicPoint(_cursorX, heightAvailable));

            // Check if a secondary cursor must be drawn (i.e. when changing position)
            if (ShowSecondaryPosition)
            {
                float secondaryPositionPercentage = (float)SecondaryPosition / (float)Length;                
                _secondaryCursorX = (secondaryPositionPercentage * ContentSize.Width) - ContentOffset.X;
                _secondaryCursorX = (float)Math.Round(_secondaryCursorX * 2) / 2; // Round to 0.5
                //Console.WriteLine("secondaryPositionPercentage: {0} secondaryCursorX: {1}", secondaryPositionPercentage, _secondaryCursorX);

                // Draw cursor line
                context.SetPen(_penSecondaryCursorLine);
                context.StrokeLine(new BasicPoint(_secondaryCursorX, 0), new BasicPoint(_secondaryCursorX, heightAvailable));
            }
        }

        public void Render(IGraphicsContext context)
        {
			//Console.WriteLine("WaveFormControl - Render");
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
            Frame = new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight);
            _density = context.Density;
            if (IsLoading)
            {
                //Console.WriteLine("WaveFormControl - Render - Drawing status... isLoading: {0}", IsLoading);
                DrawStatus(context, _status);
            }
            else if (!_waveFormCacheService.IsEmpty)
            {
                //Console.WriteLine("WaveFormControl - Render - Drawing wave form bitmap... isLoading: {0}", IsLoading);
                DrawWaveFormBitmap(context);
            }
            else
            {
                //Console.WriteLine("WaveFormControl - Render - Drawing empty background...");
                context.DrawRectangle(Frame, new BasicBrush(_backgroundColor), new BasicPen());
            }

            //stopwatch.Stop();
            //Console.WriteLine("WaveFormControl - Render - stopwatch: {0} ms", stopwatch.ElapsedMilliseconds);
        }

        public void MouseDown(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            _isMouseDown = true;
            if (AudioFile == null)
                return;

            ShowSecondaryPosition = true;
            long position = (long)(((x + ContentOffset.X) / ContentSize.Width) * Length);
            float positionPercentage = (float)position / (float)Length;
            //Console.WriteLine("positionPercentage: {0} x: {1} ContentSize.Width: {2}", positionPercentage, x, ContentSize.Width);
            SecondaryPosition = (long)(positionPercentage * Length);
        }

        public void MouseUp(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            _isMouseDown = false;
            if (AudioFile == null)
                return;

            ShowSecondaryPosition = false;
            long position = (long)(((x + ContentOffset.X) / ContentSize.Width) * Length);
            float positionPercentage = (float)position / (float)Length;
            if (button == MouseButtonType.Left)
            {
                Position = position;
                OnChangePosition(positionPercentage);
            }
        }

        public void MouseClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
        }

        public void MouseDoubleClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
        }

        public void MouseMove(float x, float y, MouseButtonType button)
        {
            //Console.WriteLine("WaveFormControl - MouseMove - x: {0} y: {1}", x, y);
            if (AudioFile == null)
                return;

            if (_isMouseDown)
            {
                long position = (long)(((x + ContentOffset.X) / ContentSize.Width) * Length);
                float positionPercentage = (float)position / (float)Length;
                //Console.WriteLine("positionPercentage: {0} x: {1} ContentSize.Width: {2}", positionPercentage, x, ContentSize.Width);
                SecondaryPosition = position;
                OnChangeSecondaryPosition(positionPercentage);
            } 
        }

        public void MouseLeave()
        {
        }

        public void MouseEnter()
        {
        }

        public void MouseWheel(float delta)
        {
        }

        public enum InputInteractionMode
        {
            Select = 0, ZoomIn = 1, ZoomOut = 2
        }
    }
}