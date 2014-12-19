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
using Sessions.GenericControls.Controls.Base;
using Sessions.GenericControls.Controls.Interfaces;
using Sessions.GenericControls.Interaction;
using Sessions.GenericControls.Services;
using Sessions.GenericControls.Services.Events;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Graphics;
using Sessions.Core;
using Sessions.MVP.Bootstrap;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using Sessions.GenericControls.Services.Objects;
using Sessions.GenericControls.Controls.Items;
using Sessions.GenericControls.Helpers;

namespace Sessions.GenericControls.Controls
{
    /// <summary>
    /// The WaveFormScale control displays the scale in minutes/seconds on top of the wave form.
    /// </summary>
    public class WaveFormControl : ControlBase, IControlMouseInteraction, IScrubbingSpeedSupport
    {
        private const int ScrollBarHeight = 8;
        private IWaveFormEngineService _waveFormEngineService;
        private bool _isMouseDown;
        private bool _isDraggingThumb;
        private bool _isDraggingScrollBar;
        private bool _isDraggingSegment;
        private float _thumbMouseDownX;
        private float _mouseDownX;
        private float _mouseDownY;
        private float _mouseDownScrubbingX;
        private float _mouseDownScrubbingValue;
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
        private MouseCursorType _cursorType;
        private Segment _segmentMouseOver;
        private Segment _segmentDragging;
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
                var previousValue = _position;
                _position = value;

                // Don't bother if a peak file is loading
                if (IsLoading)// || _imageCache == null)
                    return;

                // Calculate position
                float positionPercentage = (float)_position / (float)Length;
                float cursorX = (positionPercentage * ContentSize.Width) - ContentOffset.X;
                float scrollBarCursorX = positionPercentage * Frame.Width;

                float previousValuePercentage = (float)previousValue / (float)Length;                
                float previousValueX = (previousValuePercentage * ContentSize.Width) - ContentOffset.X;
                previousValueX = (float)Math.Round(previousValueX * 2) / 2; // Round to 0.5

                // Invalidate cursor
                var rectCursor = new BasicRectangle(cursorX - 5, 0, 15, Frame.Height);
                var rectPreviousCursor = new BasicRectangle(previousValueX - 5, 0, 15, Frame.Height);
                var rectCursorScrollBar = new BasicRectangle(scrollBarCursorX - 5, Frame.Height - ScrollBarHeight, 15, ScrollBarHeight);

                rectCursor.Merge(rectPreviousCursor);
                rectCursor.Merge(rectCursorScrollBar);
                InvalidateVisualInRect(rectCursor);
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
                var previousValue = _secondaryPosition;
                _secondaryPosition = value;

                // Don't bother if a peak file is loading
                if (IsLoading)
                    return;

                float secondaryPositionPercentage = (float)_secondaryPosition / (float)Length;                
                float secondaryCursorX = (secondaryPositionPercentage * ContentSize.Width) - ContentOffset.X;
                secondaryCursorX = (float)Math.Round(secondaryCursorX * 2) / 2; // Round to 0.5

                float previousValuePercentage = (float)previousValue / (float)Length;                
                float previousValueX = (previousValuePercentage * ContentSize.Width) - ContentOffset.X;
                previousValueX = (float)Math.Round(previousValueX * 2) / 2; // Round to 0.5

                var rectCursor = new BasicRectangle(secondaryCursorX - 5, 0, 15, Frame.Height);
                var rectPreviousCursor = new BasicRectangle(previousValueX - 5, 0, 15, Frame.Height);

                // Calling two times = completely draw two times. It'd be a better idea to merge the dirty rects.
                rectCursor.Merge(rectPreviousCursor);

                // TODO: Check if auto scroll + invalidate is superfluous
                ProcessAutoScroll(_secondaryPosition);
                InvalidateVisualInRect(rectCursor);
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
                InvalidateVisual();
            }
        }
        
        public BasicRectangle ContentSize
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
                InvalidateVisual();
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

        private List<ScrubbingSpeed> _scrubbingSpeeds;
        private ScrubbingSpeed _currentScrubbingSpeed;
        public ScrubbingSpeed CurrentScrubbingSpeed
        {
            get { return _currentScrubbingSpeed; }
        }

        public AudioFile AudioFile { get; private set; }
        public bool ShowSecondaryPosition { get; set; }
        public long Length { get; set; }

        public delegate void ChangePosition(float positionPercentage);
        public delegate void ChangeSegmentPosition(Segment segment, float positionPercentage);
        public delegate void ContentOffsetChanged(BasicPoint offset);
        public delegate void ChangeMouseCursorType(MouseCursorType mouseCursorType);
        public event ChangePosition OnChangePosition;
        public event ChangePosition OnChangeSecondaryPosition;
        public event ChangeSegmentPosition OnChangingSegmentPosition;
        public event ChangeSegmentPosition OnChangedSegmentPosition;
        public event ContentOffsetChanged OnContentOffsetChanged;
        public event ChangeMouseCursorType OnChangeMouseCursorType;
        public event ScrubbingSpeedChangedDelegate OnScrubbingSpeedChanged;

        public WaveFormControl()
        {
            Initialize();
        }

        private void Initialize()
        {
            OnChangePosition += (position) => { };
            OnChangeSecondaryPosition += position => { };
            OnChangingSegmentPosition += (segment, position) => { };
            OnChangedSegmentPosition += (segment, position) => { };
            OnContentOffsetChanged += (offset) => { };
            OnChangeMouseCursorType += type => { };
            
            IsEmpty = true;
            ShowScrollBar = true;
            DisplayType = WaveFormDisplayType.Stereo;

            _waveFormEngineService = Bootstrapper.GetContainer().Resolve<IWaveFormEngineService>();
            _waveFormEngineService.GeneratePeakFileBegunEvent += HandleGeneratePeakFileBegunEvent;
            _waveFormEngineService.GeneratePeakFileProgressEvent += HandleGeneratePeakFileProgressEvent;
            _waveFormEngineService.GeneratePeakFileEndedEvent += HandleGeneratePeakFileEndedEvent;
            _waveFormEngineService.LoadedPeakFileSuccessfullyEvent += HandleLoadedPeakFileSuccessfullyEvent;
            _waveFormEngineService.GenerateWaveFormBitmapEndedEvent += HandleGenerateWaveFormEndedEvent;

            CreateDrawingResources();
            CreateScrubbingSpeeds();
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

        private void CreateScrubbingSpeeds()
        {
            _scrubbingSpeeds = ScrubbingSpeedHelper.GetScrubbingSpeeds();
            _currentScrubbingSpeed = _scrubbingSpeeds[0];
        }

        protected void ScrubbingSpeedChanged(ScrubbingSpeed scrubbingSpeed)
        {
            if (OnScrubbingSpeedChanged != null)
                OnScrubbingSpeedChanged(scrubbingSpeed);
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

            IsLoading = false;
            InvalidateBitmaps();
        }

        private void HandleLoadedPeakFileSuccessfullyEvent(object sender, LoadPeakFileEventArgs e)
        {
			//Console.WriteLine("WaveFormControl - HandleLoadedPeakFileSuccessfullyEvent");
            IsLoading = false;
            InvalidateBitmaps();
        }

        private void HandleGenerateWaveFormEndedEvent(object sender, GenerateWaveFormEventArgs e)
        {
            //Console.WriteLine("WaveFormControl - HandleGenerateWaveFormEndedEvent - e.Width: {0} e.Zoom: {1}", e.Width, e.Zoom);
            //float deltaZoom = Zoom / e.Zoom;
            //float offsetX = (e.OffsetX*deltaZoom) - ContentOffset.X;
            //OnInvalidateVisualInRect(new BasicRectangle(offsetX, 0, e.Width, Frame.Height));

            // TODO: Invalidate only dirty zone
            InvalidateVisual();
        }

        public void InvalidateBitmaps()
        {
            if (Frame == null || Frame.Width == 0)
                return;

            //Console.WriteLine("=========> WaveFormControl - InvalidateBitmaps");
            _waveFormEngineService.FlushCache();

            // Start generating all tiles for zoom @ 100%
            //for(int a = 0; a < Frame.Width; a = a + WaveFormCacheService.TileSize)
            //_waveFormCacheService.GetTile(a, Frame.Height, Frame.Width, Zoom);

            InvalidateVisual();
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
                InvalidateVisualInRect(rectCursor);
            }
            catch (Exception ex)
            {
                Console.WriteLine("WaveFormControl - InvalidateMarker - ex: {0}", ex);
            }
        }

        public void SetMarkers(IEnumerable<Marker> markers)
        {
            _markers = markers.ToList();
            InvalidateVisual();
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
            ProcessAutoScroll(marker.PositionBytes);
            InvalidateVisual();
        }

        public void SetLoop(Loop loop)
        {
            _loop = loop;
            InvalidateVisual();
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

            InvalidateVisual();
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
			Console.WriteLine("WaveFormControl - LoadPeakFile - filePath: {0}", audioFile.FilePath);
            _loop = null;
            _markers = null;
            IsLoading = true;
            IsEmpty = false;
            AudioFile = audioFile;
            RefreshStatus("Loading peak file...");
            _waveFormEngineService.LoadPeakFile(audioFile);
        }

        public void CancelPeakFile()
        {
            if (IsLoading)
            {
                Console.WriteLine("WaveFormControl - Canceling peak file generation...");
                IsLoading = false;
                IsEmpty = true;
                _waveFormEngineService.CancelPeakFile();
            }
        }

        public void Reset()
        {
            IsEmpty = true;            
            IsLoading = false;            
            _waveFormEngineService.FlushCache();
            InvalidateVisual();
        }

        private void ProcessAutoScroll(long position)
        {
            if (Zoom <= 1)
                return;

            float positionPercentage = (float)position / (float)Length;
            float cursorX = (positionPercentage * ContentSize.Width);
          //float scrollStartX = ContentOffset.X;
            float scrollCenterX = ContentOffset.X + Frame.Width / 2;
          //float scrollEndX = Bounds.Width + ContentOffset.X;
            //Console.WriteLine("WaveFormScrollView - AutoScroll - positionPct: {0} cursorX: {1} contentOffset.X: {2} waveFormView.Width: {3} scrollStartX: {4} scrollCenterX: {5} scrollEndX: {6}", positionPercentage, cursorX, ContentOffset.X, WaveFormView.Bounds.Width, scrollStartX, scrollCenterX, scrollEndX);

            if (cursorX != scrollCenterX)
            {
                if (cursorX < scrollCenterX)
                {
                    //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - The cursor is left of center X!");
                    if (ContentOffset.X >= 0)
                    {
                        float newContentOffsetX = cursorX - (Frame.Width / 2f);
                        newContentOffsetX = newContentOffsetX < 0 ? 0 : newContentOffsetX;
                        //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - There is space on the left; AUTOCENTER! currentContentOffsetX: {0} newContentOffsetX: {1}", ContentOffset.X, newContentOffsetX);
                        ContentOffset = new BasicPoint(newContentOffsetX, 0);
                    }
                }
                else if(cursorX > scrollCenterX)
                {
                    //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - The cursor is right of center X!");
                    if (ContentOffset.X < ContentSize.Width - Frame.Width)
                    {
                        float newContentOffsetX = cursorX - (Frame.Width / 2f);
                        newContentOffsetX = newContentOffsetX > ContentSize.Width - Frame.Width ? ContentSize.Width - Frame.Width : newContentOffsetX;
                        //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - There is space on the right; AUTOCENTER! currentContentOffsetX: {0} newContentOffsetX: {1}", ContentOffset.X, newContentOffsetX);
                        ContentOffset = new BasicPoint(newContentOffsetX, 0);
                    }
                }
            }
        }

        private void RefreshStatus(string status)
        {
			//Console.WriteLine("WaveFormControl - RefreshStatus - status: {0}", status);            
            _status = status;
            InvalidateVisual();
        }

        private void DrawStatus(IGraphicsContext context, string status)
        {
            //Console.WriteLine("WaveFormControl - DrawStatus - status: {0}", status);
            context.DrawRectangle(Frame, new BasicBrush(_backgroundColor), new BasicPen());            
			var rectText = context.MeasureText(status, new BasicRectangle(0, 0, Frame.Width, 30), FontFace, FontSize);                
            float x = (context.BoundsWidth - rectText.Width) / 2;
            float y = (context.BoundsHeight / 2) - (WaveFormScaleControl.ControlHeight / 2);
            context.DrawText(status, new BasicPoint(x, y), _textColor, FontFace, FontSize);
        }

        private void DrawWaveFormBitmap(IGraphicsContext context)
        {          
            // The scroll bar slowly appears from zoom 100% to 200%. 
            // This enables a smoother effect when zooming, especially with touch input.
            const int tileSize = WaveFormEngineService.TileSize;
            int scrollBarHeight = (int)(Zoom <= 2 ? ((Zoom - 1) * ScrollBarHeight) : ScrollBarHeight);
            int heightAvailable = (int)Frame.Height;

            // Calculate position
            float positionPercentage = (float)Position / (float)Length;
            _cursorX = (positionPercentage * ContentSize.Width) - ContentOffset.X;
            float scrollBarCursorX = positionPercentage * Frame.Width;
            float cursorHeight = heightAvailable - scrollBarHeight;

            DrawTiles(context, tileSize);
            //context.DrawRectangle(context.DirtyRect, new BasicBrush(new BasicColor(255, 255, 0, 100)), _penTransparent); // for debugging dirty rects
            DrawMarkers(context, cursorHeight);
            DrawLoop(context, cursorHeight);
            DrawCursors(context, heightAvailable, cursorHeight, scrollBarCursorX);
            DrawScrollBar(context, scrollBarHeight);
        }

        private void DrawTiles(IGraphicsContext context, int tileSize)
        {
            //Console.WriteLine("-------------- DrawTiles -----------");
            var tiles = _waveFormEngineService.GetTiles(ContentOffset.X, Zoom, Frame, context.DirtyRect, _displayType);

            //Console.WriteLine("WaveFormControl - GetTiles - startTile: {0} startTileX: {1} contentOffset.X: {2} contentOffset.X/tileSize: {3} numberOfDirtyTilesToDraw: {4} firstTileX: {5}", request.StartTile, request.StartTile * tileSize, ContentOffset.X, ContentOffset.X / tileSize, numberOfDirtyTilesToDraw, (request.StartTile * tileSize) - ContentOffset.X);
            foreach (var tile in tiles)
            {
                float tileDeltaZoom = Zoom / tile.Zoom;
                float x = tile.ContentOffset.X * tileDeltaZoom;
                float tileWidth = tileSize * tileDeltaZoom;

                //Console.WriteLine("WaveFormControl - Draw - tile x: {0} tileWidth: {1} tileDeltaZoom: {2} tile.Zoom: {3}", x, tileWidth, tileDeltaZoom, tile.Zoom);
                context.DrawImage(new BasicRectangle(x - ContentOffset.X, 0, tileWidth, Frame.Height), tile.Image.ImageSize, tile.Image.Image);

//                // Debug overlay
//                context.DrawRectangle(new BasicRectangle(x - ContentOffset.X, 0, tileWidth, Frame.Height), new BasicBrush(new BasicColor(0, 0, 255, 50)), _penCursorLine);
//
//                string debugText = string.Format("{0:0.0}", tile.Zoom);
//                context.DrawText(debugText, new BasicPoint(x - ContentOffset.X + 2, 2), _textColor, "Roboto", 11);
//
//                string debugText2 = string.Format("{0:0.0}", tile.ContentOffset.X);
//                context.DrawText(debugText2, new BasicPoint(x - ContentOffset.X + 2, 16), _textColor, "Roboto", 10);
            }
        }

        private void DrawScrollBar(IGraphicsContext context, float realScrollBarHeight)
        {
            if (ShowScrollBar && Zoom > 1)
            {
                byte startAlpha = 0;
                byte maxAlpha = 100;
                byte alpha = (byte)Math.Min(maxAlpha, (startAlpha + (15 * Zoom)));
                var colorThumb = new BasicColor(182, 198, 209, alpha);

                float visibleAreaWidth = (1 / Zoom) * Frame.Width;
                float visibleAreaX = (1 / Zoom) * ContentOffset.X;
                var rectThumb = new BasicRectangle(visibleAreaX, Frame.Height - realScrollBarHeight, visibleAreaWidth, realScrollBarHeight);
                context.DrawRectangle(rectThumb, new BasicBrush(colorThumb), new BasicPen());
            }
        }

        private void DrawMarkers(IGraphicsContext context, float cursorHeight)
        {
            if (_markers == null)
                return;

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

        private void DrawLoop(IGraphicsContext context, float cursorHeight)
        {
            if (_loop == null) 
                return;

            var startSegment = _loop.GetStartSegment();
            var endSegment = _loop.GetEndSegment();
            if (startSegment == null || endSegment == null)
                return;

            // Calculate position
            float startPositionPercentage = (float)startSegment.PositionBytes / (float)Length;
            float startX = (startPositionPercentage * ContentSize.Width) - ContentOffset.X;
            float endPositionPercentage = (float)endSegment.PositionBytes / (float)Length;
            float endX = (endPositionPercentage * ContentSize.Width) - ContentOffset.X;

            // Draw loop lines
            context.SetPen(_penLoopLine);
            context.StrokeLine(new BasicPoint(startX, 0), new BasicPoint(startX, cursorHeight));
            context.StrokeLine(new BasicPoint(endX, 0), new BasicPoint(endX, cursorHeight));

            // Draw text
            var rectText = new BasicRectangle(startX, Frame.Height - 14, endX > startX ? endX - startX : 0, 14);
            context.DrawRectangle(rectText, _brushLoopBackground, _penTransparent);
            rectText.X += 2;
            context.DrawText(_loop.Name, rectText, _textColor, LetterFontFace, LetterFontSize);

            // Draw overlay
            var overlayLeft = new BasicRectangle(0, 0, startX >= 0 ? startX : 0, Frame.Height);
            var overlayRight = new BasicRectangle(endX, 0, Frame.Width, Frame.Height);
            var brushOverlay = new BasicBrush(new BasicColor(0, 0, 0, 130));
            context.DrawRectangle(overlayLeft, brushOverlay, _penTransparent);
            context.DrawRectangle(overlayRight, brushOverlay, _penTransparent);
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

        public override void Render(IGraphicsContext context)
        {
            base.Render(context);

            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
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
            _mouseDownX = x;
            _mouseDownY = y;
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
                    _thumbMouseDownX = visibleAreaX;
                    //Console.WriteLine("Dragging thumb - _thumbMouseDownX: {0}", _thumbMouseDownX);
                }
            }
            else if (_segmentMouseOver != null)
            {
                _isDraggingSegment = true;
                _segmentDragging = _segmentMouseOver;
                _mouseDownScrubbingX = x;
            }
            else
            {
                // Wave form area
                ShowSecondaryPosition = true;
                float positionPercentage = (ContentOffset.X + x) / ContentSize.Width;
                long position = (long)(positionPercentage * Length);
                //Console.WriteLine("positionPercentage: {0} x: {1} ContentSize.Width: {2}", positionPercentage, x, ContentSize.Width);
                SecondaryPosition = position;
            }
        }

        public void MouseUp(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            _isMouseDown = false;
            if (AudioFile == null)
                return;

            long position = (long)(((x + ContentOffset.X) / ContentSize.Width) * Length);
            float positionPercentage = (float)position / (float)Length;

            if (_isDraggingScrollBar)
            {
                _isDraggingThumb = false;
                _isDraggingScrollBar = false;
            } 
            else if (_isDraggingSegment)
            {
                _isDraggingSegment = false;
                SetSegmentPosition(position, positionPercentage, true);
                _segmentDragging = null;
            }
            else
            {
                ShowSecondaryPosition = false;
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
                // Change position
                float positionPercentage = (x + ContentOffset.X) / ContentSize.Width;
                long position = (long)(positionPercentage * Length);

                if (_isDraggingScrollBar)
                {
                    // Change scrollbar thumb position
                    if (_isDraggingThumb)
                    {
                        float visibleAreaWidth = (1 / Zoom) * Frame.Width;
                        float trackWidth = Frame.Width - visibleAreaWidth;
                        float newThumbX = (x - _mouseDownX) + _thumbMouseDownX;
                        float scrollWidth = (Frame.Width * Zoom) - Frame.Width;
                        float newThumbRatio = newThumbX / trackWidth;
                        float newContentOffsetX = newThumbRatio * scrollWidth;
                        OnContentOffsetChanged(new BasicPoint(newContentOffsetX, 0));
                        //Console.WriteLine("ContentOffset change to {0} - _thumbMouseDownX: {1}", newContentOffsetX, _thumbMouseDownX);
                    }
                }
                else if (_isDraggingSegment)
                {
                    // Identify scrubbing speed
                    float deltaY = y - _mouseDownY;
                    var scrubbingSpeed = ScrubbingSpeedHelper.IdentifyScrubbingSpeed(deltaY, _scrubbingSpeeds);
                    if (_currentScrubbingSpeed != scrubbingSpeed)
                    {
                        _currentScrubbingSpeed = scrubbingSpeed;
                        ScrubbingSpeedChanged(_currentScrubbingSpeed);

                        // Set a new reference when changing scrubbing speed
                        _mouseDownScrubbingValue = positionPercentage;
                        _mouseDownScrubbingX = x;
                    }

                    var rect = GetCurrentLoopRect();
                    SetSegmentPosition(position, positionPercentage, false);

                    // Merge with new rect to make a dirty zone to update
                    rect.Merge(GetCurrentLoopRect());
                    InvalidateVisualInRect(rect);
                }
                else
                {
                    SecondaryPosition = position;
                    OnChangeSecondaryPosition(positionPercentage);
                    Console.WriteLine("positionPercentage: {0} x: {1} (x / Zoom): {2} ContentOffset.X: {3} ContentSize.Width: {4}", positionPercentage, x, x / Zoom, ContentOffset.X, ContentSize.Width);
                }
            }
            else
            {
                var cursorType = MouseCursorType.Default;                
                int mouseX = (int) Math.Floor(x);
                if (_loop != null)
                {
                    for (int a = 0; a < _loop.Segments.Count; a++)
                    {
                        float segmentPositionPercentage = (float)_loop.Segments[a].PositionBytes / (float)Length;
                        int segmentX = (int)Math.Floor((segmentPositionPercentage * ContentSize.Width) - ContentOffset.X);

                        // Three pixels wide selection range to make it easier
                        if (segmentX >= mouseX - 1 && segmentX <= mouseX + 1)
                        {
                            _segmentMouseOver = _loop.Segments[a];
                            cursorType = MouseCursorType.VSplit;
                        }
                    }
                }

                if (cursorType == MouseCursorType.Default)
                    _segmentMouseOver = null;

                ChangeMouseCursor(cursorType);
            }
        }

        private void SetSegmentPosition(long position, float positionPercentage, bool mouseUp)
        {
            var startSegment = _loop.GetStartSegment();
            var endSegment = _loop.GetEndSegment();

            // Make sure the loop length doesn't get below 0
            if (_segmentDragging == startSegment && position > endSegment.PositionBytes)
            {
                position = endSegment.PositionBytes;
                positionPercentage = (float) endSegment.PositionBytes/(float) Length;
            }
            else if (_segmentDragging == endSegment && position < startSegment.PositionBytes)
            {
                position = startSegment.PositionBytes;
                positionPercentage = (float) startSegment.PositionBytes/(float) Length;
            }

            //Console.WriteLine("WaveFormControl - MouseMove - position: {0} startSegment.positionbyttes: {1} endSegment.positionbytes: {2}", position, startSegment.PositionBytes, endSegment.PositionBytes);
            _segmentDragging.PositionBytes = position;

            if(mouseUp)
                OnChangedSegmentPosition(_segmentDragging, positionPercentage);
            else
                OnChangingSegmentPosition(_segmentDragging, positionPercentage);
        }

        public void MouseLeave()
        {
            ChangeMouseCursor(MouseCursorType.Default);
        }

        public void MouseEnter()
        {
        }

        public void MouseWheel(float delta)
        {
        }        

        private void ChangeMouseCursor(MouseCursorType cursorType)
        {
            if (_cursorType == cursorType)
                return;

            _cursorType = cursorType;
            OnChangeMouseCursorType(cursorType);
        }

        private BasicRectangle GetCurrentLoopRect()
        {
            var rect = new BasicRectangle();
            var startSegment = _loop.GetStartSegment();
            var endSegment = _loop.GetEndSegment();

            if(startSegment == null || endSegment == null)
                return rect;

            // Zoom...
            float startPct = startSegment.PositionBytes/(float) Length;
            float endPct = endSegment.PositionBytes/(float) Length;
            float x = (startPct * Frame.Width * Zoom) - ContentOffset.X;
            float width = endPct * Frame.Width * Zoom;
            return new BasicRectangle(x, 0, width, Frame.Height);
        }

        public enum InputInteractionMode
        {
            Select = 0, ZoomIn = 1, ZoomOut = 2
        }
    }
}