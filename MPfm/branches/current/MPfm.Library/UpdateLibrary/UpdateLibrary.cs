//
// UpdateLibrary.cs: The UpdateLibrary class scans the metadata of audio files and 
//                   imports them into the database. It also cleans up the database, 
//                   searches for broken file paths, and more. Supports multi-threading 
//                   through Reactive Extensions. It is also cancellable. 
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MPfm.Sound;

namespace MPfm.Library
{
    /// <summary>
    /// The UpdateLibrary class scans the metadata of audio files and imports them into the database.
    /// It also cleans up the database, searches for broken file paths, and more.
    /// Supports multi-threading through Reactive Extensions. It is also cancellable.     
    /// </summary>
    public class UpdateLibrary
    {
        /// <summary>
        /// Defines the list of IDisposables (subscriptions to IObservables).
        /// </summary>
        private List<IDisposable> m_listSubscriptions = null;

        /// <summary>
        /// List of IObservables of UpdateLibraryProgressData.
        /// </summary>
        private List<IObservable<UpdateLibraryProgressData>> m_listObservables = null;

        /// <summary>
        /// Defines the current index in the list of files (in the FilePaths property).
        /// </summary>
        private int m_currentIndex = 0;

        /// <summary>
        /// Delegate for the OnProcessData event.
        /// </summary>
        /// <param name="data">Event data</param>
        public delegate void ProcessData(UpdateLibraryProgressData data);

        /// <summary>
        /// Event called every 20 blocks when generating a peak file.
        /// </summary>
        public event ProcessData OnProcessData;

        /// <summary>
        /// Delegate for the OnProcessDone event.
        /// </summary>
        /// <param name="data">Event data</param>
        public delegate void ProcessDone(UpdateLibraryDoneData data);

        /// <summary>
        /// Event called when all the GeneratePeakFiles threads have completed their work.
        /// </summary>
        public event ProcessDone OnProcessDone;

        /// <summary>
        /// Private value for the FilePaths property.
        /// </summary>
        private List<string> m_filePaths = null;
        /// <summary>
        /// List of audio files to import into the database.
        /// Can be updated in real-time (insert new items at the end of the list!).
        /// </summary>
        public List<string> FilePaths
        {
            get
            {
                return m_filePaths;
            }
        }

        /// <summary>
        /// Private value for the DatabaseFilePath property.
        /// </summary>
        private string m_databaseFilePath = null;
        /// <summary>
        /// MPfm database file path.
        /// </summary>
        public string DatabaseFilePath
        {
            get
            {
                return m_databaseFilePath;
            }
        }

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
        /// Default constructor for the UpdateLibrary class.
        /// </summary>
        /// <param name="numberOfThreads">Defines the number of threads used for scanning audio file metadata</param>
        /// <param name="databaseFilePath">MPfm database file path</param>
        public UpdateLibrary(int numberOfThreads, string databaseFilePath)
        {
            // Set private values
            m_numberOfThreads = numberOfThreads;
            m_databaseFilePath = databaseFilePath;
        }

        /// <summary>
        /// Scans the metadata of an audio file. This method returns an IObservable object for use with Reactive Extensions.        
        /// </summary>
        /// <param name="audioFilePath">Audio file path</param>
        /// <param name="threadNumber">Thread number</param>
        /// <returns>UpdateLibraryProgressData</returns>
        protected IObservable<UpdateLibraryProgressData> ScanMetadataAsync(string audioFilePath, int threadNumber)
        {
            // Declare variables         
            bool cancelled = false;
            UpdateLibraryProgressData data = null;

            // Create observable
            IObservable<UpdateLibraryProgressData> observable = Observable.Create<UpdateLibraryProgressData>(o =>
            {
                // Declare cancel token
                var cancel = new CancellationDisposable();

                // Schedule operation in a new thread
                Scheduler.NewThread.Schedule(() =>
                {
                    // Create data
                    data = new UpdateLibraryProgressData();
                    data.FilePath = audioFilePath;
                    data.ThreadNumber = threadNumber;

                    try
                    {
                        // Read audio file metadata
                        data.AudioFile = new AudioFile(audioFilePath);
                    }
                    catch (Exception ex)
                    {
                        // Create exception
                        data.Exception = new UpdateLibraryException("There was an error while reading the audio file metadata.", ex);
                    }

                    // Send progress and set completed
                    o.OnNext(data);
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
                // NO. DOESNT WORK WITHOUT LOCKING. FUCK.

                // Decrement thread count
                m_numberOfThreadsRunning--;

                // Check if this is the last thread
                if (m_numberOfThreadsRunning == 0)
                {
                    // Wait X number of times to insert audio files into the database
                    if (m_audioFilesToInsert.Count > 50)
                    {
                        // Insert files into database
                        MPfmGateway gateway = new MPfmGateway(m_databaseFilePath);
                        gateway.InsertAudioFiles(m_audioFilesToInsert);
                        m_audioFilesToInsert.Clear();

                        // Is an event binded to OnProcessData?
                        if (OnProcessData != null)
                        {
                            // Raise event with data
                            UpdateLibraryProgressData data = new UpdateLibraryProgressData();
                            data.PercentageDone = ((float)m_currentIndex / (float)m_listObservables.Count) * 100;
                            OnProcessData(data);
                        }
                    }

                    // Determine how many files to process (do not start more threads than files to process!)
                    int numberOfFilesToProcess = m_listObservables.Count - 1 - m_currentIndex;

                    // Check if there are files left
                    if (numberOfFilesToProcess == 0)
                    {                        
                        // Set flags
                        m_isProcessing = false;

                        // Insert the last files into database
                        if (m_audioFilesToInsert.Count > 0)
                        {
                            // Insert files into database
                            MPfmGateway gateway = new MPfmGateway(m_databaseFilePath);
                            gateway.InsertAudioFiles(m_audioFilesToInsert);
                            m_audioFilesToInsert.Clear();
                        }

                        // Is an event binded to OnProcessDone?
                        if (OnProcessDone != null)
                        {
                            // Raise event with data
                            OnProcessDone(new UpdateLibraryDoneData());
                        }

                        return;
                    }
                    else if (numberOfFilesToProcess > NumberOfThreads)
                    {
                        // Set number of files to process to the number of threads
                        numberOfFilesToProcess = NumberOfThreads;
                    }

                    // Subscribe the next x threads
                    m_numberOfThreadsRunning = numberOfFilesToProcess;
                    for (int a = 0; a < numberOfFilesToProcess; a++)
                    {
                        // Subscribe next thread
                        m_currentIndex++;
                        Subscribe(m_currentIndex);
                    }                    
                }

            // Subscribe to the IObservable (starts the thread)
            }).Subscribe(o =>
            {
                try
                {
                    // Insert into list
                    m_audioFilesToInsert.Add(o.AudioFile);


                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }));
        }

        private List<AudioFile> m_audioFilesToInsert = new List<AudioFile>();

        /// <summary>
        /// This is the main method for updating the library.
        /// </summary>
        public void Update()
        {
            // Fetch the list of folders to import

        }

        /// <summary>
        /// Imports audio file metadata from a specific folder path into the database.
        /// </summary>
        /// <param name="folderPath">Folder path</param>        
        public void ImportFolder(string folderPath)
        {
            // Search recursively and import files
            List<string> filePaths = AudioTools.SearchAudioFilesRecursive(folderPath, "MP3;FLAC;OGG");

            // Import new files
            ImportFiles(filePaths);
        }

        /// <summary>
        /// Imports metadata from a list of audio files into the database.
        /// </summary>
        /// <param name="filePaths">List of audio file paths</param>
        public void ImportFiles(List<string> filePaths)
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
            m_listObservables = new List<IObservable<UpdateLibraryProgressData>>();
            m_listSubscriptions = new List<IDisposable>();

            // Loop through file paths
            for (int a = 0; a < filePaths.Count; a++)
            {
                // Create IObservable
                m_listObservables.Add(ScanMetadataAsync(filePaths[a], a));
            }

            // Determine how many files to process (do not start more threads than files to process!)
            int numberOfFilesToProcess = filePaths.Count;
            if (filePaths.Count > NumberOfThreads)
            {
                // Set number of files to process to the number of threads
                numberOfFilesToProcess = NumberOfThreads;
            }

            // Set the number of threads running
            m_numberOfThreadsRunning = numberOfFilesToProcess;

            // Loop through initial threads
            for (int a = 0; a < numberOfFilesToProcess; a++)
            {
                // Subscribe
                Subscribe(m_currentIndex);

                // Increment current index
                m_currentIndex++;
            }
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
            while (true)
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
                catch (Exception ex)
                {
                    // Throw exception and exit loop
                    throw ex;
                }
            }
        }
    }
}

