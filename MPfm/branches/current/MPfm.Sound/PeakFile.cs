//
// PeakFile.cs: This class contains methods to generate and load peak files.
//
// Copyright © 2011 Yanick Castonguay
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MPfm.Sound.BassNetWrapper;

namespace MPfm.Sound
{
    public delegate void GeneratePeakFileProgressChangedEventHandler(GeneratePeakFileProgressChangedEventArgs e);
    public delegate void GeneratePeakFileCompletedEventHandler(object sender, GeneratePeakFileCompletedEventArgs e);

    public class PeakFile
    {
        private SendOrPostCallback onProgressReportDelegate;
        private SendOrPostCallback onCompletedDelegate;

        public event GeneratePeakFileProgressChangedEventHandler ProgressChanged;
        public event GeneratePeakFileCompletedEventHandler Completed;

        private delegate void GeneratePeakFileWorkerEventHandler(string audioFilePath, string peakFilePath, AsyncOperation asyncOp);

        private HybridDictionary userStateToLifetime = new HybridDictionary();

        public PeakFile()
        {
            InitializeDelegates();
        }

        protected virtual void InitializeDelegates()
        {
            onProgressReportDelegate = new SendOrPostCallback(ReportProgress);
            onCompletedDelegate = new SendOrPostCallback(GeneratePeakFileCompleted);
        }

        private void GeneratePeakFileCompleted(object operationState)
        {
            GeneratePeakFileCompletedEventArgs e = operationState as GeneratePeakFileCompletedEventArgs;
            OnCompleted(e);
        }

        private void ReportProgress(object state)
        {
            GeneratePeakFileProgressChangedEventArgs e = state as GeneratePeakFileProgressChangedEventArgs;
            OnProgressChanged(e);
        }

        protected void OnCompleted(GeneratePeakFileCompletedEventArgs e)
        {
            if (Completed != null)
            {
                Completed(this, e);
            }
        }

        protected void OnProgressChanged(GeneratePeakFileProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(e);
            }
        }


        public void GeneratePeakFileAsync(string audioFilePath, string peakFilePath)
        {

        }

        /// <summary>
        /// Generates a peak file for an audio file.
        /// Note: BASS.NET should be initialized already before calling this method. This uses a decode stream.
        /// </summary>
        /// <param name="audioFilePath">Audio file path</param>
        /// <param name="peakFilePath">Peak file path</param>
        public void GeneratePeakFile(string audioFilePath, string peakFilePath)
        {
            // Declare variables            
            FileStream fileStream = null;
            BinaryWriter binaryWriter = null;
            GZipStream gzipStream = null;
            bool generatePeakFile = false;
            int CHUNKSIZE = 0;
            //uint length = 0;
            int read = 0;
            long bytesRead = 0;
            float[] floatLeft = null;
            float[] floatRight = null;
            byte[] buffer = null;
            IntPtr data = new IntPtr(); // initialized properly later
            WaveDataMinMax minMax = null;

            try
            {                
                // Create a channel for decoding
                Channel channelDecode = Channel.CreateFileStreamForDecoding(audioFilePath, true);

                // Check if peak file exists                
                if (!File.Exists(peakFilePath))
                {
                    // Set flag
                    generatePeakFile = true;

                    // Create peak file
                    fileStream = new FileStream(peakFilePath, FileMode.Create, FileAccess.Write);
                    binaryWriter = new BinaryWriter(fileStream);
                    gzipStream = new GZipStream(fileStream, CompressionMode.Compress);
                }

                // Write file header (30 characters)
                binaryWriter.Write("MPfm Peak File (version# 1.00)");

                // 4096 bytes for 16-bit PCM data
                CHUNKSIZE = 4096;

                //    // Check the bits per sample to determine what chunk size to get                
                //    if (soundFormat.BitsPerSample == 16)
                //    {
                //        // 4096 bytes for 16-bit PCM data
                //        CHUNKSIZE = 4096;

                //        //int blockSize = 3 * channels * (sampleRate / 10);
                //        //CHUNKSIZE = 3 * soundFormat.Channels * (soundFormat.Frequency / 10);
                //    }
                //    else if (soundFormat.BitsPerSample == 24)
                //    {
                //        //CHUNKSIZE = 3 * soundFormat.Channels * (soundFormat.Frequency / 10);
                //        CHUNKSIZE = 3 * soundFormat.Channels * (soundFormat.Frequency / 100);
                //    }

                // Create buffer
                data = Marshal.AllocHGlobal(CHUNKSIZE);
                buffer = new byte[CHUNKSIZE];

                // length of a 20ms window in bytes
                //int length20ms = (int)m_playerV4.MainChannel.Seconds2Bytes2(0.02);   //(int)Bass.BASS_ChannelSeconds2Bytes(channel, 0.02);

                // Loop through file using chunk size
                do
                {
                    // Get data
                    read = channelDecode.GetData(buffer, CHUNKSIZE);

                    // Increment bytes read
                    bytesRead += read;

                    // Create arrays for left and right channel
                    floatLeft = new float[CHUNKSIZE / 2];
                    floatRight = new float[CHUNKSIZE / 2];

                    // Loop through sample data to split channels
                    for (int a = 0; a < CHUNKSIZE; a++)
                    {
                        // Check if left or right channel
                        if (a % 2 == 0)
                        {
                            // Left channel
                            floatLeft[a / 2] = buffer[a];
                        }
                        else
                        {
                            // Left channel
                            floatRight[a / 2] = buffer[a];
                        }
                    }

                    //// Check for cancel
                    //if (m_workerWaveForm.CancellationPending)
                    //{
                    //    return;
                    //}

                    // Calculate min/max
                    minMax = AudioTools.GetMinMaxFromWaveData(floatLeft, floatRight, false);
                    ////WaveDataHistory.Add(minMax);

                    //// Report progress
                    ////m_bytesRead = bytesread;
                    ////m_totalPCMBytes = length;
                    ////m_percentageDone = ((float)bytesread / (float)length) * 100;

                    // Write peak information to hard disk
                    if (generatePeakFile)
                    {
                        // Write peak information
                        binaryWriter.Write((double)minMax.leftMin);
                        binaryWriter.Write((double)minMax.leftMax);
                        binaryWriter.Write((double)minMax.rightMin);
                        binaryWriter.Write((double)minMax.rightMax);
                        binaryWriter.Write((double)minMax.mixMin);
                        binaryWriter.Write((double)minMax.mixMax);
                    }

                    //myDelegate.Invoke()

                    //// Report progress
                    //WorkerWaveFormProgress progress = new WorkerWaveFormProgress();
                    //progress.BytesRead = bytesread;
                    //progress.TotalBytes = length;
                    //progress.PercentageDone = ((float)bytesread / (float)length) * 100;
                    //progress.WaveDataMinMax = minMax;
                    //m_workerWaveForm.ReportProgress(0, progress);                  
                }
                //while (result == FMOD.RESULT.OK && read == CHUNKSIZE);
                while (read == CHUNKSIZE);

                // Free channel
                channelDecode.Free();

                // Set nulls for garbage collection               
                channelDecode = null;
                floatLeft = null;
                floatRight = null;
                buffer = null;
                minMax = null;
            }
            catch (Exception ex)
            {
                // Return exception
                //e.Result = ex;
                throw ex;
            }
            finally
            {
                // Did we have to generate a peak file?
                if (generatePeakFile)
                {
                    // Write closing string
                    binaryWriter.Write("[EOF]");

                    // Close writer and stream
                    gzipStream.Close();
                    binaryWriter.Close();
                    fileStream.Close();

                    // Set nulls
                    gzipStream = null;
                    binaryWriter = null;
                    fileStream = null;
                }

                // TODO: Delete the peak file if the operation was cancelled.
                //       HOWEVER, if the user suddently quits the application this won't be called so
                //       the integrity of the file must be checked before loading file!
            }
        }

        public void ReadPeakFile(string peakFilePath)
        {

        }
    }

    public class GeneratePeakFileProgressChangedEventArgs
        : ProgressChangedEventArgs
    {
        public long Position { get; set; }
        public double Progress { get; set; }

        public GeneratePeakFileProgressChangedEventArgs(int progressPercentage, object userState)
            : base(progressPercentage, userState)
        {
        
        }
    }

    public class GeneratePeakFileCompletedEventArgs 
        : AsyncCompletedEventArgs
    {
        public GeneratePeakFileCompletedEventArgs(Exception e, bool canceled, object state) 
            : base(e, canceled, state)
        {

        }
    }
}

