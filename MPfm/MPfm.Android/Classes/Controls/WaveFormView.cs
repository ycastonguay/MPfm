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
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Managers;
using MPfm.Android.Classes.Managers.Events;
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.PeakFiles;

namespace org.sessionsapp.android
{
    public class WaveFormView : SurfaceView
    {
        private bool _isLoading;
        private bool _isGeneratingImageCache;
        private string _status;
        private List<Marker> _markers;
        private Bitmap _imageCache = null;
        private int _cursorX;
        private int _secondaryCursorX;

        public WaveFormDisplayType DisplayType { get; set; }

        private WaveFormCacheManager _waveFormCacheManager;
        public WaveFormCacheManager WaveFormCacheManager
        {
            get
            {
                return _waveFormCacheManager;
            }
        }

        private AudioFile _audioFile = null;
        public AudioFile AudioFile
        {
            get
            {
                return _audioFile;
            }
        }

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
                int density = (int) Resources.DisplayMetrics.Density;
                int x = _cursorX - (5*density);
                Rect rectCursor = new Rect(x, 0, x + (10 * density), Height);
                Invalidate(rectCursor);
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

                // Invalidate cursor
                int density = (int)Resources.DisplayMetrics.Density;
                int x = _secondaryCursorX - (25 * density);
                Rect rectCursor = new Rect(x, 0, x + (50 * density), Height);
                Invalidate(rectCursor);
            }
        }

        private bool _showSecondaryPosition = false;
        public bool ShowSecondaryPosition
        {
            get
            {
                return _showSecondaryPosition;
            }
            set
            {
                _showSecondaryPosition = value;
            }
        }

        private long _length;
        public long Length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
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

        public WaveFormView(Context context) : base(context)
        {
            Initialize();
        }

        public WaveFormView(Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Initialize();
        }

        public WaveFormView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        public WaveFormView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
        }

        private void Initialize()
        {
            //SetBackgroundColor(Color.Violet);
            _waveFormCacheManager = Bootstrapper.GetContainer().Resolve<WaveFormCacheManager>();
            _waveFormCacheManager.GeneratePeakFileBegunEvent += HandleGeneratePeakFileBegunEvent;
            _waveFormCacheManager.GeneratePeakFileProgressEvent += HandleGeneratePeakFileProgressEvent;
            _waveFormCacheManager.GeneratePeakFileEndedEvent += HandleGeneratePeakFileEndedEvent;
            _waveFormCacheManager.LoadedPeakFileSuccessfullyEvent += HandleLoadedPeakFileSuccessfullyEvent;
            _waveFormCacheManager.GenerateWaveFormBitmapBegunEvent += HandleGenerateWaveFormBegunEvent;
            _waveFormCacheManager.GenerateWaveFormBitmapEndedEvent += HandleGenerateWaveFormEndedEvent;

            if (!IsInEditMode)
                SetLayerType(LayerType.Hardware, null); // Use GPU instead of CPU (except in IDE such as Eclipse)
        }

        private void HandleGeneratePeakFileBegunEvent(object sender, GeneratePeakFileEventArgs e)
        {
            Post(() =>
            {
                Console.WriteLine("WaveFormView - HandleGeneratePeakFileBegunEvent");
                RefreshStatus("Generating wave form (0% done)");
            });
        }

        private void HandleGeneratePeakFileProgressEvent(object sender, GeneratePeakFileEventArgs e)
        {
            Post(() =>
            {
                //Console.WriteLine("WaveFormView - HandleGeneratePeakFileProgressEvent  (" + e.PercentageDone.ToString("0") + "% done)");
                RefreshStatus("Generating wave form (" + e.PercentageDone.ToString("0") + "% done)");
            });
        }

        private void HandleGeneratePeakFileEndedEvent(object sender, GeneratePeakFileEventArgs e)
        {
            Post(() =>
            {
                // TO DO: Check if cancelled? This will not fire another LoadPeakFile if the peak file gen was cancelled.
                Console.WriteLine("WaveFormView - HandleGeneratePeakFileEndedEvent - LoadPeakFile Cancelled: " + e.Cancelled.ToString() + " FilePath: " + e.AudioFilePath);
                if (!e.Cancelled)
                    _waveFormCacheManager.LoadPeakFile(new AudioFile(e.AudioFilePath));
            });
        }

        private void HandleLoadedPeakFileSuccessfullyEvent(object sender, LoadPeakFileEventArgs e)
        {
            Post(() =>
            {
                Console.WriteLine("WaveFormView - HandleLoadedPeakFileSuccessfullyEvent");
                GenerateWaveFormBitmap(e.AudioFile, new Rect(0, 0, Width, Height));
            });
        }

        private void HandleGenerateWaveFormBegunEvent(object sender, GenerateWaveFormEventArgs e)
        {

        }

        private void HandleGenerateWaveFormEndedEvent(object sender, GenerateWaveFormEventArgs e)
        {
            Post(() =>
            {
                _isLoading = false;
                _imageCache = e.Image;
                Invalidate();
            });
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

        public void SetMarkers(IEnumerable<Marker> markers)
        {
            _markers = markers.ToList();
            Invalidate();
        }

        public void FlushCache()
        {
            _waveFormCacheManager.FlushCache();

            if (_imageCache != null)
            {
                _imageCache.Dispose();
                _imageCache = null;
            }
        }

        public void SetWaveFormLength(long lengthBytes)
        {
            _length = lengthBytes;
            //WaveFormView.Length = lengthBytes;
            //WaveFormScaleView.AudioFileLength = lengthBytes;
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            //WaveFormScaleView.Hidden = true;
            //UserInteractionEnabled = false;
            //if (ScrollViewMode == WaveFormScrollViewMode.Standard)
            //{
            //    WaveFormView.Frame = new RectangleF(0, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight);
            //    WaveFormScaleView.Frame = new RectangleF(0, 0, Bounds.Width, _scaleHeight);
            //    ContentSize = Bounds.Size;
            //    ContentOffset = new PointF(0, 0);
            //}
            //else if (ScrollViewMode == WaveFormScrollViewMode.SelectPosition)
            //{
            //    WaveFormView.Frame = new RectangleF(Bounds.Width / 2, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight);
            //    WaveFormScaleView.Frame = new RectangleF(Bounds.Width / 2, 0, Bounds.Width, _scaleHeight);
            //    ContentSize = new SizeF(Bounds.Width * 2, Bounds.Height);
            //    ContentOffset = new PointF(0, 0);
            //}
            //WaveFormView.LoadPeakFile(audioFile);
            //WaveFormScaleView.AudioFile = audioFile;

            Console.WriteLine("WaveFormView - LoadPeakFile " + audioFile.FilePath);
            _audioFile = audioFile;
            RefreshStatus("Loading peak file...");
            _waveFormCacheManager.LoadPeakFile(audioFile);
        }

        private void RefreshStatus(string status)
        {
            _isLoading = true;
            _status = status;
            Invalidate();
        }

        private void DrawStatus(Canvas canvas, string status)
        {
            float density = Resources.DisplayMetrics.Density;

            // Draw background
            var paintBackground = new Paint();
            paintBackground.Color = Resources.GetColor(MPfm.Android.Resource.Color.background);
            paintBackground.SetStyle(Paint.Style.Fill);
            canvas.DrawRect(new Rect(0, 0, Width, Height), paintBackground);

            // Draw status text
            var paintText = new Paint
            {
                AntiAlias = true,
                Color = Color.White,                
                TextSize = 12 * density                
            };
            Rect rectText = new Rect();
            paintText.GetTextBounds(status, 0, status.Length, rectText);
            int x = (Width - rectText.Width()) / 2;
            int y = Height / 2;
            canvas.DrawText(status, x, y, paintText);
        }

        private void DrawWaveFormBitmap(Canvas canvas)
        {
            _isLoading = false;
            float density = Resources.DisplayMetrics.Density;
            int heightAvailable = Height;

            // Draw bitmap cache
            var paintBitmap = new Paint();
            canvas.DrawBitmap(_imageCache, 0, 0, paintBitmap);

            // Calculate position
            float positionPercentage = (float)Position / (float)Length;
            _cursorX = (int) (positionPercentage * Width);

            // Draw markers
            for (int a = 0; a < _markers.Count; a++)
            {
                float xPct = (float)_markers[a].PositionBytes / (float)Length;
                int x = (int) (xPct * Width);

                // Draw cursor line
                var paintMarkerCursor = new Paint
                {
                    AntiAlias = true,
                    Color = new Color(255, 0, 0),
                    StrokeWidth = 1 * density
                };
                paintMarkerCursor.SetStyle(Paint.Style.Fill);
                canvas.DrawLine(x, 0, x, heightAvailable, paintMarkerCursor);

                // Draw text
                //var rectText = new RectangleF(x, 0, 12, 12);
                //CoreGraphicsHelper.FillRect(context, rectText, new CGColor(1, 0, 0, 0.7f));
                string letter = Conversion.IndexToLetter(a).ToString();
                //UIGraphics.PushContext(context);
                //CoreGraphicsHelper.DrawTextAtPoint(context, new PointF(x + 2, 0), letter, "HelveticaNeue", 10, new CGColor(1, 1, 1, 1));
                //UIGraphics.PopContext();

                int flagWidth = (int) (12*density);
                Rect rectTextBackground = new Rect(x - flagWidth, 0, x, flagWidth);
                var paintTextBackground = new Paint
                {
                    AntiAlias = true,
                    Color = new Color(255, 0, 0, 180)
                };
                paintTextBackground.SetStyle(Paint.Style.Fill);
                canvas.DrawRect(rectTextBackground, paintTextBackground);

                //paintText.GetTextBounds(letter, 0, letter.Length, rectText);
                //int newX = (12 - rectText.Width()) / 2;
                var paintText = new Paint
                {
                    AntiAlias = true,
                    Color = Color.White,
                    TextSize = 10 * density
                };
                canvas.DrawText(letter, x - ((flagWidth * 3) / 4), (flagWidth * 3) / 4, paintText); // newX, Height - rectText.Height() - 4 - 1, paintText);
            }

            // Draw cursor line
            var paintCursor = new Paint
            {
                AntiAlias = true,
                Color = new Color(0, 125, 255),
                StrokeWidth = 1 * density
            };
            paintCursor.SetStyle(Paint.Style.Fill);
            canvas.DrawLine(_cursorX, 0, _cursorX, heightAvailable, paintCursor);

            // Check if a secondary cursor must be drawn (i.e. when changing position)
            if (_showSecondaryPosition)
            {
                float secondaryPositionPercentage = (float)SecondaryPosition / (float)Length;
                _secondaryCursorX = (int) (secondaryPositionPercentage * Width);
                //_secondaryCursorX = (float)Math.Round(_secondaryCursorX * 2) / 2; // Round to 0.5

                paintCursor.Color = Color.White;
                canvas.DrawLine(_secondaryCursorX, 0, _secondaryCursorX, heightAvailable, paintCursor);
            }
        }

        public void RefreshWaveFormBitmap(int width)
        {
            RefreshStatus("Generating new bitmap...");
            GenerateWaveFormBitmap(_audioFile, new Rect(0, 0, width, Height));
        }

        private void GenerateWaveFormBitmap(AudioFile audioFile, Rect frame)
        {
            if (!_isGeneratingImageCache)
            {
                _isGeneratingImageCache = true;
                Console.WriteLine("WaveFormView - GenerateWaveFormBitmap audioFilePath: {0}", audioFile.FilePath);
                _waveFormCacheManager.RequestBitmap(audioFile, WaveFormDisplayType.Stereo, frame, 1, _length);
                _isGeneratingImageCache = false;
            }
        }

        public override void Draw(Canvas canvas)
        {
            //Console.WriteLine("WaveFormView - Draw");
            //base.Draw(canvas);

            if (_isLoading)
            {
                DrawStatus(canvas, _status);
            }
            else if (_imageCache != null)
            {
                DrawWaveFormBitmap(canvas);
            }
            else
            {
                //CoreGraphicsHelper.FillRect(context, Bounds, _colorGradient1);
            }
        }
    }

    /// <summary>
    /// Defines the wave form display type.
    /// </summary>
    public enum WaveFormDisplayType
    {
        /// <summary>
        /// Left channel.
        /// </summary>
        LeftChannel = 0,
        /// <summary>
        /// Right channel.
        /// </summary>
        RightChannel = 1,
        /// <summary>
        /// Stereo (left and right channels).
        /// </summary>
        Stereo = 2,
        /// <summary>
        /// Mix (mix of left/right channels).
        /// </summary>
        Mix = 3
    }
}