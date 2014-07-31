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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Services.Events;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Services.Objects;
using Sessions.Sound.AudioFiles;

namespace Sessions.GenericControls.Services
{
    public class WaveFormRequestService : IWaveFormRequestService
    {
        public const int MaxNumberOfRequests = 20;
        #if MACOSX
        public const int MaximumNumberOfTasks = 1;
        #else
        public const int MaximumNumberOfTasks = 2;
        #endif
        private readonly object _lockerRequests = new object();
        private readonly IWaveFormRenderingService _waveFormRenderingService;
        private readonly ITileCacheService _cacheService;
        private int _numberOfBitmapTasksRunning;
        private List<WaveFormBitmapRequest> _requests;

        public event WaveFormRenderingService.GenerateWaveFormEventHandler GenerateWaveFormBitmapEndedEvent;
        public int Count { get { return _requests.Count; } }

        public WaveFormRequestService(IWaveFormRenderingService waveFormRenderingService, ITileCacheService cacheService)
        {
            _requests = new List<WaveFormBitmapRequest>();
            _cacheService = cacheService;
            _waveFormRenderingService = waveFormRenderingService;
            _waveFormRenderingService.GenerateWaveFormBitmapEndedEvent += HandleGenerateWaveFormEndedEvent;

            StartBitmapRequestProcessLoop();
        }

        private void HandleGenerateWaveFormEndedEvent(object sender, GenerateWaveFormEventArgs e)
        {
            //Console.WriteLine("WaveFormCacheService - HandleGenerateWaveFormEndedEvent - e.Width: {0} e.Zoom: {1}", e.Width, e.Zoom);
            lock (_lockerRequests)
            {
                _numberOfBitmapTasksRunning--;
            }

            var tile = new WaveFormTile()
            {
                ContentOffset = new BasicPoint(e.OffsetX, 0),
                Zoom = e.Zoom,
                Image = e.Image
            };
            _cacheService.AddTile(tile, e.IsScrollBar);

            if (GenerateWaveFormBitmapEndedEvent != null)
                GenerateWaveFormBitmapEndedEvent(sender, e);
        }

        public void Flush()
        {
            lock (_lockerRequests)
            {
                _requests.Clear();
            }
        }

        public Task RequestBitmap(BasicRectangle boundsBitmap, BasicRectangle boundsWaveForm, float zoom, WaveFormDisplayType displayType)
        {
            // Make sure we don't slow down GetTile() by creating a task and running LINQ queries on another thread
            var task = Task.Factory.StartNew(() =>
            {
                var request = new WaveFormBitmapRequest()
                {
                    DisplayType = displayType,
                    BoundsBitmap = boundsBitmap,
                    BoundsWaveForm = boundsWaveForm,
                    Zoom = zoom
                };

                // Check if a tile already exists
                WaveFormTile existingTile = _cacheService.GetTile(boundsBitmap.X, zoom, false);
                lock (_lockerRequests)
                {
                    // Check if bitmap has already been requested in queue
                    var existingRequest = _requests.FirstOrDefault(obj =>
                        obj.BoundsBitmap.Equals(request.BoundsBitmap) &&
                        obj.BoundsWaveForm.Equals(request.BoundsWaveForm) &&
                        obj.Zoom == request.Zoom);

                    // Request a new bitmap only if necessary
                    if (existingRequest == null && existingTile == null)
                    {
                        //Console.WriteLine("WaveFormCacheService - Adding bitmap request to queue - zoom: {0} boundsBitmap: {1} boundsWaveForm: {2}", zoom, boundsBitmap, boundsWaveForm);
                        _requests.Add(request);

                        // Remove the oldest request from the list if we hit the maximum 
                        if(_requests.Count > MaxNumberOfRequests)
                            _requests.RemoveAt(0);

                        //Console.WriteLine("............. PULSING");
                        Monitor.Pulse(_lockerRequests);                        
                    }
                }
            });
            return task;
        }

        public void StartBitmapRequestProcessLoop()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    //Console.WriteLine("WaveFormCacheService - BitmapRequestProcessLoop - Loop - requests.Count: {0} numberOfBitmapTasksRunning: {1}", _requests.Count, _numberOfBitmapTasksRunning);
                    var requestsToProcess = new List<WaveFormBitmapRequest>();
                    int requestCount = 0;
                    lock (_lockerRequests)
                    {
                        while (_requests.Count > 0 && _numberOfBitmapTasksRunning < MaximumNumberOfTasks)
                        {
                            //int index = 0; // FIFO
                            int index = _requests.Count - 1; // LIFO
                            _numberOfBitmapTasksRunning++;
                            var request = _requests[index];
                            requestsToProcess.Add(request);
                            _requests.RemoveAt(index);
                        }
                        requestCount = _requests.Count;
                    }

                    foreach (var request in requestsToProcess)
                    {
                        //Console.WriteLine("WaveFormCacheService - BitmapRequestProcessLoop - Processing bitmap request - boundsBitmap: {0} boundsWaveForm: {1} zoom: {2} numberOfBitmapTasksRunning: {3}", request.BoundsBitmap, request.BoundsWaveForm, request.Zoom, _numberOfBitmapTasksRunning);
                        _waveFormRenderingService.RequestBitmap(request); // ThreadQueueWorkItem will manage a thread pool
                    }

                    if (requestCount > 0)
                    {
                        //Console.WriteLine(">>>>>>>>> SLEEPING");
                        Thread.Sleep(50);
                    }
                    else
                    {
                        lock (_lockerRequests)
                        {
                            //Console.WriteLine(">>>>>>>>> WAITING");
                            Monitor.Wait(_lockerRequests);
                            //Console.WriteLine(">>>>>>>>> WOKEN UP!");
                        }
                    }
                }
            }));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
