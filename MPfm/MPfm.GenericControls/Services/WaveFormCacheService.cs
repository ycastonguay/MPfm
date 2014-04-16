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
        public const int MaxNumberOfRequests = 20;
#if ANDROID // parallelism will be added later for these platforms, not working well for now
        public const int MaximumNumberOfTasks = 2;
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

        public List<WaveFormTile> GetTiles(int startTile, int endTile, int tileSize, BasicRectangle boundsWaveForm, float zoom)
        {
            float coveredAreaX = 0;
            var tiles = new List<WaveFormTile>();
            for (int a = startTile; a < endTile; a++)
            {
                float tileX = a*tileSize;
                var tile = GetTile(tileX, boundsWaveForm.Height, boundsWaveForm.Width, zoom);

                //Console.WriteLine("WaveFormCacheService - GetTiles - tile {0} x: {1} Zoom: {2} // tileFound: {3} tile.X: {4} tile.Zoom: {5}", a, tileX, zoom, tile == null, tile != null ? tile.ContentOffset.X : -1, tile != null ? tile.Zoom : -1);
                if (tile != null)
                    tiles.Add(tile);
            }

            // Order tiles by zoom and then by content offset x; this makes sure that the tiles with the nearest zoom level get drawn on top of farther zoom levels
            // maybe replace this linq query by inserting the tiles in the list in the right order (at tiles.Add(tile) just up from here)
            // Also use Distinct to prevent drawing the same tile multiple times
            var tilesOrdered = tiles.Distinct().OrderBy(obj => obj.Zoom).ThenBy(obj => obj.ContentOffset.X).ToList();
            return tilesOrdered;
        }

        public WaveFormTile GetTile(float x, float height, float waveFormWidth, float zoom)
        {
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
            WaveFormTile tile = null;
            List<WaveFormTile> tiles = null;
            float zoomThreshold = (float) Math.Floor(zoom);
            var boundsBitmap = new BasicRectangle(x, 0, TileSize, height);
            var boundsWaveForm = new BasicRectangle(0, 0, waveFormWidth * zoomThreshold, height);

            lock (_lockerTiles)
            {
                // Try to get a bitmap tile that covers the area we're interested in. The content offset x must be adjusted depending on the zoom level because a tile with
                // a lower zoom might cover a very large area when adjusted to the new zoom; we need to draw the bitmap tile with a large offset (most of the bitmap is off screen)
                tiles = _tiles.Where(obj => obj.ContentOffset.X == obj.GetAdjustedContentOffsetForZoom(x, TileSize, zoomThreshold)).ToList();
            }

            if (tiles != null && tiles.Count > 0)
            {
                // Get the tile with the zoom that is the closest to the current zoom threshold 
                tile = GetOptimalTileAtZoom(tiles, zoomThreshold);

                // If we could not find a tile at this zoom level, we need to generate one 
                if (tile.Zoom != zoomThreshold)
                {
                    //Console.WriteLine("WaveFormCacheService - Requesting a new bitmap (zoom doesn't match) - zoom: {0} tile.Zoom: {1} boundsBitmap: {2} boundsWaveForm: {3}", zoom, tile.Zoom, boundsBitmap, boundsWaveForm);
                    AddBitmapRequestToList(boundsBitmap, boundsWaveForm, zoomThreshold);
                }

                return tile;
            }
            else
            {
                // We need to request a new bitmap at this zoom threshold because there are no bitmaps available (usually zoom @ 100%)
                //Console.WriteLine("WaveFormCacheService - Requesting a new bitmap - zoom: {0} boundsBitmap: {1} boundsWaveForm: {2}", zoomThreshold, boundsBitmap, boundsWaveForm);
                AddBitmapRequestToList(boundsBitmap, boundsWaveForm, zoomThreshold);
            }

            //stopwatch.Stop();
            //Console.WriteLine("WaveFormCacheService - GetTile - stopwatch: {0} ms", stopwatch.ElapsedMilliseconds);
            return tile;
        }

        private void AddBitmapRequestToList(BasicRectangle boundsBitmap, BasicRectangle boundsWaveForm, float zoom)
        {
            // Make sure we don't slow down GetTile() by creating a task and running LINQ queries on another thread
            Task.Factory.StartNew(() =>
            {
                var request = new WaveFormBitmapRequest()
                {
                    DisplayType = WaveFormDisplayType.Stereo,
                    BoundsBitmap = boundsBitmap,
                    BoundsWaveForm = boundsWaveForm,
                    Zoom = zoom
                };

                // Check if a tile already exists
                WaveFormTile existingTile = null;
                lock (_lockerTiles)
                {
                    existingTile = _tiles.FirstOrDefault(obj => obj.ContentOffset.X == boundsBitmap.X && obj.Zoom == zoom);
                }

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
                    }
                }
            });
        }

        private WaveFormTile GetOptimalTileAtZoom(IEnumerable<WaveFormTile> tiles, float zoom)
        {
            // We don't want to scale down bitmaps, it is more CPU intensive than scaling up
            var orderedTiles = tiles.OrderBy(obj => obj.Zoom).ToList();
            var tile = orderedTiles.Count > 0 ? orderedTiles[0] : null;
            foreach (var thisTile in orderedTiles)
            {
                if (thisTile.Zoom <= zoom)
                {
                    //if (tile != null && thisTile.Zoom > tile.Zoom || tile == null)
                    if (thisTile.Zoom > tile.Zoom)
                    {
                        tile = thisTile;
                    }
                }
            }
            return tile;
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

                    foreach (var request in requestsToProcess)
                    {
                        //Console.WriteLine("WaveFormCacheService - BitmapRequestProcessLoop - Processing bitmap request - boundsBitmap: {0} boundsWaveForm: {1} zoom: {2} numberOfBitmapTasksRunning: {3}", request.BoundsBitmap, request.BoundsWaveForm, request.Zoom, _numberOfBitmapTasksRunning);
                        _waveFormRenderingService.RequestBitmap(request); // ThreadQueueWorkItem will manage a thread pool
                    }
                    
                    Thread.Sleep(50);
                }
			}));
			thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
        }

    }
}
