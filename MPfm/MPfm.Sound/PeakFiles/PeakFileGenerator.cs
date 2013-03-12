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
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Bass.Net;

namespace MPfm.Sound
{	
    /// <summary>
    /// This class can generate peak files asynchronously. Use the ProcessData event to get the progress.
    /// </summary>
    public class PeakFileGenerator
    {
        private Task _currentTask;
        private CancellationTokenSource cancellationTokenSource = null;
        private CancellationToken cancellationToken;

        /// <summary>
        /// Indicates if a peak file is generating.
        /// </summary>
        public bool IsLoading { get; private set; }

        /// <summary>
        /// Defines the current peak file version. Used when reading peak files to make sure
        /// the format is compatible.
        /// </summary>
        private string version = "1.00";

        /// <summary>
        /// Delegate for the OnProcessStarted event.
        /// </summary>
        /// <param name="data">Event data</param>
        public delegate void ProcessStarted(PeakFileStartedData data);

        /// <summary>
        /// Event called when a thread starts its work.
        /// </summary>
        public event ProcessStarted OnProcessStarted;

        /// <summary>
        /// Delegate for the OnProcessData event.
        /// </summary>
        /// <param name="data">Event data</param>
        public delegate void ProcessData(PeakFileProgressData data);

        /// <summary>
        /// Event called every 20 blocks when generating a peak file.
        /// </summary>
        public event ProcessData OnProcessData;

        /// <summary>
        /// Delegate for the OnProcessDone event.
        /// </summary>        
        /// <param name="data">Event data</param>
        public delegate void ProcessDone(PeakFileDoneData data);

        /// <summary>
        /// Event called when all the GeneratePeakFiles threads have completed their work.
        /// </summary>
        public event ProcessDone OnProcessDone;

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
        /// Default constructor for the PeakFile class.
        /// </summary>
        public PeakFileGenerator()
        {
        }

        /// <summary>
        /// Generates a peak file for an audio file.
        /// Note: BASS should be initialized already before calling this method. This uses a decode stream.
        /// </summary>
        /// <param name="audioFilePath">Audio file path</param>
        /// <param name="peakFilePath">Peak file path</param>
        public void GeneratePeakFile(string audioFilePath, string peakFilePath)
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

            // Schedule operation in a new thread
            IsLoading = true;
            _currentTask = Task.Factory.StartNew(() =>
            {
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
                    string version = "MPfm Peak File (version# " + this.version + ")";
                    binaryWriter.Write(version);

                    // Write audio file length
                    binaryWriter.Write(audioFileLength);
                    
                    // Write chunk size and number of blocks
                    binaryWriter.Write((Int32)chunkSize);
                    binaryWriter.Write((Int32)blocks);                      

                    // Create buffer
                    data = Marshal.AllocHGlobal(chunkSize);
                    buffer = new float[chunkSize];

                    // Is an event binded to OnProcessData?
                    if (OnProcessStarted != null)
                    {
                        // Report progress (started)
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
                        if (cancellationToken.IsCancellationRequested)
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
                                floatLeft [a / 2] = buffer [a];
                            } else
                            {
                                // Left channel
                                floatRight [a / 2] = buffer [a];
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

                        // Increment current block
                        currentBlock++;
                    } while (read == chunkSize);

                    // Free channel
                    channelDecode.Free();

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
                    throw ex;
                } finally
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
                OnProcessDone(new PeakFileDoneData() {
                    AudioFilePath = audioFilePath,
                    Cancelled = false
                });
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        /// <summary>
        /// Cancels the peak file generation.
        /// </summary>
        public void Cancel()
        {
            if (IsLoading)
                if(cancellationTokenSource != null)
                    cancellationTokenSource.Cancel();
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

                // Open binary reader
                binaryReader = new BinaryReader(fileStream);

                // Create GZip stream
                gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);

                // Read file header (30 characters) 
                // Ex: MPfm Peak File (version# 1.00)                
                fileHeader = new string(binaryReader.ReadChars(31));

                // Extract version and validate
                string version = fileHeader.Substring(fileHeader.Length - 5, 4);
                if (version != this.version)
                {
                    throw new PeakFileFormatIncompatibleException("Error: The peak file format is not compatible. Expecting version " + this.version + " instead of version " + version + ".", null);
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
                // Get file information
                FileInfo fileInfo = new FileInfo(file);

                // Increment length
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
            // Get list of files
            string[] files = Directory.GetFiles(path, "*.mpfmPeak");

            // Loop through files            
            foreach (string file in files)
            {
                // Delete file
                File.Delete(file);
            }
        }
    }
    	
    /// <summary>
    /// Defines the data used with the OnProcessStarted event.
    /// </summary>
    public class PeakFileStartedData
    {
        /// <summary>
        /// Defines the audio file path length in bytes.
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// Defines the total number of blocks to read.
        /// </summary>
        public int TotalBlocks { get; set; }
    }

    /// <summary>
    /// Defines the progress data used with the OnProcessData event.
    /// </summary>
    public class PeakFileProgressData
    {
        /// <summary>
        /// Defines the audio file path to analyse in order to generate the peak file.
        /// </summary>
        public string AudioFilePath { get; set; }
        /// <summary>
        /// Defines the peak file path.
        /// </summary>
        public string PeakFilePath { get; set; }
        /// <summary>
        /// Defines the thread number currently reporting.
        /// </summary>
        public int ThreadNumber { get; set; }
        /// <summary>
        /// Defines the current thread progress in percentage.
        /// </summary>
        public float PercentageDone { get; set; }
        /// <summary>
        /// Defines the audio file path length in bytes.
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// Defines the current block to read.
        /// </summary>
        public int CurrentBlock { get; set; }
        /// <summary>
        /// Defines the total number of blocks to read.
        /// </summary>
        public int TotalBlocks { get; set; }

        /// <summary>
        /// Defines the list of min/max wave data values for waveform.
        /// Useful for displaying the waveform generation in real-time.
        /// </summary>
        public List<WaveDataMinMax> MinMax { get; set; }
    }

    /// <summary>
    /// Defines the data used with the OnProcessDone event.
    /// </summary>
    public class PeakFileDoneData
    {
        public string AudioFilePath { get; set; }
        public bool Cancelled { get; set; }
    }

    /// <summary>
    /// This Exception class is raised when the peak file is corrupted.    
    /// Related to the PeakFile class.
    /// </summary>
    [Serializable]
    public class PeakFileCorruptedException 
        : Exception
    {
        /// <summary>
        /// Default constructor for the PeakFileCorruptedException exception class.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public PeakFileCorruptedException(string message, Exception innerException) 
            : base(message, innerException)
        {

        }
    }

    /// <summary>
    /// This Exception class is raised when the peak file is incompatible.
    /// Related to the PeakFile class.
    /// </summary>
    [Serializable]
    public class PeakFileFormatIncompatibleException
        : Exception
    {
        /// <summary>
        /// Default constructor for the PeakFileFormatIncompatibleException exception class.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public PeakFileFormatIncompatibleException(string message, Exception innerException) 
            : base(message, innerException)
        {

        }
    }

}
