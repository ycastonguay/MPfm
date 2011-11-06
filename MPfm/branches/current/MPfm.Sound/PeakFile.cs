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
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MPfm.Sound.BassNetWrapper;
using System.Reactive.Concurrency;

namespace MPfm.Sound
{
    //public delegate void GeneratePeakFileProgressChangedEventHandler(GeneratePeakFileProgressChangedEventArgs e);
    //public delegate void GeneratePeakFileCompletedEventHandler(object sender, GeneratePeakFileCompletedEventArgs e);

    public class PeakFile
    {
        private IDisposable m_subscription = null;
        private IDisposable m_subscription2 = null;
        private IDisposable m_subscription3 = null;
        private IObservable<PeakFileProgressData> m_observable = null;
        private IObservable<PeakFileProgressData> m_observable2 = null;
        private IObservable<PeakFileProgressData> m_observable3 = null;

        public PeakFile()
        {
            m_observable = GeneratePeakFile(@"E:\Mp3\Aphex Twin\Come to Daddy\02 Flim.mp3", @"C:\peak1.peak");
            m_observable2 = GeneratePeakFile(@"E:\Mp3\Aphex Twin\Come to Daddy\05 To Cure A Weakling Child (Contour Regard).mp3", @"C:\peak2.peak");
            m_observable3 = GeneratePeakFile(@"E:\Mp3\Aphex Twin\Come to Daddy\08 IZ-US.mp3", @"C:\peak3.peak");
        }

        /// <summary>
        /// Generates a peak file for an audio file. This method returns an IObservable object for use with Reactive Extensions.
        /// Note: BASS.NET should be initialized already before calling this method. This uses a decode stream.
        /// </summary>
        /// <param name="audioFilePath">Audio file path</param>
        /// <param name="peakFilePath">Peak file path</param>
        /// <returns>Observable object with PeakFileProgressData</returns>
        public IObservable<PeakFileProgressData> GeneratePeakFile(string audioFilePath, string peakFilePath)
        {
            // Declare variables         
            bool cancelled = false;
            FileStream fileStream = null;
            BinaryWriter binaryWriter = null;
            GZipStream gzipStream = null;
            bool generatePeakFile = false;
            int CHUNKSIZE = 0;
            long length = 0;
            int read = 0;
            long bytesRead = 0;
            float[] floatLeft = null;
            float[] floatRight = null;
            byte[] buffer = null;
            IntPtr data = new IntPtr(); // initialized properly later
            WaveDataMinMax minMax = null;

            // Create observable
            IObservable<PeakFileProgressData> observable = Observable.Create<PeakFileProgressData>(o =>
            {
                // Declare cancel token
                var cancel = new CancellationDisposable();

                // Schedule operation in a new thread
                Scheduler.NewThread.Schedule(() =>
                {
                    try
                    {
                        // Create a channel for decoding
                        Channel channelDecode = Channel.CreateFileStreamForDecoding(audioFilePath, true);

                        // Get length
                        length = channelDecode.GetLength();

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

                        // Create buffer
                        data = Marshal.AllocHGlobal(CHUNKSIZE);
                        buffer = new byte[CHUNKSIZE];

                        // Loop through file using chunk size
                        int dataBlockRead = 0;
                        do
                        {
                            // Check for cancel
                            if (cancel.Token.IsCancellationRequested)
                            {
                                // Set flags, exit loop
                                cancelled = true;
                                o.OnCompleted();
                                break;
                            }

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

                            // Calculate min/max
                            minMax = AudioTools.GetMinMaxFromWaveData(floatLeft, floatRight, false);                            

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

                            // Update progress every 20 blocks
                            dataBlockRead += read;
                            if (dataBlockRead >= read * 20)
                            {
                                // Reset flag
                                dataBlockRead = 0;

                                // Report progress
                                PeakFileProgressData progress = new PeakFileProgressData();
                                progress.PercentageDone = ((float)bytesRead / (float)length) * 100;
                                o.OnNext(progress);
                            }
                        }
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
                            // Check if the operation was cancelled
                            if (!cancelled)
                            {
                                // Write closing string
                                binaryWriter.Write("[EOF]");
                            }

                            // Close writer and stream
                            gzipStream.Close();
                            binaryWriter.Close();
                            fileStream.Close();

                            // Set nulls
                            gzipStream = null;
                            binaryWriter = null;
                            fileStream = null;

                            // If the operation was cancelled, delete the files
                            if (cancelled)
                            {
                                // Check if file exists
                                if (File.Exists(peakFilePath))
                                {
                                    // Delete file
                                    File.Delete(peakFilePath);
                                }
                            }
                        }
                    }

                    // Set completed
                    o.OnCompleted();
                });

                return cancel;
            });

            return observable;
        }

        public void Test()
        {
            // Start how many you want. 
            m_subscription = m_observable.Subscribe(i => 
                Console.WriteLine("1: " + i.PercentageDone.ToString()));
            m_subscription2 = m_observable2.Subscribe(i => Console.WriteLine("2: " + i.PercentageDone.ToString()));
            m_subscription3 = m_observable3.Subscribe(i => Console.WriteLine("3: " + i.PercentageDone.ToString()));            
            
            //var o = Observable.CombineLatest()
            //    Observable.Start(() => { Console.WriteLine("stuff"); return "A"; }),
            //    Observable.Start(() => { Console.WriteLine("stuff"); return "B"; })
            //    ).Finally(() => Console.WriteLine("done"));
        }

        public void CancelGenerate()
        {
            if (m_subscription != null)
            {
                m_subscription.Dispose();
                m_subscription2.Dispose();
                m_subscription3.Dispose();
            }
        }

        public void ReadPeakFile(string peakFilePath)
        {

        }
    }

    public class PeakFileProgressData
    {
        public float PercentageDone { get; set; }        
    }
}

