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
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Services.Events;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Services.Objects;
using Sessions.Sound.AudioFiles;
using Sessions.GenericControls.Helpers;

namespace Sessions.GenericControls.Services
{
    public class WaveFormEngineService : IWaveFormEngineService
    {
        public const int TileSize = 50;
        private readonly IWaveFormRenderingService _waveFormRenderingService;
        private readonly IWaveFormRequestService _requestService;
        private readonly ITileCacheService _cacheService;

        public event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileBegunEvent;
        public event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileProgressEvent;
        public event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileEndedEvent;
        public event WaveFormRenderingService.LoadPeakFileEventHandler LoadedPeakFileSuccessfullyEvent;
        public event WaveFormRenderingService.GenerateWaveFormEventHandler GenerateWaveFormBitmapEndedEvent;

        public WaveFormEngineService(IWaveFormRenderingService waveFormRenderingService, ITileCacheService cacheService, IWaveFormRequestService requestService)
        {
            _cacheService = cacheService;
            _waveFormRenderingService = waveFormRenderingService;
            _waveFormRenderingService.GeneratePeakFileBegunEvent += HandleGeneratePeakFileBegunEvent;
            _waveFormRenderingService.GeneratePeakFileProgressEvent += HandleGeneratePeakFileProgressEvent;
            _waveFormRenderingService.GeneratePeakFileEndedEvent += HandleGeneratePeakFileEndedEvent;
            _waveFormRenderingService.LoadedPeakFileSuccessfullyEvent += HandleLoadedPeakFileSuccessfullyEvent;
            _waveFormRenderingService.GenerateWaveFormBitmapEndedEvent += HandleGenerateWaveFormEndedEvent;
            _requestService = requestService;
            _requestService.GenerateWaveFormBitmapEndedEvent += HandleGenerateWaveFormBitmapEndedEvent;
        }

        private void HandleGenerateWaveFormBitmapEndedEvent(object sender, GenerateWaveFormEventArgs e)
        {
            //Console.WriteLine("REQUEST SERVICE DONE");
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

        private void HandleGenerateWaveFormEndedEvent(object sender, GenerateWaveFormEventArgs e)
        {
            //Console.WriteLine("WaveFormCacheService - HandleGenerateWaveFormEndedEvent - e.Width: {0} e.Zoom: {1}", e.Width, e.Zoom);
//            lock (_lockerTiles)
//            {
//                _numberOfBitmapTasksRunning--;
//                var tile = new WaveFormTile()
//                {
//                    ContentOffset = new BasicPoint(e.OffsetX, 0),
//                    Zoom = e.Zoom,
//                    Image = e.Image
//                };
//                _cacheService.AddTile(tile, e.IsScrollBar);
//            }

            if (GenerateWaveFormBitmapEndedEvent != null)
                GenerateWaveFormBitmapEndedEvent(sender, e);
        }

        public void FlushCache()
        {
            _requestService.Flush();
            _cacheService.Flush();
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            FlushCache();
            _waveFormRenderingService.LoadPeakFile(audioFile);
        }

        public WaveFormBitmapRequest GetTilesRequest(float offsetX, float zoom, BasicRectangle controlFrame, BasicRectangle dirtyRect, WaveFormDisplayType displayType)
        {
            int startDirtyTile = TileHelper.GetStartDirtyTile(offsetX, dirtyRect.X, zoom, TileSize);
            int endDirtyTile = TileHelper.GetEndDirtyTile(offsetX, dirtyRect.X, dirtyRect.Width, zoom, TileSize) + 1;

            //Console.WriteLine("GetTilesRequest --> offsetX: {0} zoom: {1} startTile: {2} endTile: {3} dirtyRect: {4}", offsetX, zoom, startDirtyTile, endDirtyTile, dirtyRect);
            var request = new WaveFormBitmapRequest()
            {
                StartTile = startDirtyTile,
                EndTile = endDirtyTile,
                TileSize = TileSize,
                BoundsWaveForm = controlFrame,
                Zoom = zoom,
                DisplayType = displayType
            };

            return request;
        }

        public List<WaveFormTile> GetTiles(WaveFormBitmapRequest request)
        {
            float zoomThreshold = (float)Math.Floor(request.Zoom);
            var boundsWaveFormAdjusted = new BasicRectangle(0, 0, request.BoundsWaveForm.Width * zoomThreshold, request.BoundsWaveForm.Height);
            var tiles = new List<WaveFormTile>();
            for (int a = request.StartTile; a < request.EndTile; a++)
            {
                float tileX = a * request.TileSize;
                //Console.WriteLine("<---------------->");
                //Console.WriteLine("GetTiles --> Requesting tile from cache - a: {0} tileX: {1} zoom: {2}", a, tileX, zoomThreshold);
                var tile = _cacheService.GetTile(tileX, zoomThreshold, request.IsScrollBar);
                //if (tile != null)
                    //Console.WriteLine("WaveFormEngineService - GetTiles - Found tile in cache! tileOffsetX: {0} tileZoom: {1}", tile.ContentOffset.X, tile.Zoom);

                if (tile == null)
                {
                    var boundsBitmap = new BasicRectangle(tileX, 0, TileSize, request.BoundsWaveForm.Height);
                    _requestService.RequestBitmap(boundsBitmap, boundsWaveFormAdjusted, zoomThreshold, request.DisplayType);
                }

                //Console.WriteLine("WaveFormCacheService - GetTiles - tile {0} x: {1} Zoom: {2} // tileFound: {3} tile.X: {4} tile.Zoom: {5}", a, tileX, request.Zoom, tile == null, tile != null ? tile.ContentOffset.X : -1, tile != null ? tile.Zoom : -1);
                if (tile != null)
                    if(!tiles.Contains(tile))
                        tiles.Add(tile);
            }

            //var tilesOrdered = tiles.OrderBy(obj => obj.Zoom).ThenBy(obj => obj.ContentOffset.X).ToList();
            //return tilesOrdered;
            return tiles;
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
                    _requestService.RequestBitmap(boundsBitmap, boundsWaveForm, zoomThreshold, WaveFormDisplayType.Stereo);
                }

                return tile;
            }
            else
            {
                // We need to request a new bitmap at this zoom threshold because there are no bitmaps available (usually zoom @ 100%)
                //Console.WriteLine("WaveFormCacheService - Requesting a new bitmap - zoom: {0} boundsBitmap: {1} boundsWaveForm: {2}", zoomThreshold, boundsBitmap, boundsWaveForm);
                _requestService.RequestBitmap(boundsBitmap, boundsWaveForm, zoomThreshold, WaveFormDisplayType.Stereo);
            }

            //stopwatch.Stop();
            //Console.WriteLine("WaveFormCacheService - GetTile - stopwatch: {0} ms", stopwatch.ElapsedMilliseconds);
            return tile;
        }            
    }
}
