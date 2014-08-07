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
using System.CodeDom;
using System.Net.Mime;
using System.Threading.Tasks;
using Sessions.Core.Network;
using Sessions.MVP.Services.Interfaces;
using Sessions.Sound.AudioFiles;

namespace Sessions.MVP.Services
{
    /// <summary>
    /// Service used for downloading images from the internet.
    /// </summary>
    public class DownloadImageService : IDownloadImageService
    {
        private IDownloadImageProvider _provider;

        public delegate void ImageDownloaded(AudioFile audioFile, string imageUrl, byte[] data);

        public event ImageDownloaded OnImageDownloaded;

        public DownloadImageService()
        {
            OnImageDownloaded += (audioFile, url, data) => { };
            _provider = new DownloadImageProvider();
        }

        public Task<DownloadImageResult> DownloadAlbumArt(AudioFile audioFile)
        {
            var task = new Task<DownloadImageResult>(() =>
            {
                try
                {
                    var httpService = new HttpService();

                    string searchUrl = _provider.GetSearchUrl(audioFile);
                    string html = httpService.DownloadString(searchUrl);

                    var imageUrls = _provider.ExtractImageUrlsFromSearchResults(html);
                    if (imageUrls.Count == 0)
                        return new DownloadImageResult(audioFile, string.Empty, null);

                    string imageUrl = imageUrls[0];
                    byte[] imageData = httpService.DownloadData(imageUrl);

                    OnImageDownloaded(audioFile, imageUrl, imageData);
                    return new DownloadImageResult(audioFile, imageUrl, imageData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DownloadImageService - Failed to download image: {0}", ex);
                    //return null;
                    return new DownloadImageResult(audioFile, string.Empty, null);
                }
            });
            return task;
        }

        public class DownloadImageResult
        {
            public AudioFile AudioFile { get; set; }
            public string ImageUrl { get; set; }
            public byte[] ImageData { get; set; }

            public DownloadImageResult()
            {
            }

            public DownloadImageResult(AudioFile audioFile, string imageUrl, byte[] imageData)
            {
                this.AudioFile = audioFile;
                this.ImageUrl = imageUrl;
                this.ImageData = imageData;
            }
        }
    }
}
