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
using System.Threading.Tasks;
using MPfm.Core;

namespace MPfm.Library
{
    /// <summary>
    /// The UpdateLibrary class scans the metadata of audio files and imports them into the database.
    /// It also cleans up the database, searches for broken file paths, and more.
    /// Supports multi-threading through Reactive Extensions. It is also cancellable.     
    /// </summary>
    public class UpdateLibrary
    {
        private int currentTaskIndex = 0;

        private string m_currentFile = string.Empty;
        public string CurrentFile
        {
            get
            {
                return m_currentFile;
            }
        }

        private float m_percentageDone = 0;
        public float PercentageDone
        {
            get
            {
                return m_percentageDone;
            }
        }

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

        public async Task<List<AudioFile>> LoadFiles(List<string> filePaths)
        {

            // Todo: Split WHENALL in 50 files, and report only every 50 files. 
            // OR use the polling technique since 50 files in old hardware might take a lot longer than newer hardware.

            int maxTasks = 50;
            currentTaskIndex = 0;
            List<AudioFile> listAudioFiles = new List<AudioFile>();

            // Create gateway
            MPfmGateway gateway = new MPfmGateway(m_databaseFilePath);

            while (true)
            {
                // Check how many tasks to process
                int numberOfTasksToProcess = filePaths.Count - currentTaskIndex;

                if (numberOfTasksToProcess == 0)
                {
                    m_percentageDone = 100;
                    break;
                }
                else if (filePaths.Count - currentTaskIndex > maxTasks)
                {
                    numberOfTasksToProcess = maxTasks;
                }

                List<Task<UpdateLibraryProgressData>> tasks = new List<Task<UpdateLibraryProgressData>>();
                for (int a = 0; a < numberOfTasksToProcess; a++)
                {
                    currentTaskIndex++;
                    tasks.Add(LoadAudioFileAsync(filePaths[currentTaskIndex-1], currentTaskIndex, filePaths.Count));
                }

                UpdateLibraryProgressData[] data = await TaskEx.WhenAll(tasks);
                List<AudioFile> audioFiles = new List<AudioFile>();
                for (int a = 0; a < data.Length; a++)
                {
                    if (data[a].AudioFile != null)
                    {
                        audioFiles.Add(data[a].AudioFile);
                    }
                }


                //listAudioFiles.AddRange(audioFiles);

                // Insert audio files into database                
                gateway.InsertAudioFiles(audioFiles);
                //m_audioFilesToInsert.Clear();
            }

            return listAudioFiles;
        }

        public async Task<UpdateLibraryProgressData> LoadAudioFileAsync(string filePath, int index, int count)
        {
            AudioFile audioFile = null;
            UpdateLibraryProgressData data = new UpdateLibraryProgressData();
            data.FilePath = filePath;
            m_currentFile = filePath;

            try
            {
                //Tracing.Log("Loading " + filePath + "...");
                audioFile = await TaskEx.Run(() => new AudioFile(filePath));
                data.AudioFile = audioFile;                
            }
            catch (Exception ex)
            {
                data.Exception = new UpdateLibraryException("Error reading audio file", ex);
            }

            // Set percentage
            m_percentageDone = ((float)index / (float)count) * 100;

            return data;
        }

        /// <summary>
        /// Cancels the peak file generation.
        /// </summary>
        public void Cancel()
        {
            //// Check if the subscriptions are valid
            //if (m_listSubscriptions == null || m_listSubscriptions.Count == 0)
            //{
            //    throw new Exception("Error cancelling process: The subscription list is empty or doesn't exist!");
            //}

            //// Check if the class is currently processing data
            //if (!m_isProcessing)
            //{
            //    throw new Exception("Error cancelling process: There are no currently active threads!");
            //}

            //// Loop through subscriptions            
            //while (true)
            //{
            //    try
            //    {
            //        // Check if there is a subscription left
            //        if (m_listSubscriptions.Count == 0)
            //        {
            //            // Exit loop
            //            break;
            //        }

            //        // Dispose subscription and remove it from list
            //        m_listSubscriptions[0].Dispose();
            //        m_listSubscriptions.RemoveAt(0);
            //    }
            //    catch (Exception ex)
            //    {
            //        // Throw exception and exit loop
            //        throw ex;
            //    }
            //}
        }
    }    
}

