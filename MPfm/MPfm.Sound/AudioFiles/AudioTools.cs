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
using System.IO;
using System.Linq;

namespace MPfm.Sound.AudioFiles
{
    /// <summary>
    /// This static class contains different methods for manipulating audio.
    /// </summary>
    public static class AudioTools
    {
        /// <summary>
        /// Defines the note names from C0 to B9.
        /// To be used with NoteFrequencies.
        /// </summary>
        private static string[] NoteNames =
        {
            "C 0", "C#0", "D 0", "D#0", "E 0", "F 0", "F#0", "G 0", "G#0", "A 0", "A#0", "B 0",  
            "C 1", "C#1", "D 1", "D#1", "E 1", "F 1", "F#1", "G 1", "G#1", "A 1", "A#1", "B 1",  
            "C 2", "C#2", "D 2", "D#2", "E 2", "F 2", "F#2", "G 2", "G#2", "A 2", "A#2", "B 2",  
            "C 3", "C#3", "D 3", "D#3", "E 3", "F 3", "F#3", "G 3", "G#3", "A 3", "A#3", "B 3",  
            "C 4", "C#4", "D 4", "D#4", "E 4", "F 4", "F#4", "G 4", "G#4", "A 4", "A#4", "B 4",  
            "C 5", "C#5", "D 5", "D#5", "E 5", "F 5", "F#5", "G 5", "G#5", "A 5", "A#5", "B 5",  
            "C 6", "C#6", "D 6", "D#6", "E 6", "F 6", "F#6", "G 6", "G#6", "A 6", "A#6", "B 6",  
            "C 7", "C#7", "D 7", "D#7", "E 7", "F 7", "F#7", "G 7", "G#7", "A 7", "A#7", "B 7",  
            "C 8", "C#8", "D 8", "D#8", "E 8", "F 8", "F#8", "G 8", "G#8", "A 8", "A#8", "B 8",  
            "C 9", "C#9", "D 9", "D#9", "E 9", "F 9", "F#9", "G 9", "G#9", "A 9", "A#9", "B 9"
        };

        /// <summary>
        /// Defines the note frequencies from C0 to B9.
        /// To be used with NoteNames.
        /// </summary>
        private static float[] NoteFrequencies =
        {
            16.35f,   17.32f,   18.35f,   19.45f,    20.60f,    21.83f,    23.12f,    24.50f,    25.96f,    27.50f,    29.14f,    30.87f, 
            32.70f,   34.65f,   36.71f,   38.89f,    41.20f,    43.65f,    46.25f,    49.00f,    51.91f,    55.00f,    58.27f,    61.74f, 
            65.41f,   69.30f,   73.42f,   77.78f,    82.41f,    87.31f,    92.50f,    98.00f,   103.83f,   110.00f,   116.54f,   123.47f, 
            130.81f,  138.59f,  146.83f,  155.56f,   164.81f,   174.61f,   185.00f,   196.00f,   207.65f,   220.00f,   233.08f,   246.94f, 
            261.63f,  277.18f,  293.66f,  311.13f,   329.63f,   349.23f,   369.99f,   392.00f,   415.30f,   440.00f,   466.16f,   493.88f, 
            523.25f,  554.37f,  587.33f,  622.25f,   659.26f,   698.46f,   739.99f,   783.99f,   830.61f,   880.00f,   932.33f,   987.77f, 
            1046.50f, 1108.73f, 1174.66f, 1244.51f,  1318.51f,  1396.91f,  1479.98f,  1567.98f,  1661.22f,  1760.00f,  1864.66f,  1975.53f, 
            2093.00f, 2217.46f, 2349.32f, 2489.02f,  2637.02f,  2793.83f,  2959.96f,  3135.96f,  3322.44f,  3520.00f,  3729.31f,  3951.07f, 
            4186.01f, 4434.92f, 4698.64f, 4978.03f,  5274.04f,  5587.65f,  5919.91f,  6271.92f,  6644.87f,  7040.00f,  7458.62f,  7902.13f, 
            8372.01f, 8869.84f, 9397.27f, 9956.06f, 10548.08f, 11175.30f, 11839.82f, 12543.85f, 13289.75f, 14080.00f, 14917.24f, 15804.26f
        };

        #region Output and VU Meter
        
        /// <summary>
        /// This method takes a set of parameters defining time, time variation, value and value target and
        /// "eases" in the output value following an exponential . This is the same "easing" used for example
        /// in jQuery animations. This method is useful when fed a min and max value from wave data and ease in
        /// the output value with time. This creates a "fallout" effect in output meters.
        /// </summary>
        /// <param name="currentTime">Current time</param>
        /// <param name="duration">Total time duration</param>
        /// <param name="beginValue">Original value</param>
        /// <param name="changeInValue">Target value</param>
        /// <returns></returns>
        public static float EaseInExpo(float currentTime, float duration, float beginValue, float changeInValue)
        {
            // Ripped off from jQuery easing plugin: http://plugins.jquery.com/files/jquery.easing.1.2.js.txt
            //
            // t: current time, b: begInnIng value, c: change In value, d: duration
            //
            //easeInExpo: function (x, t, b, c, d) {
            //    return (t==0) ? b : c * Math.pow(2, 10 * (t/d - 1)) + b;
            //},            

            if (currentTime == 0)
            {
                return beginValue;
            }

            return changeInValue * (float)Math.Pow(2, 10 * (currentTime/duration - 1)) + beginValue;
        }

        /// <summary>
        /// Takes a block of wave data from both channels and detects if the volume level reaches or goes past 
        /// the distortion threshold passed in parameter. The distortion threshold is measured in decibels
        /// (ex: 0.0f for 0dB)
        /// </summary>
        /// <param name="waveDataLeft">Raw wave data (left channel)</param>
        /// <param name="waveDataRight">Raw wave data (right channel)</param>
        /// <param name="convertNegativeToPositive">Convert negative values to positive values (ex: true when used for output meters, 
        /// false when used with wave form display controls (since the negative value is used to draw the bottom end of the waveform).</param>
        /// <param name="distortionThreshold">If the volume level reaches or goes past this threshold, the method retuns true.</param>
        /// <returns></returns>
        public static bool CheckForDistortion(float[] waveDataLeft, float[] waveDataRight, bool convertNegativeToPositive, float distortionThreshold)
        {
            // Get min max data
            WaveDataMinMax minMax = AudioTools.GetMinMaxFromWaveData(waveDataLeft, waveDataRight, convertNegativeToPositive);

            // Convert values into decibels
            //float dbLeft = ConvertRawWaveValueToDecibels(minMax.leftMax);
            //float dbRight = ConvertRawWaveValueToDecibels(minMax.rightMax);
            int peakL = (int)Math.Round(32767f * minMax.leftMax) & 0xFFFF;
            int peakR = (int)Math.Round(32767f * minMax.rightMax) & 0xFFFF;

            float dbLeft = 0;
            float dbRight = 0;

            #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
            dbLeft = (float)Base.LevelToDB_16Bit(peakL);
            dbRight = (float)Base.LevelToDB_16Bit(peakR);
            #endif

            // Check if the max peak reaches or goes past the threshold (left channel)
            if (dbLeft >= distortionThreshold)
            {
                return true;
            }
            // Check if the max peak reaches or goes past the threshold (right channel)
            if (dbRight >= distortionThreshold)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method takes a min/max wave data structure to analyse it and return a value that can be used in output/VU
        /// meters. It uses "easing" to create a "fallout" visual effect.
        /// </summary>
        /// <param name="history">Min/max wave data structure to analyse</param>
        /// <param name="bufferSizeToAnalyse">Size of buffer to analyse</param>
        /// <param name="channelType">Channel type to analyse</param>
        /// <returns>Value "eased" in to use with an output meter (volume in dB)</returns>
        public static float GetVUMeterValue(List<WaveDataMinMax> history, int bufferSizeToAnalyse, ChannelType channelType)
        {
            // Need to have position in array too...

            // Example: 100 elements in array for 1000ms (10ms update time)
            //
            // Max peak (-10dB) was found at index 50 (500ms). Min peak (-50dB) is found at index 80 (800ms).
            //
            // 


            // Audio Tools considerations
            //
            // 1) If the max peak was found in the last index of the array, it means there's no min following this value.
            //    Since the default value is 0, it makes the bar automatically disappear. Bug fix: set min to max value.
            //
            // 2) Did we convert the negative min value to a positive value to compare with the max value? i.e. the negative value
            //    might be higher (think of lower bottom of the waveform)  if the min is negative, it will make the bar disappear.
            //    *** CONFIRMED ***

            ///////// 1- Find the max peak in the last 1000ms, and its position in the array

            // 1- Find the *TRUE* max peak in the last 1000ms, and its position in the array
            PeakInfo peakMax = GetMaxdBPeakFromWaveDataMaxHistoryWithInfo(history, bufferSizeToAnalyse, channelType);

            // 2- Find the *TRUE* min peak following the max peak and its position in the array
            PeakInfo peakMin = GetMindBPeakFromWaveDataMaxHistoryWithInfo(history, bufferSizeToAnalyse, channelType, peakMax.position);

            // 3- Find the position in ms for the current time and duration (between max and min)

            // Current time is the max position in the array (multiplied by ten (10ms for each data item))
            float currentTime = peakMax.position * 10;

            // Duration is the delta between the min peak position and the max peak. It is the duration of the easing to do later.
            float duration = peakMin.position - peakMax.position;

            // 4- Use easing to make a fallout effect using an exponential curve between the max and the min peak.
            float easedValue = EaseInExpo(currentTime, duration, peakMax.peak, peakMin.peak);

            // Return value
            return easedValue;            
        }

        #endregion

        #region Get Min/Max Peaks From History

        /// <summary>
        /// Returns the maximum peak of a list of min/max peaks.
        /// The values range from -1.0f to 1.0f.
        /// </summary>
        /// <param name="history">List of min/max peaks</param>
        /// <param name="bufferSizeToAnalyse">Buffer size to analyse</param>
        /// <param name="channelType">Channel type</param>
        /// <returns>PeakInfo data structure (peak + position)</returns>
        public static float GetMaxPeakFromWaveDataMaxHistory(List<WaveDataMinMax> history, int bufferSizeToAnalyse, ChannelType channelType)
        {
            return GetMaxPeakFromWaveDataMaxHistoryWithInfo(history, bufferSizeToAnalyse, channelType).peak;
        }

        /// <summary>
        /// Returns the maximum peak of a list of min/max peaks, including its position in the array.
        /// The values range from -1.0f to 1.0f.
        /// </summary>
        /// <param name="history">List of min/max peaks</param>
        /// <param name="bufferSizeToAnalyse">Buffer size to analyse</param>
        /// <param name="channelType">Channel type</param>
        /// <returns>PeakInfo data structure (peak + position)</returns>
        public static PeakInfo GetMaxPeakFromWaveDataMaxHistoryWithInfo(List<WaveDataMinMax> history, int bufferSizeToAnalyse, ChannelType channelType)
        {
            // Declare variables
            float current = 0.0f;

            // Each history item == 10 ms
            int historySize = history.Count;
            if (historySize > bufferSizeToAnalyse)
            {
                historySize = bufferSizeToAnalyse;
            }

            // Start with a peak of 0
            float peak = 0;
            int indexOf = -1;
            for (int a = 0; a < historySize; a++)
            {
                // Get the right value depending on the channel to analyse
                if (channelType == ChannelType.Left)
                {
                    current = history[a].leftMax;
                }
                else if (channelType == ChannelType.Right)
                {
                    current = history[a].rightMax;
                }
                else if (channelType == ChannelType.Mix)
                {
                    current = history[a].mixMax;
                }

                // Check if peak needs to be updated with higher value
                if (current > peak)
                {
                    peak = current;
                    indexOf = a;
                }
            }

            return new PeakInfo() { peak = peak, position = indexOf };
        }

        /// <summary>
        /// Returns the minimum peak of a list of min/max peaks.
        /// The values range from -1.0f to 1.0f.
        /// </summary>
        /// <param name="history">List of min/max peaks</param>
        /// <param name="bufferSizeToAnalyse">Buffer size to analyse</param>
        /// <param name="channelType">Channel type</param>        
        /// <returns>PeakInfo data structure (peak + position)</returns>
        public static float GetMinPeakFromWaveDataMaxHistory(List<WaveDataMinMax> history, int bufferSizeToAnalyse, ChannelType channelType)
        {
            return GetMinPeakFromWaveDataMaxHistory(history, bufferSizeToAnalyse, channelType, 0);
        }

        /// <summary>
        /// Returns the minimum peak of a list of min/max peaks.
        /// The values range from -1.0f to 1.0f.
        /// </summary>
        /// <param name="history">List of min/max peaks</param>
        /// <param name="bufferSizeToAnalyse">Buffer size to analyse</param>
        /// <param name="channelType">Channel type</param>
        /// <param name="startPosition">Start position</param>
        /// <returns>PeakInfo data structure (peak + position)</returns>
        public static float GetMinPeakFromWaveDataMaxHistory(List<WaveDataMinMax> history, int bufferSizeToAnalyse, ChannelType channelType, int startPosition)
        {
            return GetMinPeakFromWaveDataMaxHistoryWithInfo(history, bufferSizeToAnalyse, channelType, startPosition).peak;
        }

        /// <summary>
        /// Returns the minimum peak of a list of min/max peaks, including its position in the array.
        /// The values range from -1.0f to 1.0f.
        /// </summary>
        /// <param name="history">List of min/max peaks</param>
        /// <param name="bufferSizeToAnalyse">Buffer size to analyse</param>
        /// <param name="channelType">Channel type</param>
        /// <param name="startPosition">Start position</param>
        /// <returns>PeakInfo data structure (peak + position)</returns>
        public static PeakInfo GetMinPeakFromWaveDataMaxHistoryWithInfo(List<WaveDataMinMax> history, int bufferSizeToAnalyse, ChannelType channelType, int startPosition)
        {
            if (startPosition < 0)
            {
                return new PeakInfo();
            }

            // Declare variables
            float current = 0.0f;

            // Each history item == 10 ms
            int historySize = history.Count;
            if (historySize > bufferSizeToAnalyse)
            {
                historySize = bufferSizeToAnalyse;
            }

            // Start with a peak of 0
            float peak = 0;
            int indexOf = -1;
            for (int a = startPosition; a < historySize; a++)
            {
                // Get the right value depending on the channel to analyse
                if (channelType == ChannelType.Left)
                {
                    current = history[a].leftMin;
                }
                else if (channelType == ChannelType.Right)
                {
                    current = history[a].rightMin;
                }
                else if (channelType == ChannelType.Mix)
                {
                    current = history[a].mixMin;
                }

                // Check if peak needs to be updated with higher value
                if (current < peak)
                {
                    peak = current;
                    indexOf = a;
                }
            }

            return new PeakInfo() { peak = peak, position = indexOf };
        }

        #endregion

        #region Get Min/Max dB Peaks From History

        /// <summary>
        /// Returns the maximum peak (in decibels) of a list of min/max peaks.
        /// The values range from -1.0f to 1.0f.
        /// </summary>
        /// <param name="history">List of min/max peaks</param>
        /// <param name="bufferSizeToAnalyse">Buffer size to analyse</param>
        /// <param name="channelType">Channel type</param>        
        /// <returns>PeakInfo data structure (peak + position)</returns>
        public static float GetMaxdBPeakFromWaveDataMaxHistory(List<WaveDataMinMax> history, int bufferSizeToAnalyse, ChannelType channelType)
        {
            return GetMaxdBPeakFromWaveDataMaxHistoryWithInfo(history, bufferSizeToAnalyse, channelType).peak;
        }

        /// <summary>
        /// Returns the maximum peak (in decibels) of a list of min/max peaks, including its position in the array.
        /// The values range from -1.0f to 1.0f.
        /// </summary>
        /// <param name="history">List of min/max peaks</param>
        /// <param name="bufferSizeToAnalyse">Buffer size to analyse</param>
        /// <param name="channelType">Channel type</param>        
        /// <returns>PeakInfo data structure (peak + position)</returns>
        public static PeakInfo GetMaxdBPeakFromWaveDataMaxHistoryWithInfo(List<WaveDataMinMax> history, int bufferSizeToAnalyse, ChannelType channelType)
        {
            // Declare variables
            float dbCurrent = 0.0f;

            // Each history item == 10 ms
            int historySize = history.Count;
            if (historySize > bufferSizeToAnalyse)
            {
                historySize = bufferSizeToAnalyse;
            }

            // Start with a peak of -100 dB
            float peak = -100;
            int indexOf = -1;
            for (int a = 0; a < historySize; a++)
            {
                // Get the right value depending on the channel to analyse
                if (channelType == ChannelType.Left)
                {
                    dbCurrent = ConvertRawWaveValueToDecibels(history[a].leftMax);
                    //dbCurrent = 20.0f * (float)Math.Log10(history[a].leftMax);                    
                }
                else if (channelType == ChannelType.Right)
                {
                    dbCurrent = ConvertRawWaveValueToDecibels(history[a].rightMax);
                    //dbCurrent = 20.0f * (float)Math.Log10(history[a].rightMax);
                }
                else if (channelType == ChannelType.Mix)
                {
                    dbCurrent = ConvertRawWaveValueToDecibels(history[a].mixMax);
                    //dbCurrent = 20.0f * (float)Math.Log10(history[a].mixMax);
                }

                // Check if peak needs to be updated with higher value
                if (dbCurrent > peak)
                {
                    peak = dbCurrent;
                    indexOf = a;
                }
                // Ceiling
                else if (dbCurrent > 10.0f)
                {
                    peak = 10.0f;
                    indexOf = a;
                }
            }

            // Floor
            if (peak < -100.0f)
            {
                peak = -100.0f;
            }

            return new PeakInfo() { peak = peak, position = indexOf };
        }

        /// <summary>
        /// Returns the minimum peak (in decibels) of a list of min/max peaks.
        /// The values range from -1.0f to 1.0f.
        /// </summary>
        /// <param name="history">List of min/max peaks</param>
        /// <param name="bufferSizeToAnalyse">Buffer size to analyse</param>
        /// <param name="channelType">Channel type</param>        
        /// <returns>PeakInfo data structure (peak + position)</returns>
        public static float GetMindBPeakFromWaveDataMaxHistory(List<WaveDataMinMax> history, int bufferSizeToAnalyse, ChannelType channelType)
        {
            return GetMindBPeakFromWaveDataMaxHistory(history, bufferSizeToAnalyse, channelType, 0);
        }

        /// <summary>
        /// Returns the minimum peak (in decibels) of a list of min/max peaks.
        /// The values range from -1.0f to 1.0f.
        /// </summary>
        /// <param name="history">List of min/max peaks</param>
        /// <param name="bufferSizeToAnalyse">Buffer size to analyse</param>
        /// <param name="channelType">Channel type</param>
        /// <param name="startPosition">Start position</param>
        /// <returns>PeakInfo data structure (peak + position)</returns>
        public static float GetMindBPeakFromWaveDataMaxHistory(List<WaveDataMinMax> history, int bufferSizeToAnalyse, ChannelType channelType, int startPosition)
        {
            return GetMindBPeakFromWaveDataMaxHistoryWithInfo(history, bufferSizeToAnalyse, channelType, startPosition).peak;
        }

        /// <summary>
        /// Returns the minimum peak (in decibels) of a list of min/max peaks, including its position in the array.
        /// The values range from -1.0f to 1.0f.
        /// </summary>
        /// <param name="history">List of min/max peaks</param>
        /// <param name="bufferSizeToAnalyse">Buffer size to analyse</param>
        /// <param name="channelType">Channel type</param>
        /// <param name="startPosition">Start position</param>
        /// <returns>PeakInfo data structure (peak + position)</returns>
        public static PeakInfo GetMindBPeakFromWaveDataMaxHistoryWithInfo(List<WaveDataMinMax> history, int bufferSizeToAnalyse, ChannelType channelType, int startPosition)
        {
            if (startPosition < 0)
            {
                return new PeakInfo();
            }

            // Declare variables
            float dbCurrent = 0.0f;

            // Each history item == 10 ms
            int historySize = history.Count;
            if (historySize > bufferSizeToAnalyse)
            {
                historySize = bufferSizeToAnalyse;
            }

            // Start with a peak of 10 dB
            float peak = 10;
            int indexOf = -1;
            for (int a = startPosition; a < historySize; a++)
            {
                // Get the right value depending on the channel to analyse
                if (channelType == ChannelType.Left)
                {
                    dbCurrent = ConvertRawWaveValueToDecibels(history[a].leftMin);
                    //dbCurrent = 20.0f * (float)Math.Log10(history[a].leftMin);
                }
                else if (channelType == ChannelType.Right)
                {
                    dbCurrent = ConvertRawWaveValueToDecibels(history[a].rightMin);
                    //dbCurrent = 20.0f * (float)Math.Log10(history[a].rightMin);
                }
                else if (channelType == ChannelType.Mix)
                {
                    dbCurrent = ConvertRawWaveValueToDecibels(history[a].mixMin);
                    //dbCurrent = 20.0f * (float)Math.Log10(history[a].mixMin);
                }

                // Check if peak needs to be updated with higher value
                if (dbCurrent < peak)
                {
                    peak = dbCurrent;
                    indexOf = a;
                }
                // Ceiling (only if already 10dB)
                else if (dbCurrent >= 10.0f && peak == 10.0f)
                {
                    peak = 10.0f;
                    indexOf = a;
                }
            }

            // Floor
            if (peak < -100.0f)
            {
                peak = -100.0f;
            }

            return new PeakInfo() { peak = peak, position = indexOf };
        }

        #endregion

        /// <summary>
        /// This method takes the left channel and right channel wave raw data and analyses it to get
        /// the maximum and minimum values in the float structure. It returns a data structure named
        /// WaveDataMinMax (see class description for more information). Negative values can be converted to
        /// positive values before min and max comparaison. Set this parameter to true for output meters and
        /// false for wave form display controls.
        /// </summary>
        /// <param name="waveDataLeft">Raw wave data (left channel)</param>
        /// <param name="waveDataRight">Raw wave data (right channel)</param>
        /// <param name="convertNegativeToPositive">Convert negative values to positive values (ex: true when used for output meters, 
        /// false when used with wave form display controls (since the negative value is used to draw the bottom end of the waveform).</param>
        /// <returns>WaveDataMinMax data structure</returns>
        public static WaveDataMinMax GetMinMaxFromWaveData(float[] waveDataLeft, float[] waveDataRight, bool convertNegativeToPositive)
        {
            // Create default data
            WaveDataMinMax data = new WaveDataMinMax();

            // Loop through values to get min/max
            for (int i = 0; i < waveDataLeft.Length; i++)
            {
                // Set values to compare
                float left = waveDataLeft[i];
                float right = waveDataRight[i];

                // Do we have to convert values before comparaison?
                if (convertNegativeToPositive)
                {
                    // Compare values, if negative then remove negative sign
                    if (left < 0)
                    {
                        left = -left;
                    }
                    if (right < 0)
                    {
                        right = -right;
                    }
                }

                // Calculate min/max for left channel
                if (left < data.leftMin)
                {
                    data.leftMin = left;
                }
                if (left > data.leftMax)
                {
                    data.leftMax = left;
                }

                // Calculate min/max for right channel
                if (right < data.rightMin)
                {
                    data.rightMin = right;
                }
                if (right > data.rightMax)
                {
                    data.rightMax = right;
                }

                // Calculate min/max mixing both channels
                if (left < data.mixMin)
                {
                    data.mixMin = left;
                }
                if (right < data.mixMin)
                {
                    data.mixMin = right;
                }
                if (left > data.mixMax)
                {
                    data.mixMax = left;
                }
                if (right > data.mixMax)
                {
                    data.mixMax = right;
                }
            }

            return data;
        }

        /// <summary>
        /// This method converts a raw wave value (range: -1 to 1) to actual decibels.
        /// Decibels are measured using an exponential scale. -100dB is total silence.
        /// -60dB is usually the noise floor. When the signal reaches or goes past 0.0dB
        /// it is considered distortioning. The decibel range returned is -100dB to 10dB.
        /// </summary>
        /// <param name="rawValue">Raw wave value (ranging from -1 to 1)</param>
        /// <returns>Raw value converted into decibels</returns>
        public static float ConvertRawWaveValueToDecibels(float rawValue)
        {
            return 20.0f * (float)Math.Log10(rawValue);
        }

        /// <summary>
        /// Searches recursively for audio files in a folder. The file extensions must be specified
        /// using the extensions parameter. This method is recursive.
        /// </summary>
        /// <param name="folderPath">Main folder path</param>
        /// <param name="extensions">File extensions to search, separated by a semi column (ex: MP3;FLAC;OGG;WAV)</param>
        /// <returns>List of audio file paths</returns>
        public static List<string> SearchAudioFilesRecursive(string folderPath, string extensions)
        {
            // Declare variables
            List<string> arrayFiles = new List<string>();
            List<string> fileExtensions = extensions.Split(';').ToList();
            
            #if PCL || WINDOWSSTORE || WINDOWS_PHONE
            
            // TOOD: Implement this

            #else            

            // Search for MP3 files in the folder recursively
            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(folderPath);

            // Check for sub directories
            foreach (DirectoryInfo directoryInfo in rootDirectoryInfo.GetDirectories())
            {
                // Check for other files in sub directories
                List<string> files = SearchAudioFilesRecursive(directoryInfo.FullName, extensions);
                arrayFiles.AddRange(files);
            }

            // Get files for each type
            foreach (string fileExtension in fileExtensions)
            {
                // Check if the file has a supported extension
                foreach (FileInfo fileInfo in rootDirectoryInfo.GetFiles("*." + fileExtension))
                {
                    // Add file
                    arrayFiles.Add(fileInfo.FullName);
                }
            }

            #endif

            return arrayFiles;
        }
    }

    /// <summary>
    /// Structure of data representing the different min, max of different
    /// audio channels (left, right) and a mix of both channels (mix).
    /// </summary>
    public class WaveDataMinMax
    {
        /// <summary>
        /// Minimum value for the Left channel.
        /// </summary>
        public float leftMin { get; set; }
        /// <summary>
        /// Maximum value for the Left channel.
        /// </summary>
        public float leftMax { get; set; }
        /// <summary>
        /// Minimum value for the Right channel.
        /// </summary>
        public float rightMin { get; set; }
        /// <summary>
        /// Maximum value for the Right channel.
        /// </summary>
        public float rightMax { get; set; }
        /// <summary>
        /// Minimum value for the mixed Left/Right channel.
        /// </summary>
        public float mixMin { get; set; }
        /// <summary>
        /// Maximum value for the mixed Left/Right channel.
        /// </summary>
        public float mixMax { get; set; }

        /// <summary>
        /// Default constructor. Initializes all values to 0.0f.
        /// </summary>
        public WaveDataMinMax()
        {
            // Set default values
            leftMin = 0.0f;
            leftMax = 0.0f;
            rightMin = 0.0f;
            rightMax = 0.0f;
            mixMin = 0.0f;
            mixMax = 0.0f;
        }
    }

    /// <summary>
    /// Peak information (includes its original position in the WaveDataMinMax array).
    /// </summary>
    public class PeakInfo
    {
        /// <summary>
        /// Peak value.
        /// </summary>
        public float peak { get; set; }        
        /// <summary>
        /// Peak position (in the WaveDataMixMax array).
        /// </summary>
        public int position { get; set; }
    }

    /// <summary>
    /// Defines the channel type.
    /// </summary>
    public enum ChannelType
    {
        /// <summary>
        /// Left channel.
        /// </summary>
        Left = 0, 
        /// <summary>
        /// Right channel.
        /// </summary>
        Right = 1, 
        /// <summary>
        /// Mix (left/right channels).
        /// </summary>
        Mix = 2
    }
}
