// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using Sessions.Sound.PeakFiles.Interfaces;
using System;
using System.Collections.Concurrent;

namespace Sessions.Sound.PeakFiles
{
    public class PeakFileQueueService : IPeakFileQueueService
    {
        private readonly ConcurrentQueue<PeakFileQueueTask> _queue;
        private readonly PeakFileService _service;

        public event PeakFileGenerationStarted OnGenerationStarted;
        public event PeakFileGenerationProgress OnGenerationProgress;
        public event PeakFileGenerationFinished OnGenerationFinished;

        public bool IsLoading { get { return _service.IsLoading; } }

        public PeakFileQueueService()
        {
            _queue = new ConcurrentQueue<PeakFileQueueTask>();
            _service = new PeakFileService();
            _service.OnGenerationStarted += Service_OnGenerationStarted;
            _service.OnGenerationProgress += Service_OnGenerationProgress;
            _service.OnGenerationFinished += Service_OnGenerationFinished;
        }

        private void Service_OnGenerationStarted(PeakFileGenerationStartedData data)
        {
            if(OnGenerationStarted != null)
                OnGenerationStarted(data);
        }

        private void Service_OnGenerationProgress(PeakFileGenerationProgressData data)
        {
            if(OnGenerationProgress != null)
                OnGenerationProgress(data);            
        }

        private void Service_OnGenerationFinished(PeakFileGenerationFinishedData data)
        {
            if (OnGenerationFinished != null)
                OnGenerationFinished(data);

            if (_queue.Count > 0)
            {
                PeakFileQueueTask task = null;
                _queue.TryDequeue(out task);

                if (task != null)
                {
                    Console.WriteLine("--------> [PeakFileQueueService] - Peak file generated (cancel={0}), dequeuing {0}", task.PeakFilePath);
                    _service.GeneratePeakFile(task.AudioFilePath, task.PeakFilePath);
                }
            }
        }

        public void GeneratePeakFile(string audioFilePath, string peakFilePath)
        {
            Console.WriteLine("--------> [PeakFileQueueService] - GeneratePeakFile - isLoading: {0} isCanceling: {1} peakFilePath: {2}", _service.IsLoading, _service.IsCanceling, peakFilePath);
            if (_service.IsLoading)
            {
                Console.WriteLine("--------> [PeakFileQueueService] - GeneratePeakFile - Peak file service is busy, enqueuing {0}", peakFilePath);
                var task = new PeakFileQueueTask(audioFilePath, peakFilePath);
                _queue.Enqueue(task);

                // Make sure we don't interrupt a cancel in progress
                if (!_service.IsCanceling)
                {
                    Console.WriteLine("--------> [PeakFileQueueService] - GeneratePeakFile - Canceling peak file generation...");
                    _service.Cancel();
                }
            }
            else
            {
                Console.WriteLine("--------> [PeakFileQueueService] - GeneratePeakFile - Generating {0}", peakFilePath);
                _service.GeneratePeakFile(audioFilePath, peakFilePath);
            }
        }

        public void CancelAll()
        {
            Console.WriteLine("--------> [PeakFileQueueService] - CancelAll");
            PeakFileQueueTask task = null;
            while (!_queue.IsEmpty)
            {
                _queue.TryDequeue(out task);
            }
            _service.Cancel();
        }

        public class PeakFileQueueTask
        {
            public string AudioFilePath { get; set; }
            public string PeakFilePath { get; set; }

            public PeakFileQueueTask(string audioFilePath, string peakFilePath)
            {
                AudioFilePath = audioFilePath;
                PeakFilePath = peakFilePath;
            }
        }      
    }
}