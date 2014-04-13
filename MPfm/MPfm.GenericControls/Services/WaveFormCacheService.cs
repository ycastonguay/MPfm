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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Services.Events;
using MPfm.GenericControls.Services.Interfaces;
using MPfm.GenericControls.Services.Objects;
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
        private readonly object _lockerRequests = new object();
        private readonly object _lockerTiles = new object();
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
            lock (_lockerTiles)
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
            lock (_lockerTiles)
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
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
            WaveFormTile tile = null;
            List<WaveFormTile> tiles = null;
            float zoomThreshold = (float) Math.Floor(zoom); //(float) Math.Round(zoom);
            var boundsBitmap = new BasicRectangle(x, 0, TileSize, height);
            var boundsWaveForm = new BasicRectangle(0, 0, waveFormWidth * zoomThreshold, height);

            lock (_lockerTiles)
            {
                //if(x == TileSize * 2 && zoomThreshold > 1)
                    //Debugger.Break();

                // |.......|.......|.......|.......|.......| -- 100%
                // |...|...|...|...|...|...|...|...|...|...| -- 200%
                // |.|.|.|.|.|.|.|.|.|.|.|.|.|.|.|.|.|.|.|.| -- 300%
                // ex: slice at x:20 and zoom:100% is placed at x:40 for zoom:200%
                // b u g: there's sometimes more than one bitmap cache per offsetx/zoom!
                // b u g : this doesn't return a bitmap that is available for the previous threshold
                tiles = _tiles.Where(obj => obj.ContentOffset.X == x * (zoomThreshold / obj.Zoom)).ToList();
                //tiles = _tiles.Where(obj => obj.ContentOffset.X == x * (zoomThreshold / obj.Zoom) && obj.Zoom == 1).ToList();
                //tiles = _tiles.Where(obj => obj.ContentOffset.X == x * (1 / (zoomThreshold / obj.Zoom)) && obj.Zoom == 1).ToList();
            }

            if (tiles != null && tiles.Count > 0)
            {
                // Check which bitmap to use for zoom
                if (tiles.Count == 1)
                {
                    tile = tiles[0];
                }
                else if (tiles.Count > 1)
                {
                    // We don't want to scale down bitmaps
                    var orderedTiles = tiles.OrderBy(obj => obj.Zoom).ToList();
                    tile = orderedTiles.Count > 0 ? orderedTiles[0] : null;
                    foreach (var thisTile in orderedTiles)
                    {
                        if (thisTile.Zoom <= zoomThreshold)
                        {
                            if (tile != null && thisTile.Zoom > tile.Zoom || tile == null)
                            {
                                tile = thisTile;
                            }
                        }
                    }

                    // If we still haven't found a tile, take the first one.
                    if (tile == null)
                        tile = orderedTiles[0];

                    //Console.WriteLine("WaveFormCacheService - GetTile - Finding the right tile in cache; tile.Zoom: {0} -- x: {1} zoom: {2}", tile.Zoom, x, zoom);
                }

                // Do we need to request a bitmap with a zoom that's more appropriate?
                if (tile.Zoom != zoomThreshold)
                {
                    //Console.WriteLine("WaveFormCacheService - Requesting a new bitmap (zoom doesn't match) - zoom: {0} tile.Zoom: {1} boundsBitmap: {2} boundsWaveForm: {3}", zoom, tile.Zoom, boundsBitmap, boundsWaveForm);

                    // b u g: This makes the background flash between thresholds
                    AddBitmapRequestToList(boundsBitmap, boundsWaveForm, zoomThreshold);
                }

                return tile;
            }
            else
            {
                //Console.WriteLine("WaveFormCacheService - Requesting a new bitmap - zoom: {0} boundsBitmap: {1} boundsWaveForm: {2}", zoomThreshold, boundsBitmap, boundsWaveForm);
                AddBitmapRequestToList(boundsBitmap, boundsWaveForm, zoomThreshold);
            }

            //stopwatch.Stop();
            //Console.WriteLine("WaveFormCacheService - GetTile - stopwatch: {0} ms", stopwatch.ElapsedMilliseconds);
            return tile;
        }

        private void AddBitmapRequestToList(BasicRectangle boundsBitmap, BasicRectangle boundsWaveForm, float zoom)
        {
            //var thread = new Thread(new ThreadStart(() =>
            //{
            Task.Factory.StartNew(() =>
            {
                var request = new WaveFormBitmapRequest()
                {
                    DisplayType = WaveFormDisplayType.Stereo,
                    BoundsBitmap = boundsBitmap,
                    BoundsWaveForm = boundsWaveForm,
                    Zoom = zoom
                };

                lock (_lockerRequests)
                {
                    //// Check if bitmap has already been requested in queue
                    var existingRequest = _requests.FirstOrDefault(obj =>
                        obj.BoundsBitmap.Equals(request.BoundsBitmap) &&
                        obj.BoundsWaveForm.Equals(request.BoundsWaveForm) &&
                        obj.Zoom == request.Zoom);
                    //WaveFormBitmapRequest existingRequest = null;

                    if (existingRequest == null)
                    {
                        //Console.WriteLine("WaveFormCacheService - Adding bitmap request to queue - zoom: {0} boundsBitmap: {1} boundsWaveForm: {2}", zoom, boundsBitmap, boundsWaveForm);
                        _requests.Add(request);
                    }
                    else
                    {
                        //Console.WriteLine("!!!!!!! SKIPPING REQUEST");
                    }
                }
            });
            //}));
            //thread.IsBackground = true;
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();
        }

        public void StartBitmapRequestProcessLoop()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    //Console.WriteLine("WaveFormCacheService - BitmapRequestProcessLoop - Loop - requests.Count: {0} numberOfBitmapTasksRunning: {1}", _requests.Count, _numberOfBitmapTasksRunning);
                    var requestsToProcess = new List<WaveFormBitmapRequest>();
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
                    }
                    foreach(var request in requestsToProcess)
                    {                        
                        //Console.WriteLine("WaveFormCacheService - BitmapRequestProcessLoop - Processing bitmap request - boundsBitmap: {0} boundsWaveForm: {1} zoom: {2} numberOfBitmapTasksRunning: {3}", request.BoundsBitmap, request.BoundsWaveForm, request.Zoom, _numberOfBitmapTasksRunning);
                        _waveFormRenderingService.RequestBitmap(request);
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

    }
}
