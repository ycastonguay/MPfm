//
// PeakFile.cs: This class contains methods to generate and load peak files.
//
// Copyright © 2011-2012 Yanick Castonguay
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
//using System.Reactive;
//using System.Reactive.Concurrency;
//using System.Reactive.Disposables;
//using System.Reactive.Linq;
//using System.Reactive.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MPfm.Sound.BassNetWrapper;
using MPfm.Core;

namespace MPfm.Sound
{
    public static class PeakFileReader
    {
        /// <summary>
        /// Defines the current peak file version. Used when reading peak files to make sure
        /// the format is compatible.
        /// </summary>
        public static string Version = "1.00";

        /// <summary>
        /// Reads a peak file and returns a min/max peak list.
        /// </summary>
        /// <param name="peakFilePath">Peak file path</param>
        /// <returns>List of min/max peaks</returns>
        public static List<WaveDataMinMax> ReadPeakFile(string peakFilePath)
        {
            // Declare variables 
            FileStream fileStream = null;
            GZipStream gzipStream = null;
            BinaryReader binaryReader = null;
            List<WaveDataMinMax> listMinMax = new List<WaveDataMinMax>();
            string fileHeader = null;
            long audioFileLength = 0;
            int chunkSize = 0;
            int numberOfBlocks = 0;
            int currentBlock = 0;

            try
            {
                // Create file stream
                fileStream = new FileStream(peakFilePath, FileMode.Open, FileAccess.Read);

                // Open binary reader
                binaryReader = new BinaryReader(fileStream);

                // Create GZip stream
                gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);

                // Read file header (30 characters) 
                // Ex: MPfm Peak File (version# 1.00)                
                fileHeader = new string(binaryReader.ReadChars(31));

                // Extract version and validate
                string version = fileHeader.Substring(fileHeader.Length - 5, 4);
                if (version != Version)
                {
                    throw new PeakFileFormatIncompatibleException("Error: The peak file format is not compatible. Expecting version " + Version + " instead of version " + version + ".", null);
                }

                // Read audio file length
                audioFileLength = binaryReader.ReadInt64();

                // Read chunk size and number of blocks
                chunkSize = binaryReader.ReadInt32();
                numberOfBlocks = binaryReader.ReadInt32();

                // Loop through data
                while (binaryReader.PeekChar() != -1)
                {
                    // Increment block
                    currentBlock++;

                    // Read peak information and add to list
                    WaveDataMinMax peak = new WaveDataMinMax();
                    peak.leftMin = (float)binaryReader.ReadDouble();
                    peak.leftMax = (float)binaryReader.ReadDouble();
                    peak.rightMin = (float)binaryReader.ReadDouble();
                    peak.rightMax = (float)binaryReader.ReadDouble();
                    peak.mixMin = (float)binaryReader.ReadDouble();
                    peak.mixMax = (float)binaryReader.ReadDouble();
                    listMinMax.Add(peak);
                }

                // Validate number of blocks read
                if (currentBlock < numberOfBlocks - 1)
                {
                    throw new PeakFileCorruptedException("Error: The peak file is corrupted (the number of blocks didn't match)!", null);
                }
            }
            catch (Exception ex)
            {
                throw new PeakFileCorruptedException("Error: The peak file is corrupted!", ex);
            }
            finally
            {
                // Close stream and reader
                gzipStream.Close();
                binaryReader.Close();
                fileStream.Close();
            }

            return listMinMax;
        }

        /// <summary>
        /// Generates a peak file.
        /// </summary>        
        /// <param name="audioFilePath">Audio file path</param>
        /// <param name="peakFilePath">Peak file path</param>
        public static void GeneratePeakFile(string audioFilePath, string peakFilePath)
        {
            // Declare variables         
            bool cancelled = false;
            FileStream fileStream = null;
            BinaryWriter binaryWriter = null;
            GZipStream gzipStream = null;
            int chunkSize = 0;
            int currentBlock = 0;
            long audioFileLength = 0;
            int read = 0;
            long bytesRead = 0;
            float[] floatLeft = null;
            float[] floatRight = null;
            //byte[] buffer = null;
            float[] buffer = null;
            IntPtr data = new IntPtr(); // initialized properly later
            WaveDataMinMax minMax = null;
            List<WaveDataMinMax> listMinMaxForProgressData = new List<WaveDataMinMax>();


                    try
                    {
                        // Create a channel for decoding
                        Channel channelDecode = Channel.CreateFileStreamForDecoding(audioFilePath, true);

                        // Get audio file length
                        audioFileLength = channelDecode.GetLength();

                        // Divide it by two since we're using floating point
                        audioFileLength /= 2;

                        // Check if peak file exists
                        if (File.Exists(peakFilePath))
                        {
                            // Delete peak file
                            File.Delete(peakFilePath);
                        }

                        // Create streams and binary writer
                        fileStream = new FileStream(peakFilePath, FileMode.Create, FileAccess.Write);
                        binaryWriter = new BinaryWriter(fileStream);
                        gzipStream = new GZipStream(fileStream, CompressionMode.Compress);

                        // 4096 bytes for 16-bit PCM data
                        chunkSize = 4096;

                        // How many blocks will there be?                        
                        double blocks = Math.Ceiling(((double)audioFileLength / (double)chunkSize) * 2) + 1;

                        // Write file header (30 characters)                       
                        // 123456789012345678901234567890
                        // MPfm Peak File (version# 1.00)
                        string version = "MPfm Peak File (version# " + Version + ")";
                        binaryWriter.Write(version);

                        // Write audio file length
                        binaryWriter.Write(audioFileLength);

                        // Write chunk size and number of blocks
                        binaryWriter.Write((Int32)chunkSize);
                        binaryWriter.Write((Int32)blocks);

                        // Create buffer
                        data = Marshal.AllocHGlobal(chunkSize);
                        //buffer = new byte[chunkSize];
                        buffer = new float[chunkSize];

                        // Report progress (started)
                        PeakFileProgressData dataStarted = new PeakFileProgressData();
                        dataStarted.AudioFilePath = audioFilePath;
                        dataStarted.PeakFilePath = peakFilePath;
                        dataStarted.PercentageDone = 0;
                        //dataStarted.ThreadNumber = threadNumber;
                        dataStarted.Length = audioFileLength;
                        dataStarted.CurrentBlock = 0;
                        dataStarted.TotalBlocks = (Int32)blocks;
                        dataStarted.MinMax = listMinMaxForProgressData;
                        //o.OnNext(dataStarted);

                        // Loop through file using chunk size
                        int dataBlockRead = 0;
                        do
                        {
                            //// Check for cancel
                            //if (cancel.Token.IsCancellationRequested)
                            //{
                            //    // Set flags, exit loop
                            //    cancelled = true;
                            //    o.OnCompleted();
                            //    break;
                            //}

                            // Get data
                            read = channelDecode.GetData(buffer, chunkSize);

                            // Increment bytes read
                            bytesRead += read;

                            // Create arrays for left and right channel
                            floatLeft = new float[chunkSize / 2];
                            floatRight = new float[chunkSize / 2];

                            // Loop through sample data to split channels
                            for (int a = 0; a < chunkSize; a++)
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

                            // Calculate min/max and add it to the min/max list for event progress
                            minMax = AudioTools.GetMinMaxFromWaveData(floatLeft, floatRight, false);
                            listMinMaxForProgressData.Add(minMax);

                            // Write peak information
                            binaryWriter.Write((double)minMax.leftMin);
                            binaryWriter.Write((double)minMax.leftMax);
                            binaryWriter.Write((double)minMax.rightMin);
                            binaryWriter.Write((double)minMax.rightMax);
                            binaryWriter.Write((double)minMax.mixMin);
                            binaryWriter.Write((double)minMax.mixMax);

                            //// Update progress every X blocks (m_progressReportBlockInterval) default = 20
                            //dataBlockRead += read;
                            //if (dataBlockRead >= read * progressReportBlockInterval)
                            //{
                            //    // Reset flag
                            //    dataBlockRead = 0;

                            //    // Report progress
                            //    PeakFileProgressData dataProgress = new PeakFileProgressData();
                            //    dataProgress.AudioFilePath = audioFilePath;
                            //    dataProgress.PeakFilePath = peakFilePath;
                            //    dataProgress.PercentageDone = (((float)bytesRead / (float)audioFileLength) / 2) * 100;
                            //    //dataProgress.ThreadNumber = threadNumber;
                            //    dataProgress.Length = audioFileLength;
                            //    dataProgress.CurrentBlock = currentBlock;
                            //    dataProgress.TotalBlocks = (Int32)blocks;
                            //    dataProgress.MinMax = listMinMaxForProgressData;
                            //    //o.OnNext(dataProgress);

                            //    // Reset min/max list
                            //    //listMinMaxForProgressData.Clear();
                            //    listMinMaxForProgressData = new List<WaveDataMinMax>();
                            //}

                            // Increment current block
                            currentBlock++;
                        }
                        while (read == chunkSize);

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
                                try
                                {
                                    // Delete file
                                    File.Delete(peakFilePath);
                                }
                                catch
                                {
                                    // Just skip this step.
                                    Tracing.Log("Could not delete peak file " + peakFilePath + ".");
                                }
                            }
                        }
                    }


        }
    }
}

