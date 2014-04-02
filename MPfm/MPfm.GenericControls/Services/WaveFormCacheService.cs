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
using System.Collections.Generic;
using System.Linq;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Services.Events;
using MPfm.GenericControls.Services.Interfaces;
using MPfm.Sound.AudioFiles;

namespace MPfm.GenericControls.Services
{
    public class WaveFormCacheService : IWaveFormCacheService
    {
        public const int TileSize = 20;
        private readonly object _locker = new object();
        private readonly IWaveFormRenderingService _waveFormRenderingService;
        private List<WaveFormTile> _tiles;
        private bool _isGeneratingWaveForm;

        public event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileBegunEvent;
        public event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileProgressEvent;
        public event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileEndedEvent;
        public event WaveFormRenderingService.LoadPeakFileEventHandler LoadedPeakFileSuccessfullyEvent;
        public event WaveFormRenderingService.GenerateWaveFormEventHandler GenerateWaveFormBitmapBegunEvent;
        public event WaveFormRenderingService.GenerateWaveFormEventHandler GenerateWaveFormBitmapEndedEvent;

        public WaveFormCacheService(IWaveFormRenderingService waveFormRenderingService)
        {
            _tiles = new List<WaveFormTile>();
            _waveFormRenderingService = waveFormRenderingService;
            _waveFormRenderingService.GeneratePeakFileBegunEvent += HandleGeneratePeakFileBegunEvent;
            _waveFormRenderingService.GeneratePeakFileProgressEvent += HandleGeneratePeakFileProgressEvent;
            _waveFormRenderingService.GeneratePeakFileEndedEvent += HandleGeneratePeakFileEndedEvent;
            _waveFormRenderingService.LoadedPeakFileSuccessfullyEvent += HandleLoadedPeakFileSuccessfullyEvent;
            _waveFormRenderingService.GenerateWaveFormBitmapBegunEvent += HandleGenerateWaveFormBegunEvent;
            _waveFormRenderingService.GenerateWaveFormBitmapEndedEvent += HandleGenerateWaveFormEndedEvent;
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
            Console.WriteLine("WaveFormCacheService - HandleGenerateWaveFormEndedEvent - e.Width: {0} e.Zoom: {1}", e.Width, e.Zoom);
            lock (_locker)
            {
                _isGeneratingWaveForm = false;
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
            // The consumer knows how many tiles are in the wave form.
            // Should the x parameter be a multiple of tile size
            // or return the tile at that position (that would require returning the offset inside the tile)

            WaveFormTile tile = null;
            lock (_locker)
            {
                var rect = new BasicRectangle(x, 0, TileSize, height);
                var tiles = _tiles.Where(obj => obj.ContentOffset.X == x).ToList();
                if (tiles != null && tiles.Count > 0)
                {
                    // Check which bitmap to use for zoom
                    if (tiles.Count == 1)
                    {
                        tile = tiles[0];
                    }
                    else
                    {
                        // TO DO: Find a way to select the one that matches the nearest zoom
                        tile = tiles[0];
                    }

                    // Do we need to request a bitmap with a zoom that's more appropriate?
                    if (zoom - tile.Zoom >= 2 || zoom - tile.Zoom <= -2)
                    {
                        // Request a new bitmap
                        //Console.WriteLine("WaveFormCacheService - Requesting a new bitmap - rect: {0} zoom: {1}", rect, zoom);
                        _waveFormRenderingService.RequestBitmap(WaveFormDisplayType.Stereo, rect, new BasicRectangle(0, 0, waveFormWidth, height), zoom);
                    }

                    return tile;
                }
                else
                {
                    // Request a new bitmap
                    // Try not to spam new requests while one is running, this is an async method.
                    // We'll add parallelism later
                    if (!_isGeneratingWaveForm)
                    {
                        // TO DO: We need to add this in a task queue/bag with a loop that processes bitmap generation (and cancels some requests)
                        _isGeneratingWaveForm = true;
                        //Console.WriteLine("WaveFormCacheService - Requesting a new bitmap - rect: {0} zoom: {1}", rect, zoom);
                        _waveFormRenderingService.RequestBitmap(WaveFormDisplayType.Stereo, rect, new BasicRectangle(0, 0, waveFormWidth, height), zoom);
                    }
                }
            }
            return tile;
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
