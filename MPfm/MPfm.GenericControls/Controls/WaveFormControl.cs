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
using System.Collections.Generic;
using System.Linq;
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.PeakFiles;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Graphics;
using MPfm.GenericControls.Managers;
using MPfm.GenericControls.Managers.Events;

namespace MPfm.GenericControls.Controls
{
    /// <summary>
    /// The WaveFormScale control displays the scale in minutes/seconds on top of the wave form.
    /// </summary>
    public class WaveFormControl : IControl
    {
        private Guid _activeMarkerId = Guid.Empty;
        private List<Marker> _markers = new List<Marker>();
        private string _status = "";
        private bool _isLoading = false;
        private bool _isGeneratingImageCache = false;
        private IDisposable _imageCache = null;
        private float _cursorX;
        private float _secondaryCursorX;
		private BasicColor _backgroundColor = new BasicColor(32, 40, 46);
        private BasicColor _statusBackgroundColor = new BasicColor(0, 0, 0, 178);
        private BasicColor _cursorColor = new BasicColor(0, 128, 255);
        private BasicColor _secondaryCursorColor = new BasicColor(255, 255, 255);
        private BasicColor _markerCursorColor = new BasicColor(255, 0, 0);
        private BasicColor _markerSelectedCursorColor = new BasicColor(234, 138, 128);
        private BasicColor _textColor = new BasicColor(255, 255, 255);
        //var color = _markers[a].MarkerId == _activeMarkerId ? GlobalTheme.SecondaryLightColor : new UIColor(1, 0, 0, 1);
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
                if (_isLoading || _imageCache == null)
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
                if (_isLoading)
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
            }
        }

        public WaveFormDisplayType DisplayType { get; set; }
        public WaveFormCacheManager WaveFormCacheManager { get; private set; }
        public AudioFile AudioFile { get; private set; }
        public bool ShowSecondaryPosition { get; set; }
        public long Length { get; set; }

        public event InvalidateVisual OnInvalidateVisual;
        public event InvalidateVisualInRect OnInvalidateVisualInRect;

        public WaveFormControl()
        {
            Initialize();
        }

        private void Initialize()
        {
            OnInvalidateVisual += () => { };
            OnInvalidateVisualInRect += (rect) => { };
			FontSize = 12;
			FontFace = "Roboto";
			LetterFontSize = 10;
			LetterFontFace = "Roboto";
            Frame = new BasicRectangle();
            DisplayType = WaveFormDisplayType.Stereo;
            WaveFormCacheManager = Bootstrapper.GetContainer().Resolve<WaveFormCacheManager>();
            WaveFormCacheManager.GeneratePeakFileBegunEvent += HandleGeneratePeakFileBegunEvent;
            WaveFormCacheManager.GeneratePeakFileProgressEvent += HandleGeneratePeakFileProgressEvent;
            WaveFormCacheManager.GeneratePeakFileEndedEvent += HandleGeneratePeakFileEndedEvent;
            WaveFormCacheManager.LoadedPeakFileSuccessfullyEvent += HandleLoadedPeakFileSuccessfullyEvent;
            WaveFormCacheManager.GenerateWaveFormBitmapBegunEvent += HandleGenerateWaveFormBegunEvent;
            WaveFormCacheManager.GenerateWaveFormBitmapEndedEvent += HandleGenerateWaveFormEndedEvent;
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
                WaveFormCacheManager.LoadPeakFile(new AudioFile(e.AudioFilePath));
        }

        private void HandleLoadedPeakFileSuccessfullyEvent(object sender, LoadPeakFileEventArgs e)
        {
            //InvokeOnMainThread(() =>
			//Console.WriteLine("WaveFormControl - HandleLoadedPeakFileSuccessfullyEvent");
            GenerateWaveFormBitmap(e.AudioFile, Frame);
        }

        private void HandleGenerateWaveFormBegunEvent(object sender, GenerateWaveFormEventArgs e)
        {
        }

        private void HandleGenerateWaveFormEndedEvent(object sender, GenerateWaveFormEventArgs e)
        {
            //InvokeOnMainThread(() =>
			Console.WriteLine("WaveFormControl - GenerateWaveFormEndedEvent");
            _isGeneratingImageCache = false;
            _isLoading = false;
            _imageCache = e.Image;
            OnInvalidateVisual();
        }

        void HandleOnPeakFileProcessStarted(PeakFileStartedData data)
        {
        }

        void HandleOnPeakFileProcessData(PeakFileProgressData data)
        {
        }

        void HandleOnPeakFileProcessDone(PeakFileDoneData data)
        {
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
            WaveFormCacheManager.FlushCache();

            if (_imageCache != null)
            {
                // the class could be abstract and virtual methods has to implement image stuff
                _imageCache.Dispose();
                _imageCache = null;
            }
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
			//Console.WriteLine("WaveFormControl - LoadPeakFile " + audioFile.FilePath);
            AudioFile = audioFile;
            RefreshStatus("Loading peak file...");
            WaveFormCacheManager.LoadPeakFile(audioFile);
        }

        private void RefreshStatus(string status)
        {
			//Console.WriteLine("WaveFormControl - RefreshStatus - status: {0}", status);
            _isLoading = true;
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
			//Console.WriteLine("WaveFormControl - DrawWaveFormBitmap");
            _isLoading = false;
            int heightAvailable = (int)Frame.Height;

            // Draw bitmap cache
            context.DrawImage(Frame, _imageCache);

            // Calculate position
            float positionPercentage = (float)Position / (float)Length;
            _cursorX = positionPercentage * Frame.Width;

            // Draw markers
            for (int a = 0; a < _markers.Count; a++)
            {
                float xPct = (float)_markers[a].PositionBytes / (float)Length;
                float x = xPct * Frame.Width;

                // Draw cursor line
                var color = _markers[a].MarkerId == _activeMarkerId ? _markerSelectedCursorColor : _markerCursorColor;
                context.SetStrokeColor(color);
                context.SetLineWidth(1.0f);
                context.StrokeLine(new BasicPoint(x, 0), new BasicPoint(x, heightAvailable));

                // Draw text
                var rectText = new BasicRectangle(x, 0, 12, 12);
				context.DrawRectangle(rectText, new BasicBrush(_markerCursorColor), new BasicPen());
                string letter = Conversion.IndexToLetter(a).ToString();
				context.DrawText(letter, new BasicPoint(x + 2, 0), _textColor, LetterFontFace, LetterFontSize);
            }

            // Draw cursor line
            context.SetStrokeColor(_cursorColor);
            context.SetLineWidth(1.0f);
            context.StrokeLine(new BasicPoint(_cursorX, 0), new BasicPoint(_cursorX, heightAvailable));

            // Check if a secondary cursor must be drawn (i.e. when changing position)
            if (ShowSecondaryPosition)
            {
                float secondaryPositionPercentage = (float)SecondaryPosition / (float)Length;
                _secondaryCursorX = secondaryPositionPercentage * Frame.Width;
                _secondaryCursorX = (float)Math.Round(_secondaryCursorX * 2) / 2; // Round to 0.5

                // Draw cursor line
                context.SetStrokeColor(_secondaryCursorColor);
                context.SetLineWidth(1.0f);
                context.StrokeLine(new BasicPoint(_secondaryCursorX, 0), new BasicPoint(_secondaryCursorX, heightAvailable));
            }
        }

        public void RefreshWaveFormBitmap()
        {
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
            if (!_isGeneratingImageCache)
            {
                _isGeneratingImageCache = true;
                //Console.WriteLine("MPfmWaveFormView - GenerateWaveFormBitmap audioFilePath: {0}", audioFile.FilePath);
                WaveFormCacheManager.RequestBitmap(audioFile, WaveFormDisplayType.Stereo, rect, 1, Length);
            }
        }

        public void Render(IGraphicsContext context)
        {
			//Console.WriteLine("WaveFormControl - Render");
            Frame = new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight);
            if (_isLoading)
            {
				//Console.WriteLine("WaveFormControl - Render - Drawing status...");
                DrawStatus(context, _status);
            }
            else if (_imageCache != null)
            {
				//Console.WriteLine("WaveFormControl - Render - Drawing wave form bitmap...");
                DrawWaveFormBitmap(context);
            }
            else
            {
				//Console.WriteLine("WaveFormControl - Render - Drawing empty background...");
                context.DrawRectangle(Frame, new BasicBrush(_backgroundColor), new BasicPen());
            }
        }
    }
}