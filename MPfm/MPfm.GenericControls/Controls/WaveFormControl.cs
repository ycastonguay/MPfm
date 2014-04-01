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
using System.Linq;
using System.Web.UI.WebControls;
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
        private IWaveFormRenderingService _waveFormRenderingService;
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
        private bool _isGeneratingImageCache = false;
        private IDisposable _imageCache = null;
        private float _imageCacheWidth;
        private float _imageCacheZoom;
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
                if (IsLoading || _imageCache == null)
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
            _waveFormRenderingService = Bootstrapper.GetContainer().Resolve<IWaveFormRenderingService>();
            _waveFormRenderingService.GeneratePeakFileBegunEvent += HandleGeneratePeakFileBegunEvent;
            _waveFormRenderingService.GeneratePeakFileProgressEvent += HandleGeneratePeakFileProgressEvent;
            _waveFormRenderingService.GeneratePeakFileEndedEvent += HandleGeneratePeakFileEndedEvent;
            _waveFormRenderingService.LoadedPeakFileSuccessfullyEvent += HandleLoadedPeakFileSuccessfullyEvent;
            _waveFormRenderingService.GenerateWaveFormBitmapBegunEvent += HandleGenerateWaveFormBegunEvent;
            _waveFormRenderingService.GenerateWaveFormBitmapEndedEvent += HandleGenerateWaveFormEndedEvent;

            _waveFormCacheService = Bootstrapper.GetContainer().Resolve<IWaveFormCacheService>();
        }

        private void HandleGeneratePeakFileBegunEvent(object sender, GeneratePeakFileEventArgs e)
        {
            //InvokeOnMainThread(() =>
			//Console.WriteLine("WaveFormControl - HandleGeneratePeakFileBegunEvent");
            RefreshStatus("Generating wave form (0% done)");
        }

        private void HandleGeneratePeakFileProgressEvent(object sender, GeneratePeakFileEventArgs e)
        {
            //InvokeOnMainThread(() =>
			//Console.WriteLine("WaveFormControl - HandleGeneratePeakFileProgressEvent  (" + e.PercentageDone.ToString("0") + "% done)");
            RefreshStatus("Generating wave form (" + e.PercentageDone.ToString("0") + "% done)");
        }

        private void HandleGeneratePeakFileEndedEvent(object sender, GeneratePeakFileEventArgs e)
        {
            //InvokeOnMainThread(() =>
            // TODO: Check if cancelled? This will not fire another LoadPeakFile if the peak file gen was cancelled.
			//Console.WriteLine("WaveFormControl - HandleGeneratePeakFileEndedEvent - LoadPeakFile Cancelled: " + e.Cancelled.ToString() + " FilePath: " + e.AudioFilePath);
            if (!e.Cancelled)
                _waveFormRenderingService.LoadPeakFile(new AudioFile(e.AudioFilePath));
        }

        private void HandleLoadedPeakFileSuccessfullyEvent(object sender, LoadPeakFileEventArgs e)
        {
            //InvokeOnMainThread(() =>
			//Console.WriteLine("WaveFormControl - HandleLoadedPeakFileSuccessfullyEvent");
            if (Frame.Width == 0)
                return;

            GenerateWaveFormBitmap(e.AudioFile, ContentSize);
        }

        private void HandleGenerateWaveFormBegunEvent(object sender, GenerateWaveFormEventArgs e)
        {
        }

        private void HandleGenerateWaveFormEndedEvent(object sender, GenerateWaveFormEventArgs e)
        {
            // Make sure the control isn't drawing when switching bitmaps
            lock (_locker)
            {
                Console.WriteLine("WaveFormControl - GenerateWaveFormEndedEvent (isLoading: false)");
                _isGeneratingImageCache = false;
                IsLoading = false;
                _imageCache = e.Image;
                _imageCacheWidth = e.Width;
                _imageCacheZoom = e.Zoom;
            }

            Console.WriteLine("WaveFormControl - HandleGenerateWaveFormEndedEvent - e.Width: {0} e.Zoom: {1}", e.Width, e.Zoom);
            OnInvalidateVisual();
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

        public void FlushCache()
        {
            // Make sure the control isn't drawing a bitmap when flushing it
            lock (_locker)
            {
                _waveFormRenderingService.FlushCache();

                if (_imageCache != null)
                {
                    _imageCache.Dispose();
                    _imageCache = null;
                }
            }
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
			//Console.WriteLine("WaveFormControl - LoadPeakFile " + audioFile.FilePath);
            IsLoading = true;
            _imageCache = null;
            _imageCacheWidth = 0;
            _imageCacheZoom = 1;
            AudioFile = audioFile;
            RefreshStatus("Loading peak file...");
            _waveFormRenderingService.LoadPeakFile(audioFile);
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

                int tileSize = WaveFormCacheService.TileSize;
                int startTile = (int) Math.Floor(ContentOffset.X/tileSize);
                int numberOfTilesToFillWidth = (int) Math.Ceiling(Frame.Width/tileSize);
                for (int a = startTile; a < startTile + numberOfTilesToFillWidth; a++)
                {
                    float x = a * tileSize;
                    float offsetX = startTile * tileSize;                    
                    var tile = _waveFormCacheService.GetTile(x, Frame.Height, Zoom);
                    if (tile != null)
                    {
                        Console.WriteLine("WaveFormControl - Drawing tile {0} x: {1} offsetX: {2} startTile: {3}", a, x, offsetX, startTile);
                        context.DrawImage(new BasicRectangle(x - offsetX, 0, tileSize, Frame.Height), tile.Image);
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

        public void RefreshWaveFormBitmap()
        {
            //RefreshWaveFormBitmap(ContentSize.Width);
            RefreshWaveFormBitmap(Frame.Width);
        }

        public void RefreshWaveFormBitmap(float width)
        {
            if (AudioFile == null)
                return;

            //RefreshStatus("Generating new bitmap...");
            GenerateWaveFormBitmap(AudioFile, new BasicRectangle(Frame.X, Frame.Y, width, Frame.Height));
        }

        private void GenerateWaveFormBitmap(AudioFile audioFile, BasicRectangle rect)
        {
            if (!_isGeneratingImageCache && (rect.Width * _density != _imageCacheWidth || _imageCacheZoom != Zoom))
            {
                _isGeneratingImageCache = true;
                var rectImage = new BasicRectangle(0, 0, rect.Width * _density, rect.Height * _density);
                Console.WriteLine("WaveFormControl - GenerateWaveFormBitmap audioFilePath: {0} rect.width: {1} rect.height: {2}", audioFile.FilePath, rectImage.Width, rectImage.Height);
                _waveFormRenderingService.RequestBitmap(WaveFormDisplayType.Stereo, rectImage, Zoom);
            }
        }

        public void Render(IGraphicsContext context)
        {
			//Console.WriteLine("WaveFormControl - Render");
            Frame = new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight);
            _density = context.Density;
            if (IsLoading)
            {
                //Console.WriteLine("WaveFormControl - Render - Drawing status... isLoading: {0}", IsLoading);
                DrawStatus(context, _status);
            }
            else if (_imageCache != null)
            {
                //Console.WriteLine("WaveFormControl - Render - Drawing wave form bitmap... isLoading: {0}", IsLoading);
                DrawWaveFormBitmap(context);
            }
            else
            {
                //Console.WriteLine("WaveFormControl - Render - Drawing empty background...");
                context.DrawRectangle(Frame, new BasicBrush(_backgroundColor), new BasicPen());
            }
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