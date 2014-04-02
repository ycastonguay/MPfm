// Copyright � 2011-2013 Yanick Castonguay
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
using System.Linq;
using System.Threading;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Services.Events;
using MPfm.GenericControls.Services.Interfaces;
using MPfm.Sound.AudioFiles;

namespace MPfm.GenericControls.Services
{
    public class WaveFormCacheService : IWaveFormCacheService
    {
        public const int TileSize = 50;
#if ANDROID || MACOSX // parallelism will be added later for these platforms, not working well for now
        public const int MaximumNumberOfTasks = 1;
#else
        public const int MaximumNumberOfTasks = 2;
#endif
        private readonly object _locker = new object();
        private readonly IWaveFormRenderingService _waveFormRenderingService;
        private int _numberOfBitmapTasksRunning;
        private List<WaveFormTile> _tiles;
        private List<WaveFormBitmapRequest> _requests;

        public event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileBegunEvent;
        public event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileProgressEvent;
        public event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileEndedEvent;
        public event WaveFormRenderingService.LoadPeakFileEventHandler LoadedPeakFileSuccessfullyEvent;
        public event WaveFormRenderingService.GenerateWaveFormEventHandler GenerateWaveFormBitmapBegunEvent;
        public event WaveFormRenderingService.GenerateWaveFormEventHandler GenerateWaveFormBitmapEndedEvent;

        public bool IsEmpty { get { return _tiles.Count == 0; } }

        public WaveFormCacheService(IWaveFormRenderingService waveFormRenderingService)
        {
            _tiles = new List<WaveFormTile>();
            _requests = new List<WaveFormBitmapRequest>();
            _waveFormRenderingService = waveFormRenderingService;
            _waveFormRenderingService.GeneratePeakFileBegunEvent += HandleGeneratePeakFileBegunEvent;
            _waveFormRenderingService.GeneratePeakFileProgressEvent += HandleGeneratePeakFileProgressEvent;
            _waveFormRenderingService.GeneratePeakFileEndedEvent += HandleGeneratePeakFileEndedEvent;
            _waveFormRenderingService.LoadedPeakFileSuccessfullyEvent += HandleLoadedPeakFileSuccessfullyEvent;
            _waveFormRenderingService.GenerateWaveFormBitmapBegunEvent += HandleGenerateWaveFormBegunEvent;
            _waveFormRenderingService.GenerateWaveFormBitmapEndedEvent += HandleGenerateWaveFormEndedEvent;

            StartBitmapRequestProcessLoop();
        }

        private void HandleGeneratePeakFileBegunEvent(object sender, GeneratePeakFileEventArgs e)
        {
            if (GeneratePeakFileBegunEvent != null)
                GeneratePeakFileBegunEvent(sender, e);
        }

        private void HandleGeneratePeakFileProgressEvent(object sender, GeneratePeakFileEventArgs e)
        {
            if (GeneratePeakFileProgressEvent != null)
                GeneratePeakFileProgressEvent(sender, e);
        }

        private void HandleGeneratePeakFileEndedEvent(object sender, GeneratePeakFileEventArgs e)
        {
            if (GeneratePeakFileEndedEvent != null)
                GeneratePeakFileEndedEvent(sender, e);
        }

        private void HandleLoadedPeakFileSuccessfullyEvent(object sender, LoadPeakFileEventArgs e)
        {
            if (LoadedPeakFileSuccessfullyEvent != null)
                LoadedPeakFileSuccessfullyEvent(sender, e);
        }

        private void HandleGenerateWaveFormBegunEvent(object sender, GenerateWaveFormEventArgs e)
        {
            if (GenerateWaveFormBitmapBegunEvent != null)
                GenerateWaveFormBitmapBegunEvent(sender, e);
        }

        private void HandleGenerateWaveFormEndedEvent(object sender, GenerateWaveFormEventArgs e)
        {
            //Console.WriteLine("WaveFormCacheService - HandleGenerateWaveFormEndedEvent - e.Width: {0} e.Zoom: {1}", e.Width, e.Zoom);
            lock (_locker)
            {
                _numberOfBitmapTasksRunning--;
                var tile = new WaveFormTile()
                {
                    ContentOffset = new BasicPoint(e.OffsetX, 0),
                    Zoom = e.Zoom,
                    Image = e.Image
                };
                _tiles.Add(tile);
            }

            if (GenerateWaveFormBitmapEndedEvent != null)
                GenerateWaveFormBitmapEndedEvent(sender, e);
        }

        public void FlushCache()
        {
            lock (_locker)
            {
                foreach (var tile in _tiles)
                    tile.Image.Dispose();
                _tiles.Clear();
            }
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            FlushCache();
            _waveFormRenderingService.LoadPeakFile(audioFile);
        }

        public WaveFormTile GetTile(float x, float height, float waveFormWidth, float zoom)
        {
            WaveFormTile tile = null;
            lock (_locker)
            {
                var boundsBitmap = new BasicRectangle(x, 0, TileSize, height);
                var boundsWaveForm = new BasicRectangle(0, 0, waveFormWidth * zoom, height);
                var tiles = _tiles.Where(obj => obj.ContentOffset.X == x).ToList(); // not sure this works right when off zoom.
                if (tiles != null && tiles.Count > 0)
                {
                    // Check which bitmap to use for zoom
                    if (tiles.Count == 1)
                    {
                        tile = tiles[0];
                    }
                    else if(tiles.Count > 1)
                    {
                        // We don't want to scale down bitmaps

                        var orderedTiles = tiles.OrderBy(obj => obj.Zoom).ToList();
                        foreach(var thisTile in orderedTiles)
                        {
                            if (thisTile.Zoom <= zoom)
                            {
                                if (tile != null && thisTile.Zoom > tile.Zoom || tile == null)
                                {
                                    tile = thisTile;
                                }
                            }
                        }

                        // If we still haven't found a tile, take the first one.
                        if(tile == null)
                            tile = orderedTiles[0];

                        //Console.WriteLine("WaveFormCacheService - GetTile - Finding the right tile in cache; tile.Zoom: {0} -- x: {1} zoom: {2}", tile.Zoom, x, zoom);
                    }

                    // Do we need to request a bitmap with a zoom that's more appropriate?
                    if (zoom - tile.Zoom >= 2 || zoom - tile.Zoom <= -2)
                    {
                        //Console.WriteLine("WaveFormCacheService - Requesting a new bitmap (zoom doesn't match) - zoom: {0} tile.Zoom: {1} boundsBitmap: {2} boundsWaveForm: {3}", zoom, tile.Zoom, boundsBitmap, boundsWaveForm);
                        AddBitmapRequestToList(boundsBitmap, boundsWaveForm, zoom);
                    }

                    return tile;
                }
                else
                {
                    AddBitmapRequestToList(boundsBitmap, boundsWaveForm, zoom);
                }
            }
            return tile;
        }

        private void AddBitmapRequestToList(BasicRectangle boundsBitmap, BasicRectangle boundsWaveForm, float zoom)
        {
            var request = new WaveFormBitmapRequest()
            {
                DisplayType = WaveFormDisplayType.Stereo,
                BoundsBitmap = boundsBitmap,
                BoundsWaveForm = boundsWaveForm,
                Zoom = zoom
            };

            // Check if bitmap has already been requested in queue
            var existingRequest = _requests.FirstOrDefault(obj => 
                obj.BoundsBitmap.Equals(request.BoundsBitmap) && 
                obj.BoundsWaveForm.Equals(request.BoundsWaveForm) && 
                //obj.Zoom == request.Zoom);
                obj.Zoom >= request.Zoom - 2 && obj.Zoom <= request.Zoom + 2); // don't spam requests with slightly different zoom levels

            if (existingRequest == null)
            {
                Console.WriteLine("WaveFormCacheService - Adding bitmap request to queue - zoom: {0} boundsBitmap: {1} boundsWaveForm: {2}", zoom, boundsBitmap, boundsWaveForm);
                _requests.Add(request);                        
            }
            else
            {
                //Console.WriteLine("!!!!!!! SKIPPING REQUEST");
            }
        }

        public void StartBitmapRequestProcessLoop()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    //Console.WriteLine("WaveFormCacheService - BitmapRequestProcessLoop - Loop - requests.Count: {0} numberOfBitmapTasksRunning: {1}", _requests.Count, _numberOfBitmapTasksRunning);
                    var requestsToProcess = new List<WaveFormBitmapRequest>();
                    lock (_locker)
                    {
                        while (_requests.Count > 0 && _numberOfBitmapTasksRunning < MaximumNumberOfTasks)
                        {
                            _numberOfBitmapTasksRunning++;
                            var request = _requests[0];
                            requestsToProcess.Add(request);
                            _requests.RemoveAt(0);
                        }
                    }
                    foreach(var request in requestsToProcess)
                    {                        
                        //Console.WriteLine("WaveFormCacheService - BitmapRequestProcessLoop - Processing bitmap request - boundsBitmap: {0} boundsWaveForm: {1} zoom: {2} numberOfBitmapTasksRunning: {3}", request.BoundsBitmap, request.BoundsWaveForm, request.Zoom, _numberOfBitmapTasksRunning);
                        _waveFormRenderingService.RequestBitmap(request.DisplayType, request.BoundsBitmap, request.BoundsWaveForm, request.Zoom);
                    }

                    // Since the bitmap tiles are small enough to be generated under 20 ms, this basically makes it one task only.
                    // We need to loop requests until we hit the maximum.
                    Thread.Sleep(20);
                }
			}));
			thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
        }

        public class WaveFormBitmapRequest
        {
            public WaveFormDisplayType DisplayType { get; set; }
            public BasicRectangle BoundsBitmap { get; set; }
            public BasicRectangle BoundsWaveForm { get; set; }
            public float Zoom { get; set; }

            public WaveFormBitmapRequest()
            {                
                BoundsBitmap = new BasicRectangle();
                BoundsWaveForm = new BasicRectangle();
                Zoom = 1;
            }
        }

        public class WaveFormTile
        {
            public IDisposable Image { get; set; }
            public BasicPoint ContentOffset { get; set; }
            public float Zoom { get; set; }

            public WaveFormTile()
            {
                ContentOffset = new BasicPoint();
                Zoom = 1;
            }
        }
    }
}
