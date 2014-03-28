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
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Graphics;
using MPfm.GenericControls.Interaction;
using MPfm.Sound.AudioFiles;

namespace MPfm.GenericControls.Controls
{
    /// <summary>
    /// The WaveFormScale control displays the scale in minutes/seconds on top of the wave form.
    /// </summary>
    public class WaveFormScaleControl : IControl
    {
        private readonly object _locker = new object();
        private BasicBrush _brushBackground;
        private BasicRectangle _rectText;
		private BasicColor _backgroundColor = new BasicColor(32, 40, 46);
		private BasicColor _borderColor = new BasicColor(85, 85, 85);
		private BasicColor _minorTickColor = new BasicColor(85, 85, 85);
		private BasicColor _majorTickColor = new BasicColor(170, 170, 170);
        private BasicColor _textColor = new BasicColor(255, 255, 255);

        private float _lastWidth;
        private float _scaleMultiplier;
        private float _tickWidth;
        private int _tickCount;
        private long _audioFileLength;
        private WaveFormScaleType _scaleType;
        private BasicPen _penTransparent;
        private BasicPen _penBorder;

        public BasicRectangle Frame { get; set; }
		public float FontSize { get; set; }
		public string FontFace { get; set; }

        private AudioFile _audioFile = null;
        public AudioFile AudioFile
        {
            get
            {
                return _audioFile;
            }
            set
            {
                _audioFile = value;
                OnInvalidateVisual();
            }
        }
        
        public long AudioFileLength
        {
            get
            {
                return _audioFileLength;
            }
            set
            {
                _audioFileLength = value;
                OnInvalidateVisual();
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

        public event InvalidateVisual OnInvalidateVisual;
        public event InvalidateVisualInRect OnInvalidateVisualInRect;

        public WaveFormScaleControl()
            : base()
        {
            OnInvalidateVisual += () => { };
            OnInvalidateVisualInRect += (rect) => { };
            Frame = new BasicRectangle();
			FontFace = "Roboto";
			FontSize = 10;
        }

        public void Render(IGraphicsContext context)
        {
            lock (_locker)
            {
                if (_brushBackground == null)
                {
                    _brushBackground = new BasicBrush(_backgroundColor);
                    _penTransparent = new BasicPen();
                    _penBorder = new BasicPen(new BasicBrush(_borderColor), 1);
                    _rectText = context.MeasureText("12345:678.90", new BasicRectangle(0, 0, Frame.Width, Frame.Height), "HelveticaNeue", 10);
                }
            }

            context.DrawRectangle(new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight), _brushBackground, _penTransparent);

            Frame = new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight);
            if (_audioFile == null || _audioFileLength == 0)
                return;

            // Check if scale type needs to be updated
            if (_lastWidth != ContentSize.Width)
            {
                _lastWidth = ContentSize.Width;

                // Check which scale to take depending on song length and wave form length
                // The scale doesn't have to fit right at the end, it must only show 'major' positions
                // Scale majors: 1 minute > 30 secs > 10 secs > 5 secs > 1 sec
                // 10 'ticks' between each major scale; the left, central and right ticks are higher than the others
                long lengthSamples = ConvertAudio.ToPCM(_audioFileLength, (uint)_audioFile.BitsPerSample, _audioFile.AudioChannels);
                long lengthMilliseconds = ConvertAudio.ToMS(lengthSamples, (uint)_audioFile.SampleRate);
                float totalSeconds = (float)lengthMilliseconds / 1000f;
                float totalMinutes = totalSeconds / 60f;

                // Scale down total seconds/minutes
                float totalSecondsScaled = totalSeconds * 100;
                float totalMinutesScaled = totalMinutes * 100;

                // If the song duration is short, use a smaller scale right away
                _scaleType = WaveFormScaleType._1minute;
                if (totalSecondsScaled < 10)
                    _scaleType = WaveFormScaleType._1second;
                else if (totalSecondsScaled < 30)
                    _scaleType = WaveFormScaleType._10seconds;
                else if (totalMinutesScaled < 1)
                    _scaleType = WaveFormScaleType._30seconds;

                //float tickWidth = 0;
                //int tickCount = 0;
                bool foundScale = false;
                int majorTickCount = 0;
                int minorTickCount = 0;
                float lastMinuteSeconds = totalSeconds - ((float)Math.Floor(totalMinutes) * 60);
                int lastMinuteTickCount = 0;
                //float scaleMultiplier = 1;
                while (!foundScale)
                {
                    switch (_scaleType)
                    {
                        case WaveFormScaleType._10minutes:
                            _scaleMultiplier = 1f / 10f;
                            break;
                        case WaveFormScaleType._5minutes:
                            _scaleMultiplier = 1f / 5f;
                            break;
                        case WaveFormScaleType._2minutes:
                            _scaleMultiplier = 1f / 2f;
                            break;
                        case WaveFormScaleType._1minute:
                            _scaleMultiplier = 1f;
                            break;
                        case WaveFormScaleType._30seconds:
                            _scaleMultiplier = 2f;
                            break;
                        case WaveFormScaleType._10seconds:
                            _scaleMultiplier = 6f;
                            break;
                        case WaveFormScaleType._5seconds:
                            _scaleMultiplier = 12f;
                            break;
                        case WaveFormScaleType._1second:
                            _scaleMultiplier = 60f;
                            break;
                    }

                    _tickWidth = (ContentSize.Width / totalMinutes / _scaleMultiplier) / 10;
                    majorTickCount = (int)(Math.Floor(totalMinutes) * _scaleMultiplier) + 1; // +1 because of minute 0
                    minorTickCount = (int)((Math.Floor(totalMinutes) * 10) * _scaleMultiplier);
                    lastMinuteTickCount = (int)Math.Floor(lastMinuteSeconds / (6f / _scaleMultiplier)); // 6 = 6seconds (60/10) // 12
                    _tickCount = minorTickCount + lastMinuteTickCount + 1; // +1 because of line at 0:00.000
                    //Console.WriteLine("WaveFormScaleView - Scale type: {0} - scaleMultipl52ier: {1} majorTickCount: {2} minorTickCount: {3} totalSeconds: {4} lastMinuteSeconds: {5} lastMinuteTickCount: {6} tickCount: {7} tickWidth: {8}", scaleType.ToString(), scaleMultiplier, majorTickCount, minorTickCount, totalSeconds, lastMinuteSeconds, lastMinuteTickCount, tickCount, tickWidth);

                    // Check if the right scale was found
                    if (_tickWidth > 20f * context.Density)
                    {
                        //Console.WriteLine("WaveFormScaleView - tickWidth: {0} - tickWidth > 20; Moving scale down...", tickWidth);
                        switch (_scaleType)
                        {
                            case WaveFormScaleType._1minute:
                                _scaleType = WaveFormScaleType._30seconds;
                                break;
                            case WaveFormScaleType._30seconds:
                                _scaleType = WaveFormScaleType._10seconds;
                                break;
                            case WaveFormScaleType._10seconds:
                                _scaleType = WaveFormScaleType._5seconds;
                                break;
                            case WaveFormScaleType._5seconds:
                                _scaleType = WaveFormScaleType._1second;
                                break;
                            default:
                                foundScale = true;
                                break;
                        }
                    }
                    else if (_tickWidth < 5f * context.Density)
                    {
                        //Console.WriteLine("WaveFormScaleView - tickWidth: {0} - tickWidth < 5f; Moving scale up...", tickWidth);
                        switch (_scaleType)
                        {
                            case WaveFormScaleType._1minute:
                                _scaleType = WaveFormScaleType._2minutes;
                                break;
                            case WaveFormScaleType._2minutes:
                                _scaleType = WaveFormScaleType._5minutes;
                                break;
                            case WaveFormScaleType._5minutes:
                                _scaleType = WaveFormScaleType._10minutes;
                                break;
                            default:
                                foundScale = true;
                                break;
                        }
                    }
                    else
                    {
                        //Console.WriteLine("WaveFormScaleView - tickWidth: {0} - Found right scale; exiting loop...", tickWidth);
                        foundScale = true;
                    }
                }
            }

            // Draw scale borders
            //Console.WriteLine("WaveFormScaleView - scaleType: {0} totalMinutes: {1} totalSeconds: {2} totalMinutesScaled: {3} totalSecondsScaled: {4}", scaleType.ToString(), totalMinutes, totalSeconds, totalMinutesScaled, totalSecondsScaled);
            context.SetPen(_penBorder);
            context.StrokeLine(new BasicPoint(0, ContentSize.Height), new BasicPoint(ContentSize.Width, ContentSize.Height));

            float tickX = -ContentOffset.X;
            int majorTickIndex = 0;
            for (int a = 0; a < _tickCount; a++)
            {
                // Ignore ticks out of bounds
                bool isMajorTick = ((a % 10) == 0);
                if (tickX >= 0 && tickX <= Frame.Width)
                {
                    //Console.WriteLine("####> WaveFormView - Scale - tick {0} x: {1} isMajorTick: {2} tickCount: {3}", a, tickX, isMajorTick, tickCount);

                    if(isMajorTick)
                        //    //context.DrawLine(new BasicPoint(tickX, context.BoundsHeight - (context.BoundsHeight / 1.25f)), new BasicPoint(tickX, context.BoundsHeight), _penMajorTick);
                        context.StrokeLine(new BasicPoint(tickX, 0), new BasicPoint(tickX, ContentSize.Height));
                    else
                        context.StrokeLine(new BasicPoint(tickX, ContentSize.Height - (ContentSize.Height / 6)), new BasicPoint(tickX, ContentSize.Height));

                    if (isMajorTick)
                    {
                        // Draw dashed traversal line for major ticks
                        context.StrokeLine(new BasicPoint(tickX, ContentSize.Height), new BasicPoint(tickX, ContentSize.Height));

                        // Determine major scale text
                        int minutes = 0;
                        int seconds = 0;
                        switch (_scaleType)
                        {
                            case WaveFormScaleType._10minutes:
                                minutes = majorTickIndex * 10;
                                seconds = 0;
                                break;
                            case WaveFormScaleType._5minutes:
                                minutes = majorTickIndex * 5;
                                seconds = 0;
                                break;
                            case WaveFormScaleType._2minutes:
                                minutes = majorTickIndex * 2;
                                seconds = 0;
                                break;
                            case WaveFormScaleType._1minute:
                                minutes = majorTickIndex;
                                seconds = 0;
                                break;
                            case WaveFormScaleType._30seconds:
                                minutes = (int)Math.Floor(majorTickIndex / _scaleMultiplier);
                                seconds = (majorTickIndex % _scaleMultiplier == 0) ? 0 : 30;
                                break;
                            case WaveFormScaleType._10seconds:
                                minutes = (int)Math.Floor(majorTickIndex / _scaleMultiplier);
                                seconds = ((int)Math.Floor(majorTickIndex % _scaleMultiplier)) * 10;
                                break;
                            case WaveFormScaleType._5seconds:
                                minutes = (int)Math.Floor(majorTickIndex / _scaleMultiplier);
                                seconds = ((int)Math.Floor(majorTickIndex % _scaleMultiplier)) * 5;
                                break;
                            case WaveFormScaleType._1second:
                                minutes = (int)Math.Floor(majorTickIndex / _scaleMultiplier);
                                seconds = (int)Math.Floor(majorTickIndex % _scaleMultiplier);
                                break;
                        }

                        // Draw text at every major tick (minute count)
                        string scaleMajorTitle = string.Format("{0}:{1:00}", minutes, seconds);                    
                        float y = ContentSize.Height - (ContentSize.Height/12f) - _rectText.Height - (0.5f * context.Density);                    
						context.DrawText(scaleMajorTitle, new BasicPoint(tickX + (4 * context.Density), y), _textColor, FontFace, FontSize);
                    }
                }
                
                if(isMajorTick)
                    majorTickIndex++;

                tickX += _tickWidth;
            }
        }

        public enum WaveFormScaleType
        {
            _10minutes = 0,
            _5minutes = 1,
            _2minutes = 2,
            _1minute = 3,
            _30seconds = 4,
            _10seconds = 5,
            _5seconds = 6,
            _1second = 7
        }
    }
}