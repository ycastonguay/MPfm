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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.Sound;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.PeakFiles;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using MPfm.iOS.Managers;
using MPfm.iOS.Managers.Events;
using MPfm.Player.Objects;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmWaveFormScaleView")]
    public class MPfmWaveFormScaleView : UIView
    {
        private CGColor _colorGradient1 = GlobalTheme.BackgroundColor.CGColor;
        private CGColor _colorGradient2 = GlobalTheme.BackgroundColor.CGColor;
        private float _timeScaleHeight = 22f;

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
                SetNeedsDisplay();
            }
        }

        private long _audioFileLength;
        public long AudioFileLength
        {
            get
            {
                return _audioFileLength;
            }
            set
            {
                _audioFileLength = value;
                SetNeedsDisplay();
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

        public MPfmWaveFormScaleView(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public MPfmWaveFormScaleView(RectangleF frame) 
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.BackgroundColor = UIColor.Blue;
        }

        public override void Draw(RectangleF rect)
        {
            // Leave empty! Actual drawing is in DrawLayer
            Console.WriteLine("WaveFormScaleView - DrawLayer");
            var context = UIGraphics.GetCurrentContext();
            CoreGraphicsHelper.FillRect(context, Bounds, _colorGradient1);

            if (_audioFile == null || _audioFileLength == 0)
            {
                Console.WriteLine("WaveFormScaleView - DrawLayer - AudioFile is null or AudioFileLength == 0");
                return;
            }

            Console.WriteLine("WaveFormScaleView - DrawLayer - Drawing scale...");

            // Check which scale to take depending on song length and wave form length
            // The scale doesn't have to fit right at the end, it must only show 'major' positions
            // Scale majors: 1 minute > 30 secs > 10 secs > 5 secs > 1 sec
            // 10 'ticks' between each major scale; the left, central and right ticks are higher than the others
            long lengthSamples = ConvertAudio.ToPCM(_audioFileLength, (uint)_audioFile.BitsPerSample, _audioFile.AudioChannels);
            long lengthMilliseconds = ConvertAudio.ToMS(lengthSamples, (uint)_audioFile.SampleRate);
            float totalSeconds = (float)lengthMilliseconds / 1000f;
            float totalMinutes = totalSeconds / 60f;

            // Scale down total seconds/minutes
            float totalSecondsScaled = totalSeconds * (100 / _zoom);
            float totalMinutesScaled = totalMinutes * (100 / _zoom);

            // If the song duration is short, use a smaller scale right away
            var scaleType = WaveFormScaleType._1minute;
            if(totalSecondsScaled < 10)
                scaleType = WaveFormScaleType._1second;
            else if(totalSecondsScaled < 30)
                scaleType = WaveFormScaleType._10seconds;
            else if(totalMinutesScaled < 1)
                scaleType = WaveFormScaleType._30seconds;

            Console.WriteLine("WaveFormView - scaleType: {0} totalMinutes: {1} totalSeconds: {2} totalMinutesScaled: {3} totalSecondsScaled: {4}", scaleType.ToString(), totalMinutes, totalSeconds, totalMinutesScaled, totalSecondsScaled);

            // Draw scale borders
            CoreGraphicsHelper.DrawLine(context, new List<PointF>(){ new PointF(0, _timeScaleHeight), new PointF(Bounds.Width, _timeScaleHeight) }, UIColor.DarkGray.CGColor, 1, false, false);
            //CoreGraphicsHelper.DrawLine(context, new List<PointF>(){ new PointF(0, 0), new PointF(0, timeScaleHeight) }, UIColor.DarkGray.CGColor, 1, false, false);
            //CoreGraphicsHelper.DrawLine(context, new List<PointF>(){ new PointF(boundsWaveForm.Width, 0), new PointF(boundsWaveForm.Width, timeScaleHeight) }, UIColor.Gray.CGColor, 1, false, false);

            // TODO: Maybe reduce the number of ticks between major ticks if the width between ticks is too low.

            float minuteWidth = 0;
            int tickCount = 0;
            switch(scaleType)
            {
                case WaveFormScaleType._1minute:
                    minuteWidth = Bounds.Width / totalMinutes;
                    int majorTickCount = (int)Math.Floor(totalMinutes) + 1; // +1 because of minute 0

                    // Calculate how many minor/major ticks fit in the area showing "full" minutes
                    int minorTickCount = ((int)Math.Floor(totalMinutes)) * 10;

                    // Calculate how many minor ticks are in the last minute; minor tick scale = 6 seconds.
                    float lastMinuteSeconds = totalSeconds - ((float)Math.Floor(totalMinutes) * 60);
                    int lastMinuteTickCount = (int)Math.Floor(lastMinuteSeconds / 6f);
                    tickCount = minorTickCount + lastMinuteTickCount + 1; // +1 because of line at 0:00.000
                    Console.WriteLine("WaveFormView - Scale - majorTickCount: {0} minorTickCount: {1} lastMinuteSeconds: {2} lastMinuteTickCount: {3} tickCount: {4}", majorTickCount, minorTickCount, lastMinuteSeconds, lastMinuteTickCount, tickCount);
                    break;
            }

            float tickX = 0;
            int minute = 0;
            for(int a = 0; a < tickCount; a++)
            {
                bool isMajorTick = ((a % 10) == 0);
                //Console.WriteLine("####> WaveFormView - Scale - tick {0} x: {1} isMajorTick: {2} tickCount: {3}", a, tickX, isMajorTick, tickCount);

                // Draw scale line
                if(isMajorTick)
                    CoreGraphicsHelper.DrawLine(context, new List<PointF>(){ new PointF(tickX, _timeScaleHeight - (_timeScaleHeight / 1.25f)), new PointF(tickX, _timeScaleHeight) }, UIColor.LightGray.CGColor, 1, false, false);
                else
                    CoreGraphicsHelper.DrawLine(context, new List<PointF>(){ new PointF(tickX, _timeScaleHeight - (_timeScaleHeight / 6)), new PointF(tickX, _timeScaleHeight) }, UIColor.DarkGray.CGColor, 1, false, false);

                if(isMajorTick)
                {
                    // Draw dashed traversal line for major ticks
                    CoreGraphicsHelper.DrawLine(context, new List<PointF>(){ new PointF(tickX, _timeScaleHeight), new PointF(tickX, Bounds.Height) }, UIColor.LightGray.CGColor, 1, false, true);

                    // Draw text at every major tick (minute count)
                    CoreGraphicsHelper.DrawTextInRect(context, new RectangleF(tickX + 4, _timeScaleHeight - (_timeScaleHeight / 1.25f), minuteWidth, _timeScaleHeight / 2), minute.ToString() + ":00", "HelveticaNeue", 10f, UIColor.White.CGColor, UILineBreakMode.TailTruncation, UITextAlignment.Left);
                    minute++;
                }

                tickX += minuteWidth / 10;
            }

        }
    }
}
