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

        private List<WaveFormTile> GetTilesInternal(WaveFormBitmapRequest request)
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
                {
                    tile.TileIndex = a;
                    if (!tiles.Contains(tile))
                        tiles.Add(tile);
                }
            }

            return tiles;
        }

        public List<WaveFormTile> GetTiles(float contentOffsetX, float zoom, BasicRectangle frame, BasicRectangle dirtyRect, WaveFormDisplayType displayType)
        {
            //Console.WriteLine(">>>>>>>>>>>>>>>>>> GETTILES <<<<<<<<<<<<<<<<");
            var tiles = new List<WaveFormTile>();

            var requestAtCurrentZoom = GetTilesRequest(contentOffsetX, zoom, frame, dirtyRect, displayType);
            //Console.WriteLine("====> RequestAtCurrentZoom - startTile: {0} endTile: {1}", requestAtCurrentZoom.StartTile, requestAtCurrentZoom.EndTile);

            var tilesAtCurrentZoom = GetTilesInternal(requestAtCurrentZoom);
            var missingTiles = GetMissingTilesRecursive(tilesAtCurrentZoom, requestAtCurrentZoom.StartTile, requestAtCurrentZoom.EndTile, contentOffsetX, zoom, frame, dirtyRect, displayType);
            tiles.AddRange(missingTiles);
            tiles.AddRange(tilesAtCurrentZoom);

            //var tilesOrdered = tiles.OrderBy(obj => obj.Zoom).ThenBy(obj => obj.ContentOffset.X).ToList();
            //return tilesOrdered;
            return tiles;
        }

        // Maybe it'd be just faster to get a series of tiles at different zooms, THEN merge the list together.
        private List<WaveFormTile> GetMissingTilesRecursive(List<WaveFormTile> tileSet, int tileSetStartTile, int tileSetEndTile, float contentOffsetX, float zoom, BasicRectangle frame, BasicRectangle dirtyRect, WaveFormDisplayType displayType)
        {
            var tiles = new List<WaveFormTile>();
            float floorZoom = (float)(zoom / Math.Floor(zoom));
            float tileSize = TileSize * floorZoom;
            var missingTileIndexes = FindMissingNumbersInSequence(tileSet.Select(x => x.TileIndex), tileSetStartTile, tileSetEndTile).ToList();
            if (missingTileIndexes.Count == 0)
                return tiles;

            //var missingTileIndexesString = missingTileIndexes.ConvertAll<string>((int i) => i.ToString() + " ");
            //Console.WriteLine("====> RequestAtCurrentZoom - MISSING TILES: {0}", String.Concat(missingTileIndexesString));

            // To make things faster, try to bundle requests together, when missing tiles are next to each other.
            // since missing tiles are usually next to each other, this will speed up dramatically.
            // Or do not even try to bundle them in separate lists, just make one single list with the lowest to highest value.

            int lowestIndex = missingTileIndexes.Min();
            int highestIndex = missingTileIndexes.Max();

            // Find a way to make this recursive.
            // Create a "dirty" rect that identifies the visible area at a different zoom level
            //var missingTileDirtyRect = new BasicRectangle(contentOffsetX / zoom, 0, frame.Width / zoom, frame.Height);
            float offsetX = lowestIndex * tileSize;
            float width = (highestIndex * tileSize) - offsetX;
            var missingTileDirtyRect = new BasicRectangle(offsetX / zoom, 0, width / zoom, frame.Height);
            var requestForMissingTile = GetTilesRequest(0, 1, frame, missingTileDirtyRect, displayType);
            var missingTiles = GetTilesInternal(requestForMissingTile);
            tiles.AddRange(missingTiles);
            //Console.WriteLine("=======> RequestForMissingTile - startTile: {0} endTile: {1} dirtyRect: {2}", requestForMissingTile.StartTile, requestForMissingTile.EndTile, missingTileDirtyRect);            

            return tiles;
        }

        private IEnumerable<int> FindMissingNumbersInSequence(IEnumerable<int> values, int start, int end)
        {
            return Enumerable.Range(start, end - start).Except(values);
        }
    }
}
