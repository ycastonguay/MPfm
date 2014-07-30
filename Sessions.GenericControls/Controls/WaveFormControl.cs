// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using Sessions.GenericControls.Interaction;
using Sessions.GenericControls.Services;
using Sessions.GenericControls.Services.Events;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Services.Objects;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Graphics;
using Sessions.Core;
using Sessions.MVP.Bootstrap;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;

namespace Sessions.GenericControls.Controls
{
    /// <summary>
    /// The WaveFormScale control displays the scale in minutes/seconds on top of the wave form.
    /// </summary>
    public class WaveFormControl : IControl, IControlMouseInteraction
    {
        private const int ScrollBarHeight = 16;//20;
        private readonly object _locker = new object();
        private IWaveFormCacheService _waveFormCacheService;
        private bool _isMouseDown;
        private bool _isDraggingThumb;
        private bool _isDraggingScrollBar;
        private float _thumbMouseDownX;
        private float _mouseDownX;
        private float _density;
        private BasicPen _penTransparent;
        private BasicPen _penCursorLine;
        private BasicPen _penSecondaryCursorLine;
        private BasicPen _penLoopLine;
        private BasicPen _penMarkerLine;
        private BasicPen _penSelectedMarkerLine;
        private BasicBrush _brushLoopBackground;
        private BasicBrush _brushMarkerBackground;
        private BasicBrush _brushSelectedMarkerBackground;
        private Guid _activeMarkerId = Guid.Empty;
        private Loop _loop;
        private List<Marker> _markers = new List<Marker>();
        private string _status = "";
        private float _cursorX;
        private float _secondaryCursorX;
		private BasicColor _backgroundColor = new BasicColor(32, 40, 46);        
        private BasicColor _cursorColor = new BasicColor(0, 128, 255);
        private BasicColor _secondaryCursorColor = new BasicColor(255, 255, 255);
        private BasicColor _loopCursorColor = new BasicColor(0, 0, 255);
        private BasicColor _loopBackgroundColor = new BasicColor(0, 0, 255, 125);
        private BasicColor _markerCursorColor = new BasicColor(255, 0, 0);
        private BasicColor _markerSelectedCursorColor = new BasicColor(234, 138, 128);
        private BasicColor _markerBackgroundColor = new BasicColor(255, 0, 0, 125);
        private BasicColor _textColor = new BasicColor(255, 255, 255);

        public bool IsEmpty { get; private set; }
        public bool IsLoading { get; private set; }
        public InputInteractionMode InteractionMode { get; set; }
        public BasicRectangle Frame { get; set; }
		public float FontSize { get; set; }
		public string FontFace { get; set; }
		public float LetterFontSize { get; set; }
		public string LetterFontFace { get; set; }
        public bool ShowScrollBar { get; set; }

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

                // Calculate position
                float positionPercentage = (float)_position / (float)Length;
                float cursorX = (positionPercentage * ContentSize.Width) - ContentOffset.X;
                float scrollBarCursorX = positionPercentage * Frame.Width;

                // Invalidate cursor
                float zoomAdjustment = Math.Max(1, Zoom/4);
                var rectCursor = new BasicRectangle(cursorX - (5 * zoomAdjustment), 0, 10 * zoomAdjustment, Frame.Height);
				var rectCursorScrollBar = new BasicRectangle(scrollBarCursorX - (5 * zoomAdjustment), Frame.Height - 30, 10 * zoomAdjustment, 30);
                OnInvalidateVisualInRect(rectCursor);
                OnInvalidateVisualInRect(rectCursorScrollBar); // WARNING: Maybe it is a better idea to merge the dirty rectangles.
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

                float secondaryPositionPercentage = (float)_secondaryPosition / (float)Length;                
                float secondaryCursorX = (secondaryPositionPercentage * ContentSize.Width) - ContentOffset.X;
                secondaryCursorX = (float)Math.Round(secondaryCursorX * 2) / 2; // Round to 0.5

                float zoomAdjustment = Math.Max(1, Zoom / 4);
                var rectCursor = new BasicRectangle(secondaryCursorX - (5 * zoomAdjustment), 0, 25 * zoomAdjustment, Frame.Height);
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
                OnInvalidateVisual();
            }
        }
        
        private BasicRectangle ContentSize
        {
            get
            {
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
                OnInvalidateVisual();
            }
        }

        private WaveFormDisplayType _displayType = WaveFormDisplayType.Stereo;
        public WaveFormDisplayType DisplayType
        {
            get
            {
                return _displayType;
            }
            set
            {
                _displayType = value;
                InvalidateBitmaps();
            }
        }

        public AudioFile AudioFile { get; private set; }
        public bool ShowSecondaryPosition { get; set; }
        public long Length { get; set; }

        public delegate void ChangePosition(float position);
        public delegate void ContentOffsetChanged(BasicPoint offset);
        public event InvalidateVisual OnInvalidateVisual;
        public event InvalidateVisualInRect OnInvalidateVisualInRect;
        public event ChangePosition OnChangePosition;
        public event ChangePosition OnChangeSecondaryPosition;
        public event ContentOffsetChanged OnContentOffsetChanged;

        public WaveFormControl()
        {
            Initialize();
        }

        private void Initialize()
        {
            IsEmpty = true;
            ShowScrollBar = true;
            DisplayType = WaveFormDisplayType.Stereo;
            OnInvalidateVisual += () => { };
            OnInvalidateVisualInRect += (rect) => { };
            OnChangePosition += (position) => { };
            OnContentOffsetChanged += (offset) => { };

            _waveFormCacheService = Bootstrapper.GetContainer().Resolve<IWaveFormCacheService>();
            _waveFormCacheService.GeneratePeakFileBegunEvent += HandleGeneratePeakFileBegunEvent;
            _waveFormCacheService.GeneratePeakFileProgressEvent += HandleGeneratePeakFileProgressEvent;
            _waveFormCacheService.GeneratePeakFileEndedEvent += HandleGeneratePeakFileEndedEvent;
            _waveFormCacheService.LoadedPeakFileSuccessfullyEvent += HandleLoadedPeakFileSuccessfullyEvent;
            _waveFormCacheService.GenerateWaveFormBitmapEndedEvent += HandleGenerateWaveFormEndedEvent;

            CreateDrawingResources();
        }

        private void CreateDrawingResources()
        {
            FontSize = 12;
            FontFace = "Roboto Light";
            LetterFontSize = 10;
            LetterFontFace = "Roboto";
            Frame = new BasicRectangle();

            _penTransparent = new BasicPen();
            _penCursorLine = new BasicPen(new BasicBrush(_cursorColor), 1);
            _penSecondaryCursorLine = new BasicPen(new BasicBrush(_secondaryCursorColor), 1);
            _penMarkerLine = new BasicPen(new BasicBrush(_markerCursorColor), 1);
            _penLoopLine = new BasicPen(new BasicBrush(_loopCursorColor), 1);
            _penSelectedMarkerLine = new BasicPen(new BasicBrush(_markerSelectedCursorColor), 1);
            _brushLoopBackground = new BasicBrush(_loopBackgroundColor);
            _brushMarkerBackground = new BasicBrush(_markerBackgroundColor);
            _brushSelectedMarkerBackground = new BasicBrush(_markerSelectedCursorColor);
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

            InvalidateBitmaps();
        }

        private void HandleLoadedPeakFileSuccessfullyEvent(object sender, LoadPeakFileEventArgs e)
        {
			//Console.WriteLine("WaveFormControl - HandleLoadedPeakFileSuccessfullyEvent");
            InvalidateBitmaps();
        }

        private void HandleGenerateWaveFormEndedEvent(object sender, GenerateWaveFormEventArgs e)
        {
            //Console.WriteLine("WaveFormControl - HandleGenerateWaveFormEndedEvent - e.Width: {0} e.Zoom: {1}", e.Width, e.Zoom);
            float deltaZoom = Zoom / e.Zoom;
            float offsetX = (e.OffsetX*deltaZoom) - ContentOffset.X;
            //OnInvalidateVisualInRect(new BasicRectangle(offsetX, 0, e.Width, Frame.Height));
            OnInvalidateVisual();
        }

        public void InvalidateBitmaps()
        {
            if (Frame == null || Frame.Width == 0)
                return;

            //Console.WriteLine("=========> WaveFormControl - InvalidateBitmaps");
            // Start generating the first tile
            _waveFormCacheService.FlushCache();
            //_waveFormCacheService.GetTile(0, Frame.Height, Frame.Width, Zoom);

            // Start generating all tiles for zoom @ 100%
            //for(int a = 0; a < Frame.Width; a = a + WaveFormCacheService.TileSize)
            //_waveFormCacheService.GetTile(a, Frame.Height, Frame.Width, Zoom);

            IsLoading = false;
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

        public void SetLoop(Loop loop)
        {
            _loop = loop;
            OnInvalidateVisual();
        }

        public void SetSegment(Segment segment)
        {
            if (_loop == null)
                return;

            var localSegment = _loop.Segments.FirstOrDefault(x => x.SegmentId == segment.SegmentId);
            if (localSegment == null)
                return;

            localSegment.Position = segment.Position;
            localSegment.PositionBytes = segment.PositionBytes;
            localSegment.PositionSamples = segment.PositionSamples;

            OnInvalidateVisual();
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
			//Console.WriteLine("WaveFormControl - LoadPeakFile " + audioFile.FilePath);
            IsLoading = true;
            IsEmpty = false;
            AudioFile = audioFile;
            RefreshStatus("Loading peak file...");
            _waveFormCacheService.LoadPeakFile(audioFile);
        }

        public void Reset()
        {
            IsEmpty = true;            
            IsLoading = false;            
            _waveFormCacheService.FlushCache();
            OnInvalidateVisual();
        }

        private void RefreshStatus(string status)
        {
			//Console.WriteLine("WaveFormControl - RefreshStatus - status: {0}", status);            
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
            // The scroll bar slowly appears from zoom 100% to 200%. 
            // This enables a smoother effect when zooming, especially with touch input.
            int realScrollBarHeight = (int)(Zoom <= 2 ? ((Zoom - 1) * ScrollBarHeight) : ScrollBarHeight);
            int heightAvailable = (int)Frame.Height;
            int tileSize = WaveFormCacheService.TileSize;

            // Calculate position
            float positionPercentage = (float)Position / (float)Length;
            _cursorX = (positionPercentage * ContentSize.Width) - ContentOffset.X;
            float scrollBarCursorX = positionPercentage * Frame.Width;
            float cursorHeight = heightAvailable - realScrollBarHeight;

            DrawTiles(context, tileSize, realScrollBarHeight);
            DrawScrollBar(context, tileSize, realScrollBarHeight);
            //context.DrawRectangle(context.DirtyRect, new BasicBrush(new BasicColor(255, 255, 0, 100)), _penTransparent); // for debugging
            DrawMarkers(context, cursorHeight);
            DrawLoops(context, cursorHeight, realScrollBarHeight);
            DrawCursors(context, heightAvailable, cursorHeight, scrollBarCursorX);
        }

        private void DrawTiles(IGraphicsContext context, int tileSize, float realScrollBarHeight)
        {
            float deltaZoom = (float) (Zoom/Math.Floor(Zoom));
            int startDirtyTile = (int)Math.Floor((ContentOffset.X + context.DirtyRect.X) / ((float)tileSize * deltaZoom));
            int numberOfDirtyTilesToDraw = (int)Math.Ceiling(context.DirtyRect.Width / tileSize) + 1;
            var request = new WaveFormBitmapRequest()
            {
                StartTile = startDirtyTile,
                EndTile = startDirtyTile + numberOfDirtyTilesToDraw,
                TileSize = tileSize,
                BoundsWaveForm = Frame,
                Zoom = _zoom,
                DisplayType = _displayType
            };
            var tiles = _waveFormCacheService.GetTiles(request);

            //Console.WriteLine("WaveFormControl - GetTiles - startTile: {0} startTileX: {1} contentOffset.X: {2} contentOffset.X/tileSize: {3} numberOfDirtyTilesToDraw: {4} firstTileX: {5}", request.StartTile, request.StartTile * tileSize, ContentOffset.X, ContentOffset.X / tileSize, numberOfDirtyTilesToDraw, (request.StartTile * tileSize) - ContentOffset.X);
            //Console.WriteLine("WaveFormControl - Draw - GetTiles - ContentOffset.X: {0} context.DirtyRect.X: {1} tileSize: {2} deltaZoom: {3}", ContentOffset.X, context.DirtyRect.X, tileSize, deltaZoom);
            //Console.WriteLine("WaveFormControl - Draw - GetTiles - startTile: {0} endTile: {1} numberOfDirtyTilesToDraw: {2}", request.StartTile, request.EndTile, numberOfDirtyTilesToDraw);
            foreach (var tile in tiles)
            {
                float tileDeltaZoom = Zoom / tile.Zoom;
                float x = tile.ContentOffset.X * tileDeltaZoom;
                float tileWidth = tileSize * tileDeltaZoom;
                float tileHeight = (ShowScrollBar && Zoom > 1) ? Frame.Height - realScrollBarHeight : Frame.Height;
                //Console.WriteLine("WaveFormControl - Draw - tile - x: {0} tileWidth: {1} deltaZoom: {2}", x, tileWidth, deltaZoom);
                //Console.WriteLine("WaveFormControl - Draw - tile - tile.ContentOffset.X: {0} x: {1} tileWidth: {2} tile.Zoom: {3} tileDeltaZoom: {4}", tile.ContentOffset.X, x, tileWidth, tile.Zoom, tileDeltaZoom);

                context.DrawImage(new BasicRectangle(x - ContentOffset.X, 0, tileWidth, Frame.Height), tile.Image.ImageSize, tile.Image.Image);
                context.DrawImage(new BasicRectangle(x - ContentOffset.X, 0, tileWidth, tileHeight), tile.Image.ImageSize, tile.Image.Image);

                // Debug overlay
//                string debugText = string.Format("{0:0.0}", tile.Zoom);
//                context.DrawRectangle(new BasicRectangle(x - ContentOffset.X, 0, tileWidth, Frame.Height), new BasicBrush(new BasicColor(0, 0, 255, 50)), _penCursorLine);
//                context.DrawText(debugText, new BasicPoint(x - ContentOffset.X + 2, 4), _textColor, "Roboto", 11);
            }
        }

        private void DrawScrollBar(IGraphicsContext context, int tileSize, float realScrollBarHeight)
        {
            if (ShowScrollBar && Zoom > 1)
            {
                int startTile = 0;
                int numberOfTilesToFillWidth = (int)Math.Ceiling(Frame.Width / tileSize);// + 1; // maybe a bug here? when one of the tile is partially drawn, you need another one?
                var requestScrollBar = new WaveFormBitmapRequest()
                {
                    StartTile = startTile,
                    EndTile = numberOfTilesToFillWidth,
                    TileSize = tileSize,
                    BoundsWaveForm = new BasicRectangle(0, 0, Frame.Width, ScrollBarHeight), // Frame
                    Zoom = 1,
                    IsScrollBar = true,
                    DisplayType = _displayType
                };
                // TODO: Cache those tiles, we do not need to request them continually since these tiles are always at 100%
                var tilesScrollBar = _waveFormCacheService.GetTiles(requestScrollBar);
                foreach (var tile in tilesScrollBar)
                {
                    context.DrawImage(new BasicRectangle(tile.ContentOffset.X, Frame.Height - realScrollBarHeight, tileSize, realScrollBarHeight), tile.Image.ImageSize, tile.Image.Image);
                }

                // Draw a veil over the area that's not visible. The veil alpha gets stronger as the zoom progresses.
                byte startAlpha = 170;
                byte maxAlpha = 210;
                byte alpha = (byte)Math.Min(maxAlpha, (startAlpha + (60 * (Zoom / 10))));
                var colorVisibleArea = new BasicColor(32, 40, 46, alpha);
                float visibleAreaWidth = (1 / Zoom) * Frame.Width;
                float visibleAreaX = (1 / Zoom) * ContentOffset.X;
                var rectLeftArea = new BasicRectangle(0, Frame.Height - realScrollBarHeight, Math.Max(0, visibleAreaX), realScrollBarHeight);
                var rectRightArea = new BasicRectangle(visibleAreaX + visibleAreaWidth, Frame.Height - realScrollBarHeight, Math.Max(0, Frame.Width - visibleAreaX - visibleAreaWidth), realScrollBarHeight);
                //context.DrawRectangle(new BasicRectangle(visibleAreaX, Frame.Height - scrollBarHeight, visibleAreaWidth, scrollBarHeight), new BasicBrush(colorVisibleArea), new BasicPen());
                context.DrawRectangle(rectLeftArea, new BasicBrush(colorVisibleArea), new BasicPen());
                context.DrawRectangle(rectRightArea, new BasicBrush(colorVisibleArea), new BasicPen());
            }
        }

        private void DrawMarkers(IGraphicsContext context, float cursorHeight)
        {
            for (int a = 0; a < _markers.Count; a++)
            {
                float xPct = (float)_markers[a].PositionBytes / (float)Length;
                float x = (xPct * ContentSize.Width) - ContentOffset.X;

                // Draw cursor line
                var pen = _markers[a].MarkerId == _activeMarkerId ? _penSelectedMarkerLine : _penMarkerLine;
                context.SetPen(pen);
                context.StrokeLine(new BasicPoint(x, 0), new BasicPoint(x, cursorHeight));

                // Draw text
                var rectText = new BasicRectangle(x, 0, 12, 12);
                var brush = _markers[a].MarkerId == _activeMarkerId ? _brushSelectedMarkerBackground : _brushMarkerBackground;
                context.DrawRectangle(rectText, brush, _penTransparent);
                string letter = Conversion.IndexToLetter(a).ToString();
                context.DrawText(letter, new BasicPoint(x + 2, 0), _textColor, LetterFontFace, LetterFontSize);
            }
        }

        private void DrawLoops(IGraphicsContext context, float cursorHeight, float realScrollBarHeight)
        {
            if (_loop != null)
            {
                for (int a = 0; a < _loop.Segments.Count; a++)
                {
                    var nextSegment = _loop.GetNextSegment(a);

                    float segmentPositionPercentage = (float)_loop.Segments[a].PositionBytes / (float)Length;
                    float startX = (segmentPositionPercentage * ContentSize.Width) - ContentOffset.X;

                    float nextSegmentPositionPercentage = 0;
                    float endX = 0;
                    if (nextSegment != null)
                    {
                        nextSegmentPositionPercentage = (float)nextSegment.PositionBytes / (float)Length;
                        endX = (nextSegmentPositionPercentage * ContentSize.Width) - ContentOffset.X;
                    }

                    // Draw loop lines
                    //var pen = _markers[a].MarkerId == _activeMarkerId ? _penSelectedMarkerLine : _penMarkerLine;
                    context.SetPen(_penLoopLine);
                    context.StrokeLine(new BasicPoint(startX, 0), new BasicPoint(startX, cursorHeight));

                    // Draw text
                    //var rectText = new BasicRectangle(startX, Frame.Height - 12, 12, 12);
                    var rectText = new BasicRectangle(startX, Frame.Height - realScrollBarHeight -12, endX > startX ? endX - startX : 12, 12);
                    //var brush = _markers [a].MarkerId == _activeMarkerId ? _brushSelectedMarkerBackground : _brushMarkerBackground;
                    context.DrawRectangle(rectText, _brushLoopBackground, _penTransparent);
                    context.DrawText((a+1).ToString(), new BasicPoint(startX + 2, Frame.Height - realScrollBarHeight - 12), _textColor, LetterFontFace, LetterFontSize);
                }
            }
        }

        private void DrawCursors(IGraphicsContext context, float heightAvailable, float cursorHeight, float scrollBarCursorX)
        {
            // Draw cursor line
            context.SetPen(_penCursorLine);
            context.StrokeLine(new BasicPoint(_cursorX, 0), new BasicPoint(_cursorX, cursorHeight));
            context.StrokeLine(new BasicPoint(scrollBarCursorX, cursorHeight), new BasicPoint(scrollBarCursorX, heightAvailable));

            // Check if a secondary cursor must be drawn (i.e. when changing position)
            if (ShowSecondaryPosition)
            {
                float secondaryPositionPercentage = (float)SecondaryPosition / (float)Length;                
                _secondaryCursorX = (secondaryPositionPercentage * ContentSize.Width) - ContentOffset.X;
                _secondaryCursorX = (float)Math.Round(_secondaryCursorX * 2) / 2; // Round to 0.5
                //Console.WriteLine("secondaryPositionPercentage: {0} secondaryCursorX: {1}", secondaryPositionPercentage, _secondaryCursorX);

                // Draw cursor line
                context.SetPen(_penSecondaryCursorLine);
                context.StrokeLine(new BasicPoint(_secondaryCursorX, 0), new BasicPoint(_secondaryCursorX, cursorHeight));
            }
        }

        public void Render(IGraphicsContext context)
        {
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
            Frame = new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight);
            _density = context.Density;
            if (IsLoading)
            {
                //Console.WriteLine("WaveFormControl - Render - Drawing status... isLoading: {0}", IsLoading);
                DrawStatus(context, _status);
            }
            else if (IsEmpty)
            {
                DrawStatus(context, "No peak file information.");
            }
            else
            {
                //Console.WriteLine("WaveFormControl - Render - Drawing wave form bitmap... isLoading: {0}", IsLoading);
                DrawWaveFormBitmap(context);
            }

            //stopwatch.Stop();
            //Console.WriteLine("WaveFormControl - Render - stopwatch: {0} ms", stopwatch.ElapsedMilliseconds);
        }

        public void MouseDown(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            _isMouseDown = true;
            if (AudioFile == null)
                return;

            if (ShowScrollBar && y >= Frame.Height - ScrollBarHeight)
            {
                // ScrollBar area
                _isDraggingScrollBar = true;
                float visibleAreaWidth = (1 / Zoom) * Frame.Width;
                float visibleAreaX = (1 / Zoom) * ContentOffset.X;
                if (x >= visibleAreaX && x <= visibleAreaX + visibleAreaWidth)
                {
                    // User is dragging the thumb
                    _isDraggingThumb = true;
                    _mouseDownX = x;
                    _thumbMouseDownX = visibleAreaX;
                    //Console.WriteLine("Dragging thumb - _thumbMouseDownX: {0}", _thumbMouseDownX);
                }
            } 
            else
            {
                // Wave form area
                ShowSecondaryPosition = true;
                long position = (long)(((x + ContentOffset.X) / ContentSize.Width) * Length);
                float positionPercentage = (float)position / (float)Length;
                //Console.WriteLine("positionPercentage: {0} x: {1} ContentSize.Width: {2}", positionPercentage, x, ContentSize.Width);
                SecondaryPosition = (long)(positionPercentage * Length);
            }
        }

        public void MouseUp(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            _isMouseDown = false;
            if (AudioFile == null)
                return;

            if (_isDraggingScrollBar)
            {
                _isDraggingThumb = false;
                _isDraggingScrollBar = false;
            } 
            else
            {
                ShowSecondaryPosition = false;
                long position = (long)(((x + ContentOffset.X) / ContentSize.Width) * Length);
                float positionPercentage = (float)position / (float)Length;
                if (button == MouseButtonType.Left)
                {
                    Position = position;
                    OnChangePosition(positionPercentage);
                }
            }
        }

        public void MouseClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            float scrollUnit = ScrollBarHeight * Zoom;
            if (ShowScrollBar && y >= Frame.Height - ScrollBarHeight)
            {
                // Make sure we are outside the thumb/visible area
                float visibleAreaWidth = (1 / Zoom) * Frame.Width;
                float visibleAreaX = (1 / Zoom) * ContentOffset.X;
                if (x < visibleAreaX)
                    OnContentOffsetChanged(new BasicPoint(ContentOffset.X - scrollUnit, 0));
                else if (x > visibleAreaX + visibleAreaWidth)
                    OnContentOffsetChanged(new BasicPoint(ContentOffset.X + scrollUnit, 0));
            } 
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
                if (_isDraggingScrollBar)
                {
                    if (_isDraggingThumb)
                    {
                        float visibleAreaWidth = (1 / Zoom) * Frame.Width;
                        float trackWidth = Frame.Width - visibleAreaWidth;
                        float newThumbX = (x - _mouseDownX) + _thumbMouseDownX;
                        float scrollWidth = (Frame.Width * Zoom) - Frame.Width;
                        float newThumbRatio = newThumbX / trackWidth;
                        float newContentOffsetX = newThumbRatio * scrollWidth;
                        //Console.WriteLine("ContentOffset change to {0} - _thumbMouseDownX: {1}", newContentOffsetX, _thumbMouseDownX);
                        OnContentOffsetChanged(new BasicPoint(newContentOffsetX, 0));
                    }
                } 
                else
                {
                    long position = (long)(((x + ContentOffset.X) / ContentSize.Width) * Length);
                    float positionPercentage = (float)position / (float)Length;
                    //Console.WriteLine("positionPercentage: {0} x: {1} ContentSize.Width: {2}", positionPercentage, x, ContentSize.Width);
                    SecondaryPosition = position;
                    OnChangeSecondaryPosition(positionPercentage);
                }
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