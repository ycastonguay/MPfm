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

#if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.Sound.AudioFiles;

namespace MPfm.Sound.PeakFiles
{
    /// <summary>
    /// This class can generate peak files asynchronously. Use the ProcessData event to get the progress.
    /// </summary>
    public class PeakFileService : IPeakFileService
    {
        Task _currentTask;
        CancellationTokenSource _cancellationTokenSource = null;
        CancellationToken _cancellationToken;

        /// <summary>
        /// Defines the current peak file version. Used when reading peak files to make sure
        /// the format is compatible.
        /// </summary>
        const string _version = "1.00";

        /// <summary>
        /// Indicates if a peak file is generating.
        /// </summary>
        public bool IsLoading { get; private set; }

        /// <summary>
        /// Indicates if the class is currently generating peak files.
        /// </summary>
        public bool IsProcessing { get; private set; }

        /// <summary>
        /// Private value for the ProgressReportBlockInterval property.
        /// </summary>
        private int progressReportBlockInterval = 200;
        /// <summary>
        /// Defines when the OnProgressData event is called; it will be called
        /// every x blocks (where x is ProgressReportBlockInterval). 
        /// The default value is 20.
        /// </summary>
        public int ProgressReportBlockInterval
        {
            get
            {
                return progressReportBlockInterval;
            }
            set
            {
                progressReportBlockInterval = value;
            }
        }

        /// <summary>
        /// Generates a peak file for an audio file.
        /// Note: BASS should be initialized already before calling this method. This uses a decode stream.
        /// </summary>
        /// <param name="audioFilePath">Audio file path</param>
        /// <param name="peakFilePath">Peak file path</param>
        public void GeneratePeakFile(string audioFilePath, string peakFilePath)
        {
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

            IsLoading = true;
            bool processSuccessful = false;
            _currentTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    // Get audio file length and divide it by two since we're using floating point
                    Channel channelDecode = Channel.CreateFileStreamForDecoding(audioFilePath, true);
                    audioFileLength = channelDecode.GetLength();
                    audioFileLength /= 2;

                    // Delete any previous peak file
                    if (File.Exists(peakFilePath))
                        File.Delete(peakFilePath);

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
                    string version = "MPfm Peak File (version# " + _version + ")";
                    binaryWriter.Write(version);

                    // Write audio file length
                    binaryWriter.Write(audioFileLength);
                    binaryWriter.Write((Int32)chunkSize);
                    binaryWriter.Write((Int32)blocks);                      

                    // Create buffer
                    data = Marshal.AllocHGlobal(chunkSize);
                    buffer = new float[chunkSize];

                    // Is an event binded to OnProcessData?
                    if (OnProcessStarted != null)
                    {
                        PeakFileStartedData dataStarted = new PeakFileStartedData(){
                            Length = audioFileLength,
                            TotalBlocks = (Int32)blocks
                        };
                        OnProcessStarted(dataStarted);
                    }

                    // Loop through file using chunk size
                    int dataBlockRead = 0;
                    do
                    {
                        // Check for cancel
                        //Console.WriteLine("PeakFileService - Bytes read: " + bytesRead.ToString());
                        if (_cancellationToken.IsCancellationRequested)
                        {
                            // Set flags, exit loop
                            Console.WriteLine("PeakFileGenerator - Cancelling...");
                            cancelled = true;
                            IsLoading = false;
                            OnProcessDone(new PeakFileDoneData() { 
                                AudioFilePath = audioFilePath,
                                Cancelled = true
                            });
                            break;
                        }

                        // Get data and increment bytes read
                        read = channelDecode.GetData(buffer, chunkSize);
                        bytesRead += read;

                        // Create arrays for left and right channel
                        floatLeft = new float[chunkSize / 2];
                        floatRight = new float[chunkSize / 2];

                        // Loop through sample data to split channels
                        for (int a = 0; a < chunkSize; a++)
                        {
                            // Check if left or right channel
                            if (a % 2 == 0)
                                floatLeft[a / 2] = buffer[a];
                            else
                                floatRight[a / 2] = buffer[a];
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

                        // Update progress every X blocks (m_progressReportBlockInterval) default = 20
                        dataBlockRead += read;
                        if (dataBlockRead >= read * progressReportBlockInterval)
                        {
                            // Reset flag
                            dataBlockRead = 0;

                            // Report progress
                            PeakFileProgressData dataProgress = new PeakFileProgressData();
                            dataProgress.AudioFilePath = audioFilePath;
                            dataProgress.PeakFilePath = peakFilePath;
                            dataProgress.PercentageDone = (((float)bytesRead / (float)audioFileLength) / 2) * 100;
                            dataProgress.Length = audioFileLength;
                            dataProgress.CurrentBlock = currentBlock;
                            dataProgress.TotalBlocks = (Int32)blocks;
                            dataProgress.MinMax = listMinMaxForProgressData;
                            OnProcessData(dataProgress); 

                            // Reset min/max list
                            listMinMaxForProgressData = new List<WaveDataMinMax>();
                        }
                        currentBlock++;
                    } while (read == chunkSize);

                    // Free channel
                    Console.WriteLine("PeakFileService - Freeing channel...");
                    channelDecode.Free();

                    // TODO: This should replace the IsCancelled status since cancelling the task doesn't go end well
                    Console.WriteLine("PeakFileService - Is process successful? bytesRead: " + bytesRead.ToString() + " audioFileLength: " + audioFileLength.ToString());
                    if(bytesRead >= audioFileLength)
                        processSuccessful = true;

                    // Set nulls for garbage collection               
                    channelDecode = null;
                    floatLeft = null;
                    floatRight = null;
                    buffer = null;
                    minMax = null;
                } catch (Exception ex)
                {
                    // Return exception
                    //e.Result = ex;
                    Console.WriteLine("PeakFileService - Error: " + ex.Message);
                    throw ex;
                } finally
                {
                    // Close writer and stream
                    Console.WriteLine("PeakFileService - Closing file stream...");
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
                            } catch
                            {
                                // Just skip this step.
                                Tracing.Log("Could not delete peak file " + peakFilePath + ".");
                            }
                        }
                    }   
                }

                // Set completed
                IsLoading = false;
                Console.WriteLine("PeakFileService - ProcessDone - processSuccessful: " + processSuccessful.ToString() + " filePath: " + audioFilePath);
                OnProcessDone(new PeakFileDoneData() {
                    AudioFilePath = audioFilePath,
                    Cancelled = !processSuccessful
                });
            }, _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        /// <summary>
        /// Cancels the peak file generation.
        /// </summary>
        public void Cancel()
        {
            if (IsLoading)
                if(_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Reads a peak file and returns a min/max peak list.
        /// </summary>
        /// <param name="peakFilePath">Peak file path</param>
        /// <returns>List of min/max peaks</returns>
        public List<WaveDataMinMax> ReadPeakFile(string peakFilePath)
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
                binaryReader = new BinaryReader(fileStream);
                gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);

                // Read file header (30 characters) 
                // Ex: MPfm Peak File (version# 1.00)                
                fileHeader = new string(binaryReader.ReadChars(31));

                // Extract version and validate
                string version = fileHeader.Substring(fileHeader.Length - 5, 4);
                if (version != _version)
                    throw new PeakFileFormatIncompatibleException("Error: The peak file format is not compatible. Expecting version " + _version + " instead of version " + version + ".", null);

                // Read audio file length
                audioFileLength = binaryReader.ReadInt64();
                chunkSize = binaryReader.ReadInt32();
                numberOfBlocks = binaryReader.ReadInt32();

                // Loop through data
                while (binaryReader.PeekChar() != -1)
                {
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
                    throw new PeakFileCorruptedException("Error: The peak file is corrupted (the number of blocks didn't match)!", null);
            }
            catch (Exception ex)
            {
                throw new PeakFileCorruptedException("Error: The peak file is corrupted!", ex);
            }
            finally
            {
                gzipStream.Close();
                binaryReader.Close();
                fileStream.Close();
            }

            return listMinMax;
        }

        /// <summary>
        /// Returns the total size of all files within a directory. Non-recursive.
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Total size (in bytes)</returns>
        public static long CheckDirectorySize(string path)
        {
            // Get list of files
            string[] files = Directory.GetFiles(path, "*.mpfmPeak");

            // Loop through files and calculate total length
            long length = 0;
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                length += fileInfo.Length;
            }

            return length;
        }

        /// <summary>
        /// Deletes all the peak files in a directory.
        /// </summary>
        /// <param name="path">Path</param>        
        public static void DeletePeakFiles(string path)
        {
            string[] files = Directory.GetFiles(path, "*.mpfmPeak");
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
    }
}

#endif