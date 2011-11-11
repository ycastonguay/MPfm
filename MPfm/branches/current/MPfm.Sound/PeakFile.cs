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
        /// Defines the audio and peak file paths to process.
        /// </summary>
        public Dictionary<string, string> FilePaths
        {
            get
            {
                return m_filePaths;
            }
        }

        // Process Data event
        public delegate void ProcessData(float percentage);
        public event ProcessData OnProcessData;

        /// <summary>
        /// Private value for the IsGenerating property.
        /// </summary>
        private bool m_isGenerating = false;
        /// <summary>
        /// Indicates if the class is currently generating peak files.
        /// </summary>
        public bool IsGenerating
        {
            get
            {
                return m_isGenerating;
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
        /// <returns>Observable object with PeakFileProgressData</returns>
        protected IObservable<PeakFileProgressData> GeneratePeakFileAsync(string audioFilePath, string peakFilePath)
        {
            // Declare variables         
            bool cancelled = false;
            FileStream fileStream = null;
            BinaryWriter binaryWriter = null;
            GZipStream gzipStream = null;            
            int chunkSize = 0;
            long audioFileLength = 0;
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
                        
                        // 000000000111111111122222222223
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
                        buffer = new byte[chunkSize];

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

                            // Calculate min/max
                            minMax = AudioTools.GetMinMaxFromWaveData(floatLeft, floatRight, false);                            

                            // Write peak information
                            binaryWriter.Write((double)minMax.leftMin);
                            binaryWriter.Write((double)minMax.leftMax);
                            binaryWriter.Write((double)minMax.rightMin);
                            binaryWriter.Write((double)minMax.rightMax);
                            binaryWriter.Write((double)minMax.mixMin);
                            binaryWriter.Write((double)minMax.mixMax);                            

                            // Update progress every 20 blocks
                            dataBlockRead += read;
                            if (dataBlockRead >= read * 20)
                            {
                                // Reset flag
                                dataBlockRead = 0;

                                // Report progress
                                PeakFileProgressData progress = new PeakFileProgressData();
                                progress.PercentageDone = ((float)bytesRead / (float)audioFileLength) * 100;
                                o.OnNext(progress);
                            }
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
                    // There aren't any other peak files to generate; set flags
                    m_isGenerating = false;
                    return;
                }

                // Increment current index
                m_currentIndex++;

                // Load next thread
                Subscribe(m_currentIndex);

            // Subscribe to the IObservable (starts the thread)
            }).Subscribe(o =>
            {
                // Is an event binded to OnProcessData?
                if (OnProcessData != null)
                {
                    // Raise event with data
                    OnProcessData(o.PercentageDone);
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
            // Check if the class is currently generating peak files
            if (m_isGenerating)
            {
                throw new Exception("Error: The class is already generating peak files!");
            }

            // Set private values            
            m_filePaths = filePaths;

            // Set flags
            m_isGenerating = true;
            m_currentIndex = 0;

            // Create lists
            m_listObservables = new List<IObservable<PeakFileProgressData>>();
            m_listSubscriptions = new List<IDisposable>();

            // Loop through file paths
            foreach(KeyValuePair<string, string> filePath in filePaths)
            {
                // Create IObservable for peak file
                m_listObservables.Add(GeneratePeakFileAsync(filePath.Key, filePath.Value));
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

            // Check if the class is currently generating peak files
            if (!m_isGenerating)
            {
                throw new Exception("Error cancelling process: The class isn't generating any peak files!");
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
                if (currentBlock < numberOfBlocks)
                {
                    throw new PeakFileCorruptedException("Error: The peak file is corrupted!", null);
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
        /// /// <param name="innerException">Inner exception</param>
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

