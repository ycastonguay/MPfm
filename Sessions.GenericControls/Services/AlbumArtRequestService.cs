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
using Sessions.GenericControls.Graphics;

namespace Sessions.GenericControls.Services
{
    public class AlbumArtRequestService : IAlbumArtRequestService
    {
        public const int MaxNumberOfRequests = 20;
        #if MACOSX
        public const int MaximumNumberOfTasks = 1;
        #else
        public const int MaximumNumberOfTasks = 2;
        #endif
        private readonly object _lockerRequests = new object();
        private readonly IAlbumArtCacheService _cacheService;
        private readonly IDisposableImageFactory _disposableImageFactory;
        private int _numberOfTasksRunning;
        private List<AlbumArtRequest> _requests;

        public int Count { get { return _requests.Count; } }

        public delegate void AlbumArtExtractedDelegate(IBasicImage image, AlbumArtRequest request);
        public delegate bool CheckIfAlbumArtShouldBeExtractedDelegate(AlbumArtRequest request);
        public event AlbumArtExtractedDelegate OnAlbumArtExtracted;
        public event CheckIfAlbumArtShouldBeExtractedDelegate OnCheckIfAlbumArtShouldBeExtracted;

        public AlbumArtRequestService(IAlbumArtCacheService cacheService, IDisposableImageFactory disposableImageFactory)
        {
            _requests = new List<AlbumArtRequest>();
            _cacheService = cacheService;
            _disposableImageFactory = disposableImageFactory;

            StartRequestProcessLoop();
        }

        protected void AlbumArtExtracted(IBasicImage image, AlbumArtRequest request)
        {
            if(OnAlbumArtExtracted != null)
                OnAlbumArtExtracted(image, request);
        }

        protected bool CheckIfAlbumArtShouldBeExtracted(AlbumArtRequest request)
        {
            if (OnCheckIfAlbumArtShouldBeExtracted != null)
                return OnCheckIfAlbumArtShouldBeExtracted(request);

                // It should be removed using RemoveRequest instead...

            return false;
        }

        public void FlushRequests()
        {
            lock (_lockerRequests)
            {
                _requests.Clear();
            }
        }

        public Task RequestAlbumArt(AlbumArtRequest request)
        {
            // Make sure we don't slow down GetTile() by creating a task and running LINQ queries on another thread
            var task = Task.Factory.StartNew(() =>
            {
                // Check if a tile already exists
                var existingAlbumArt = _cacheService.GetAlbumArt(request.ArtistName, request.AlbumTitle);
                lock (_lockerRequests)
                {
                    // Check if bitmap has already been requested in queue
                    var existingRequest = _requests.FirstOrDefault(obj =>
                        obj.ArtistName == request.ArtistName &&
                        obj.AlbumTitle == request.AlbumTitle);

                    // Request a new bitmap only if necessary
                    if (existingRequest == null && existingAlbumArt == null)
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

        public void StartRequestProcessLoop()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    //Console.WriteLine("WaveFormCacheService - BitmapRequestProcessLoop - Loop - requests.Count: {0} numberOfBitmapTasksRunning: {1}", _requests.Count, _numberOfBitmapTasksRunning);
                    var requestsToProcess = new List<AlbumArtRequest>();
                    int requestCount = 0;
                    lock (_lockerRequests)
                    {
                        while (_requests.Count > 0 && _numberOfTasksRunning < MaximumNumberOfTasks)
                        {
                            //int index = 0; // FIFO
                            int index = _requests.Count - 1; // LIFO
                            _numberOfTasksRunning++;
                            var request = _requests[index];
                            requestsToProcess.Add(request);
                            _requests.RemoveAt(index);
                        }
                        requestCount = _requests.Count;
                    }

                    foreach (var request in requestsToProcess)
                    {
                        //Console.WriteLine("WaveFormCacheService - BitmapRequestProcessLoop - Processing bitmap request - boundsBitmap: {0} boundsWaveForm: {1} zoom: {2} numberOfBitmapTasksRunning: {3}", request.BoundsBitmap, request.BoundsWaveForm, request.Zoom, _numberOfBitmapTasksRunning);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(ExtractImageInternal), request);
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
            
        private void ExtractImageInternal(Object stateInfo)
        {
            var request = stateInfo as AlbumArtRequest;        
            var image = ExtractImage(request);

            if(image != null)
                _cacheService.AddAlbumArt(image, request.ArtistName, request.AlbumTitle);
        }

        private IBasicImage ExtractImage(AlbumArtRequest request)
        {
            IBasicImage image = null;
            var bytes = AudioFile.ExtractImageByteArrayForAudioFile(request.AudioFilePath);
            if (bytes != null && bytes.Length > 0)
                image = _disposableImageFactory.CreateImageFromByteArray(bytes, request.Width, request.Height);

            return image;
        }
    }
}
