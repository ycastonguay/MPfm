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

using NUnit.Framework;
using Sessions.MVP.Services;
using Sessions.MVP.Services.Interfaces;
using Sessions.Sound.AudioFiles;

namespace Sessions.MVP.Tests
{
	[TestFixture]
	public class DownloadImageServiceTest
	{
	    private IDownloadImageService _service;

	    [SetUp]
	    public void Setup()
	    {
	        _service = new DownloadImageService();
	    }

        [Test]
	    public void DownloadAlbumArtTest()
        {
            var audioFile = new AudioFile()
            {
                ArtistName = "Frank Zappa",
                AlbumTitle = "Apostrophe"
            };
            var task = _service.DownloadAlbumArt(audioFile);
            task.RunSynchronously();
            var result = task.Result;
   
            Assert.IsNotNull(result.ImageData);
        }
	}
}
