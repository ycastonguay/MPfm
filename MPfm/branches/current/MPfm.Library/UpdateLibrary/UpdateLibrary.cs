//
// UpdateLibrary.cs: The UpdateLibrary class scans the metadata of audio files and 
//                   imports them into the database. It also cleans up the database, 
//                   searches for broken file paths, and more. Supports multi-threading 
//                   through Reactive Extensions. It is also cancellable. 
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
        // Private variables
        private int currentTaskIndex = 0;

        /// <summary>
        /// Private value for the CurrentFile property.
        /// </summary>
        private string currentFile = string.Empty;
        /// <summary>
        /// Indicates which file is currently processing.
        /// </summary>
        public string CurrentFile
        {
            get
            {
                return currentFile;
            }
        }

        /// <summary>
        /// Private value for the PercentageDone property.
        /// </summary>
        private float percentageDone = 0;
        /// <summary>
        /// Indicates how much percentage of the process is done.
        /// </summary>
        public float PercentageDone
        {
            get
            {
                return percentageDone;
            }
        }

        ///// <summary>
        ///// Delegate for the OnProcessDone event.
        ///// </summary>
        ///// <param name="data">Event data</param>
        //public delegate void ProcessDone(UpdateLibraryDoneData data);

        ///// <summary>
        ///// Event called when all the GeneratePeakFiles threads have completed their work.
        ///// </summary>
        //public event ProcessDone OnProcessDone;

        /// <summary>
        /// Private value for the FilePaths property.
        /// </summary>
        private List<string> filePaths = null;
        /// <summary>
        /// List of audio files to import into the database.
        /// Can be updated in real-time (insert new items at the end of the list!).
        /// </summary>
        public List<string> FilePaths
        {
            get
            {
                return filePaths;
            }
        }

        /// <summary>
        /// Private value for the DatabaseFilePath property.
        /// </summary>
        private string databaseFilePath = null;
        /// <summary>
        /// MPfm database file path.
        /// </summary>
        public string DatabaseFilePath
        {
            get
            {
                return databaseFilePath;
            }
        }

        /// <summary>
        /// Private value for the IsProcessing property.
        /// </summary>
        private bool isProcessing = false;
        /// <summary>
        /// Indicates if the class is currently generating peak files.
        /// </summary>
        public bool IsProcessing
        {
            get
            {
                return isProcessing;
            }
        }

        /// <summary>
        /// Private value for the NumberOfThreads property.        
        /// </summary>
        private int numberOfThreads = 1;
        /// <summary>
        /// Defines the number of threads used for peak file generation.
        /// </summary>
        public int NumberOfThreads
        {
            get
            {
                return numberOfThreads;
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
            this.numberOfThreads = numberOfThreads;
            this.databaseFilePath = databaseFilePath;
        }

        /// <summary>
        /// Loads a list of files.
        /// </summary>
        /// <param name="filePaths">List of file paths</param>
        /// <returns>List of tasks</returns>
        public async Task<List<AudioFile>> LoadFiles(List<string> filePaths)
        {

            // Todo: Split WHENALL in 50 files, and report only every 50 files. 
            // OR use the polling technique since 50 files in old hardware might take a lot longer than newer hardware.

            int maxTasks = 2;
            currentTaskIndex = 0;
            List<AudioFile> listAudioFiles = new List<AudioFile>();

            // Create gateway
            MPfmGateway gateway = new MPfmGateway(databaseFilePath);

            while (true)
            {
                // Check how many tasks to process
                int numberOfTasksToProcess = filePaths.Count - currentTaskIndex;

                if (numberOfTasksToProcess == 0)
                {
                    percentageDone = 100;
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

        /// <summary>
        /// Loads a list of audio files into the library asychronously.
        /// </summary>
        /// <param name="filePath">List of file paths</param>
        /// <param name="index">File index</param>
        /// <param name="count">File count</param>
        /// <returns>Task</returns>
        public async Task<UpdateLibraryProgressData> LoadAudioFileAsync(string filePath, int index, int count)
        {
            AudioFile audioFile = null;
            UpdateLibraryProgressData data = new UpdateLibraryProgressData();
            data.FilePath = filePath;
            currentFile = filePath;

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
            percentageDone = ((float)index / (float)count) * 100;

            return data;
        }

        /// <summary>
        /// Cancels the peak file generation.
        /// </summary>
        public void Cancel()
        {
            //// Check if the subscriptions are valid
            //if (listSubscriptions == null || listSubscriptions.Count == 0)
            //{
            //    throw new Exception("Error cancelling process: The subscription list is empty or doesn't exist!");
            //}

            //// Check if the class is currently processing data
            //if (!isProcessing)
            //{
            //    throw new Exception("Error cancelling process: There are no currently active threads!");
            //}

            //// Loop through subscriptions            
            //while (true)
            //{
            //    try
            //    {
            //        // Check if there is a subscription left
            //        if (listSubscriptions.Count == 0)
            //        {
            //            // Exit loop
            //            break;
            //        }

            //        // Dispose subscription and remove it from list
            //        listSubscriptions[0].Dispose();
            //        listSubscriptions.RemoveAt(0);
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

