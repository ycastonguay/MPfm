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
    public class WaveFormCacheService : IWaveFormCacheService
    {
        public const int TileSize = 50;
        public const int MaxNumberOfRequests = 20;
        #if MACOSX
        public const int MaximumNumberOfTasks = 1;
        #else
        public const int MaximumNumberOfTasks = 2;
        #endif
        private readonly object _lockerCache = new object();
        private readonly object _lockerRequests = new object();
        private readonly object _lockerTiles = new object();
        private readonly IWaveFormRenderingService _waveFormRenderingService;
        private readonly ITileCacheService _cacheService;
        private int _numberOfBitmapTasksRunning;
        private float _lastZoom;
        private List<WaveFormBitmapRequest> _requests;

        public event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileBegunEvent;
        public event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileProgressEvent;
        public event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileEndedEvent;
        public event WaveFormRenderingService.LoadPeakFileEventHandler LoadedPeakFileSuccessfullyEvent;
        public event WaveFormRenderingService.GenerateWaveFormEventHandler GenerateWaveFormBitmapBegunEvent;
        public event WaveFormRenderingService.GenerateWaveFormEventHandler GenerateWaveFormBitmapEndedEvent;

        public WaveFormCacheService(IWaveFormRenderingService waveFormRenderingService, ITileCacheService cacheService)
        {
            _requests = new List<WaveFormBitmapRequest>();
            _cacheService = cacheService;
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
                //_tiles.Add(tile);
                _cacheService.AddTile(tile, false);
            }

            if (GenerateWaveFormBitmapEndedEvent != null)
                GenerateWaveFormBitmapEndedEvent(sender, e);
        }

        public void FlushCache()
        {
            lock (_lockerRequests)
            {
                _requests.Clear();
            }

            _cacheService.Flush();
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            FlushCache();
            _waveFormRenderingService.LoadPeakFile(audioFile);
        }

        public List<WaveFormTile> GetTiles(WaveFormBitmapRequest request)
        {
            if (request.Zoom != _lastZoom && request.Zoom > 1)
            {
                lock (_lockerCache)
                {
                    // Bug: when requesting the smaller tiles, the zoom changes
                    //Console.WriteLine("WaveFormCacheService - Zoom has changed - lastZoom: {0} zoom: {1}", _lastZoom, request.Zoom);
                    _lastZoom = request.Zoom;
                    _cacheService.Flush();
                }
            }

            //float coveredAreaX = 0;
            float zoomThreshold = (float)Math.Floor(request.Zoom);
            var boundsWaveFormAdjusted = new BasicRectangle(0, 0, request.BoundsWaveForm.Width * zoomThreshold, request.BoundsWaveForm.Height);
            var tiles = new List<WaveFormTile>();
            //List<WaveFormTile> previouslyAvailableTiles = new List<WaveFormTile>();
            for (int a = request.StartTile; a < request.EndTile; a++)
            {
                float tileX = a * request.TileSize;
                WaveFormTile tile = null;
                WaveFormTile cachedTile = _cacheService.GetTile(tileX, request.IsScrollBar);
                if (cachedTile != null)
                {
                    //Console.WriteLine(">>>>>>>>>> Taking cached tile!");
                    tile = cachedTile;
                } 
                else
                {
                    // This is a hot line, and needs to be avoided as much as possible.
                    // the problem is that tiles vary in time in quality. 
                    // maybe as a first, when a tile at the right zoom is available, cache it locally so it isn't necessary to call the algo again.
                    var availableTiles = _cacheService.GetTilesForPosition(tileX, request.Zoom);
                    var boundsBitmap = new BasicRectangle(tileX, 0, TileSize, request.BoundsWaveForm.Height);
                    if (availableTiles != null && availableTiles.Count > 0)
                    {
                        // TEMP: Add every tile for zoom == 100% (TESTING) -- This fixes the empty areas and proves the coveredAreaX technique doesn't work.
                        var tileLowRes = availableTiles.FirstOrDefault(x => x.Zoom == 1);
                        if (tileLowRes != null && !tiles.Contains(tileLowRes))
                            tiles.Add(tileLowRes);

                        // Get the tile with the zoom that is the closest to the current zoom threshold 
                        tile = TileHelper.GetOptimalTileAtZoom(availableTiles, zoomThreshold);

                        // If we could not find a tile at this zoom level, we need to generate one 
                        if (tile.Zoom != zoomThreshold)
                        {
                            //Console.WriteLine("WaveFormCacheService - Requesting a new bitmap (zoom doesn't match) - zoomThreshold: {0} tile.Zoom: {1} boundsBitmap: {2} boundsWaveForm: {3}", zoomThreshold, tile.Zoom, boundsBitmap, request.BoundsWaveForm);
                            AddBitmapRequestToList(boundsBitmap, boundsWaveFormAdjusted, zoomThreshold, request.DisplayType);
                        } 
                        else
                        {
                            _cacheService.AddTile(tile, request.IsScrollBar);
                        }
                    } 
                    else
                    {
                        // We need to request a new bitmap at this zoom threshold because there are no bitmaps available (usually zoom @ 100%)
                        //Console.WriteLine("WaveFormCacheService - Requesting a new bitmap - zoom: {0} zoomThreshold: {1} boundsWaveForm: {2}", zoomThreshold, boundsBitmap, request.BoundsWaveForm);
                        AddBitmapRequestToList(boundsBitmap, boundsWaveFormAdjusted, zoomThreshold, request.DisplayType);
                    }
                }

                //Console.WriteLine("WaveFormCacheService - GetTiles - tile {0} x: {1} Zoom: {2} // tileFound: {3} tile.X: {4} tile.Zoom: {5}", a, tileX, request.Zoom, tile == null, tile != null ? tile.ContentOffset.X : -1, tile != null ? tile.Zoom : -1);
                if (tile != null)
                {
                    // Calculate the new covered area (adjusted with the zoom delta)
                    //float currentTileDeltaZoom = request.Zoom/tile.Zoom;
                    //float currentTileX = tile.ContentOffset.X*currentTileDeltaZoom;
                    //float currentTileWidth = request.TileSize*currentTileDeltaZoom;

                    //// Check if the new tile leaves an empty area behind
                    //if (coveredAreaX < tile.ContentOffset.X)
                    //{
                    //    //Console.WriteLine("[...] WaveFormCacheService - GetTiles - An empty area has been found - coveredAreaX: {0} tile.ContentOffset.X: {1}", coveredAreaX, currentTileX);
                    //    //var tilesToFillEmptyArea = GetAvailableTilesToFillBounds(tileX, zoom, new BasicRectangle(coveredAreaX, 0, tile.ContentOffset.X - coveredAreaX, boundsWaveForm.Height));
                    //    var tilesToFillEmptyArea = GetAvailableTilesToFillBounds(zoom, new BasicRectangle(coveredAreaX, 0, tile.ContentOffset.X - coveredAreaX, boundsWaveForm.Height));
                    //    //Console.WriteLine("[...] $$$$$$$$$$$$ WaveFormCacheService - GetTiles - tilesToFillEmptyArea.Count: {0}", tilesToFillEmptyArea.Count);
                    //    //foreach (var daTile in tilesToFillEmptyArea)
                    //    //    Console.WriteLine("        .....>>>>>> tile.ContentOffset.X: {0} tile.Zoom: {1}", daTile.ContentOffset.X, daTile.Zoom);    

                    //    // Go through previously available tiles to find a tile to cover the empty area.
                    //    // This should return at least one tile (there should always be one around at zoom=100% except for the initial loading)
                    //    WaveFormTile tileToCoverEmptyArea = null;
                    //    //foreach (var previouslyAvailableTile in previouslyAvailableTiles)
                    //    //{
                    //    //    float previousTileDeltaZoom = zoom/previouslyAvailableTile.Zoom;
                    //    //    float previousTileX = previouslyAvailableTile.ContentOffset.X * previousTileDeltaZoom;
                    //    //    float previousTileWidth = tileSize * previousTileDeltaZoom;
                    //    //    if (previousTileX < coveredAreaX &&
                    //    //        previousTileX + previousTileWidth >= currentTileX)
                    //    //    {
                    //    //        tileToCoverEmptyArea = previouslyAvailableTile;
                    //    //        Console.WriteLine("[...] !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! WaveFormCacheService - GetTiles - ==> A tile has been found to cover the empty area - tileToCover.ContentOffset.X: {0} tileWidth: {1} tile.Zoom: {2}", previousTileX, previousTileWidth, tileToCoverEmptyArea.Zoom);
                    //    //        break;
                    //    //    }
                    //    //}

                    //    //tileToCoverEmptyArea = tilesToFillEmptyArea.OrderByDescending(x => x.Zoom).FirstOrDefault();
                    //    tileToCoverEmptyArea = tilesToFillEmptyArea.FirstOrDefault(x => x.Zoom == 1);

                    //    // We found a tile to cover the area. If not, this should be only when refreshing the wave form for the first time @ 100%
                    //    if (tileToCoverEmptyArea != null)
                    //    {
                    //        Console.WriteLine("---> Adding tile to fill empty - tile.X: {0} tile.Zoom: {1} -- {2}", tileToCoverEmptyArea.ContentOffset.X, tileToCoverEmptyArea.Zoom, DateTime.Now);
                    //        tiles.Add(tileToCoverEmptyArea);
                    //    }
                    //    else
                    //    {
                    //        // The problem is that there are sometimes a tile could not be found the previous list... is that really a good source of info?
                    //        // maybe try the previous list and if not, fall back to the 100% zoom.
                    //        // or actually make a linq query similar to the one with adjusted content offset x.
                    //        Console.WriteLine("[!!!] WARNING: WaveFormCacheService - GetTiles - An empty area could not be filled by a tile!");
                    //    }
                    //}

                    // Update the covered area position after trying to fill any empty areas left behind
                    // Note: There are still empty areas, this might not be the best way to make sure areas are all filled... 
                    //coveredAreaX = currentTileX + currentTileWidth;

                    // Keep the available tiles from the last index so we can search through this list to cover an empty area if needed
                    //previouslyAvailableTiles = availableTiles;

                    // Add tile to list of tiles to draw (TO DO: Check for existing tiles with the same zoom + offset
                    if(!tiles.Contains(tile))
                        tiles.Add(tile);
                }
            }

            // Order tiles by zoom and then by content offset x; this makes sure that the tiles with the nearest zoom level get drawn on top of farther zoom levels
            // maybe replace this linq query by inserting the tiles in the list in the right order (at tiles.Add(tile) just up from here)
            // Also use Distinct to prevent drawing the same tile multiple times
            // B U G: This might crash if a tile is removed from the list....
            var tilesOrdered = tiles.OrderBy(obj => obj.Zoom).ThenBy(obj => obj.ContentOffset.X).ToList();
            return tilesOrdered;
        }

        public WaveFormTile GetTile(float x, float height, float waveFormWidth, float zoom)
        {
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
            WaveFormTile tile = null;
            float zoomThreshold = (float) Math.Floor(zoom);
            var boundsBitmap = new BasicRectangle(x, 0, TileSize, height);
            var boundsWaveForm = new BasicRectangle(0, 0, waveFormWidth * zoomThreshold, height);

            var tiles = _cacheService.GetTilesForPosition(x, zoom);
            if (tiles != null && tiles.Count > 0)
            {
                // Get the tile with the zoom that is the closest to the current zoom threshold 
                tile = TileHelper.GetOptimalTileAtZoom(tiles, zoomThreshold);

                // If we could not find a tile at this zoom level, we need to generate one 
                if (tile.Zoom != zoomThreshold)
                {
                    //Console.WriteLine("WaveFormCacheService - Requesting a new bitmap (zoom doesn't match) - zoom: {0} tile.Zoom: {1} boundsBitmap: {2} boundsWaveForm: {3}", zoom, tile.Zoom, boundsBitmap, boundsWaveForm);
                    AddBitmapRequestToList(boundsBitmap, boundsWaveForm, zoomThreshold, WaveFormDisplayType.Stereo);
                }

                return tile;
            }
            else
            {
                // We need to request a new bitmap at this zoom threshold because there are no bitmaps available (usually zoom @ 100%)
                //Console.WriteLine("WaveFormCacheService - Requesting a new bitmap - zoom: {0} boundsBitmap: {1} boundsWaveForm: {2}", zoomThreshold, boundsBitmap, boundsWaveForm);
                AddBitmapRequestToList(boundsBitmap, boundsWaveForm, zoomThreshold, WaveFormDisplayType.Stereo);
            }

            //stopwatch.Stop();
            //Console.WriteLine("WaveFormCacheService - GetTile - stopwatch: {0} ms", stopwatch.ElapsedMilliseconds);
            return tile;
        }

        private void AddBitmapRequestToList(BasicRectangle boundsBitmap, BasicRectangle boundsWaveForm, float zoom, WaveFormDisplayType displayType)
        {
            // Make sure we don't slow down GetTile() by creating a task and running LINQ queries on another thread
            Task.Factory.StartNew(() =>
                {
                    var request = new WaveFormBitmapRequest()
                    {
                        DisplayType = displayType,
                        BoundsBitmap = boundsBitmap,
                        BoundsWaveForm = boundsWaveForm,
                        Zoom = zoom
                    };

                    // Check if a tile already exists
                    WaveFormTile existingTile = null;
                    lock (_lockerTiles)
                    {
                        //existingTile = _tiles.FirstOrDefault(obj => obj.ContentOffset.X == boundsBitmap.X && obj.Zoom == zoom);
                        existingTile = _cacheService.GetTile(boundsBitmap.X, zoom);
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

                            //Console.WriteLine("............. PULSING");
                            Monitor.Pulse(_lockerRequests);                        
                        }
                    }
                });
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
