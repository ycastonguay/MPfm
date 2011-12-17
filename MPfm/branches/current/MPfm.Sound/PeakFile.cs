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
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MPfm.Sound.BassNetWrapper;

namespace MPfm.Sound
{
    /// <summary>
    /// The PeakFile class can generate peak files asynchronously using multiple threads with Reactive Extensions.
    /// It is also cancellable. Use the ProcessData event to get the progress.
    /// </summary>
    public class PeakFile
    {
        /// <summary>
        /// Defines the current peak file version. Used when reading peak files to make sure
        /// the format is compatible.
        /// </summary>
        private string m_version = "1.00";

        /// <summary>
        /// Defines the list of IDisposables (subscriptions to IObservables).
        /// </summary>
        private List<IDisposable> m_listSubscriptions = null;

        /// <summary>
        /// List of IObservables using PeakFileProgressData to report the thread progress.
        /// </summary>
        private List<IObservable<PeakFileProgressData>> m_listObservables = null;

        /// <summary>
        /// Defines the current index in the list of files (in the FilePaths property).
        /// </summary>
        private int m_currentIndex = 0;

        /// <summary>
        /// Private value for the FilePaths property.
        /// </summary>
        private Dictionary<string, string> m_filePaths = null;
        /// <summary>
        /// Dictionary defining the audio (key) and peak (value) file paths to process.
        /// Can be updated in real-time (insert new items at the end of the list!).
        /// </summary>
        public Dictionary<string, string> FilePaths
        {
            get
            {
                return m_filePaths;
            }
        }

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
        /// Private value for the IsProcessing property.
        /// </summary>
        private bool m_isProcessing = false;
        /// <summary>
        /// Indicates if the class is currently generating peak files.
        /// </summary>
        public bool IsProcessing
        {
            get
            {
                return m_isProcessing;
            }
        }

        /// <summary>
        /// Private value for the NumberOfThreadsRunning property.
        /// </summary>
        private int m_numberOfThreadsRunning = 0;
        /// <summary>
        /// Indicates the number of threads currently running.
        /// </summary>
        public int NumberOfThreadsRunning
        {
            get
            {
                return m_numberOfThreadsRunning;
            }
        }

        /// <summary>
        /// Private value for the NumberOfThreads property.        
        /// </summary>
        private int m_numberOfThreads = 1;
        /// <summary>
        /// Defines the number of threads used for peak file generation.
        /// </summary>
        public int NumberOfThreads
        {
            get
            {
                return m_numberOfThreads;
            }
        }

        /// <summary>
        /// Private value for the ProgressReportBlockInterval property.
        /// </summary>
        private int m_progressReportBlockInterval = 200;
        /// <summary>
        /// Defines when the OnProgressData event is called; it will be called
        /// every x blocks (where x is ProgressReportBlockInterval). 
        /// The default value is 20.
        /// </summary>
        public int ProgressReportBlockInterval
        {
            get
            {
                return m_progressReportBlockInterval;
            }
            set
            {
                m_progressReportBlockInterval = value;
            }
        }

        /// <summary>
        /// Default constructor for the PeakFile class.
        /// </summary>
        /// <param name="numberOfThreads">Defines the number of threads used for peak file generation</param>
        public PeakFile(int numberOfThreads)
        {
            // Set private values
            m_numberOfThreads = numberOfThreads;
        }

        /// <summary>
        /// Generates a peak file for an audio file. This method returns an IObservable object for use with Reactive Extensions.
        /// Note: BASS.NET should be initialized already before calling this method. This uses a decode stream.
        /// </summary>
        /// <param name="audioFilePath">Audio file path</param>
        /// <param name="peakFilePath">Peak file path</param>
        /// <param name="threadNumber">Thread number</param>
        /// <returns>Observable object with PeakFileProgressData</returns>
        protected IObservable<PeakFileProgressData> GeneratePeakFileAsync(string audioFilePath, string peakFilePath, int threadNumber)
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

                        // Get audio file length
                        audioFileLength = channelDecode.GetLength();

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
                        string version = "MPfm Peak File (version# " + m_version + ")";
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
                        dataStarted.ThreadNumber = threadNumber;
                        dataStarted.Length = audioFileLength;
                        dataStarted.CurrentBlock = 0;
                        dataStarted.TotalBlocks = (Int32)blocks;
                        dataStarted.MinMax = listMinMaxForProgressData;
                        o.OnNext(dataStarted);

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

                            // Update progress every X blocks (m_progressReportBlockInterval) default = 20
                            dataBlockRead += read;
                            if (dataBlockRead >= read * m_progressReportBlockInterval)
                            {
                                // Reset flag
                                dataBlockRead = 0;

                                // Report progress
                                PeakFileProgressData dataProgress = new PeakFileProgressData();
                                dataProgress.AudioFilePath = audioFilePath;
                                dataProgress.PeakFilePath = peakFilePath;
                                dataProgress.PercentageDone = (((float)bytesRead / (float)audioFileLength) / 2) * 100;
                                dataProgress.ThreadNumber = threadNumber;
                                dataProgress.Length = audioFileLength;
                                dataProgress.CurrentBlock = currentBlock;
                                dataProgress.TotalBlocks = (Int32)blocks;
                                dataProgress.MinMax = listMinMaxForProgressData;
                                o.OnNext(dataProgress);

                                // Reset min/max list
                                //listMinMaxForProgressData.Clear();
                                listMinMaxForProgressData = new List<WaveDataMinMax>();
                            }

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
                                // Delete file
                                File.Delete(peakFilePath);
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

        /// <summary>
        /// Subscribe to a specific IObservable.
        /// </summary>
        /// <param name="index">File path index</param>
        private void Subscribe(int index)
        {
            // Add subsription with Finally (executed when the thread ends)
            m_listSubscriptions.Add(m_listObservables[index].Finally(() =>
            {
                // Check if there is more stuff to load
                if (m_currentIndex >= m_filePaths.Count - 1)
                {
                    // Decrement the number of threads running
                    m_numberOfThreadsRunning--;

                    // There might be multiple threads ending here, so make sure we don't raise the OnProgressDone more than once.
                    if (m_numberOfThreadsRunning == 0)
                    {
                        // There aren't any other peak files to generate; set flags
                        m_isProcessing = false;

                        // Is an event binded to OnProcessDone?
                        if (OnProcessDone != null)
                        {
                            // Raise event with data
                            OnProcessDone(new PeakFileDoneData());
                        }
                    }                    
                    
                    return;
                }

                // Increment current index
                m_currentIndex++;

                // Load next thread
                Subscribe(m_currentIndex);

            // Subscribe to the IObservable (starts the thread)
            }).Subscribe(o =>
            {
                // Check what percentage is done
                if (o.PercentageDone == 0)
                {
                    // Is an event binded to OnProcessData?
                    if (OnProcessStarted != null)
                    {
                        // Raise started event with file length
                        PeakFileStartedData data = new PeakFileStartedData();
                        data.Length = o.Length;
                        data.TotalBlocks = o.TotalBlocks;
                        OnProcessStarted(data);
                    }
                }
                else
                {
                    // Is an event binded to OnProcessData?
                    if (OnProcessData != null)
                    {
                        // Raise event with data
                        OnProcessData(o);
                    }
                }
            }));
        }

        /// <summary>
        /// Generates a peak file.
        /// </summary>
        /// <param name="audioFilePath">Audio file path</param>
        /// <param name="peakFilePath">Peak file path</param>
        public void GeneratePeakFile(string audioFilePath, string peakFilePath)
        {
            // Create dictionary and call the GeneratePeakFiles method
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add(audioFilePath, peakFilePath);
            GeneratePeakFiles(dictionary);
        }

        /// <summary>
        /// Generates a list of peak files
        /// </summary>
        /// <param name="filePaths">Dictionary of audio file paths (key) and peak file paths (value)</param>
        public void GeneratePeakFiles(Dictionary<string, string> filePaths)
        {
            // Check there are active threads
            if (m_isProcessing)
            {
                throw new Exception("Error: The process cannot be restarted when there are currently active threads!");
            }

            // Set private values            
            m_filePaths = filePaths;

            // Set flags
            m_isProcessing = true;
            m_currentIndex = 0;

            // Create lists
            m_listObservables = new List<IObservable<PeakFileProgressData>>();
            m_listSubscriptions = new List<IDisposable>();

            // Loop through file paths
            for (int a = 0; a < filePaths.Count; a++)
            {                
                // Create IObservable for peak file
                m_listObservables.Add(GeneratePeakFileAsync(filePaths.Keys.ElementAt(a), filePaths.Values.ElementAt(a), a));
            }

            // Determine how many files to process (do not start more threads than files to process!)
            int numberOfFilesToProcess = filePaths.Count;
            if (filePaths.Count > NumberOfThreads)
            {
                // Set number of files to process to the number of threads
                numberOfFilesToProcess = NumberOfThreads;
            }
            
            // Loop through initial threads
            for (int a = 0; a < numberOfFilesToProcess; a++)
            {
                // Subscribe
                Subscribe(m_currentIndex);

                // Increment current index
                m_currentIndex++;
            }

            // Set the number of threads running
            m_numberOfThreadsRunning = numberOfFilesToProcess;

            //m_subscription = Observable.Merge(list).Subscribe(o =>
            //{
            //    if (OnProcessData != null)
            //    {
            //        OnProcessData(o.PercentageDone);
            //    }
            //});
        }

        /// <summary>
        /// Cancels the peak file generation.
        /// </summary>
        public void Cancel()
        {
            // Check if the subscriptions are valid
            if (m_listSubscriptions == null || m_listSubscriptions.Count == 0)
            {
                throw new Exception("Error cancelling process: The subscription list is empty or doesn't exist!");
            }

            // Check if the class is currently processing data
            if (!m_isProcessing)
            {
                throw new Exception("Error cancelling process: There are no currently active threads!");
            }

            // Loop through subscriptions            
            while(true)
            {
                try
                {
                    // Check if there is a subscription left
                    if (m_listSubscriptions.Count == 0)
                    {
                        // Exit loop
                        break;
                    }

                    // Dispose subscription and remove it from list
                    m_listSubscriptions[0].Dispose();
                    m_listSubscriptions.RemoveAt(0);                    
                }
                catch(Exception ex)
                {
                    // Throw exception and exit loop
                    throw ex;                    
                }
            }

            // CANNOT enumerate in a list that is currently modified.
            //foreach (IDisposable subscription in m_listSubscriptions)
            //{
            //    try
            //    {                    
            //        subscription.Dispose();
            //    }
            //    catch(Exception ex)
            //    {
            //        throw ex;
            //    }
            //}
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
                if (version != m_version)
                {
                    throw new PeakFileFormatIncompatibleException("Error: The peak file format is not compatible. Expecting version " + m_version + " instead of version " + version + ".", null);
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
    /// Defines the data used with the OnProcessDone event (actually nothing).
    /// </summary>
    public class PeakFileDoneData
    {
    }

    /// <summary>
    /// This Exception class is raised when the peak file is corrupted.    
    /// Related to the PeakFile class.
    /// </summary>
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

