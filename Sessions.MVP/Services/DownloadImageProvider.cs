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
using System.Net.Mime;
using System.Xml.Linq;
using Sessions.MVP.Services.Interfaces;
using Sessions.Sound.AudioFiles;

namespace Sessions.MVP.Services
{	
	/// <summary>
    /// Provider used for downloading images from the internet.
	/// </summary>
    public class DownloadImageProvider : IDownloadImageProvider
	{
	    public string GetSearchUrl(AudioFile audioFile)
	    {
	        string baseUrl = "http://www.allmusic.com/search/albums";
	        string artistName = audioFile.ArtistName.Replace(" ", "+");
            string albumTitle = audioFile.AlbumTitle.Replace(" ", "+");
	        return string.Format("{0}/{1}+{2}", baseUrl, artistName, albumTitle);
	    }

	    public List<string> ExtractImageUrlsFromSearchResults(string html)
	    {
	        try
	        {
                // We're keeping this very simple for now, but this is weak if the page layout changes a bit
	            var imageUrls = new List<string>();
	            string match = "http://cps-static.rovicorp.com";
	            var split = html.Split(new string[1] {match}, StringSplitOptions.RemoveEmptyEntries).ToList();
	            split.RemoveAt(0);
	            foreach (var part in split)
	            {
	                int indexOfQuote = part.IndexOf('"');
	                string url = match + part.Substring(0, indexOfQuote);
	                url = url.Replace("/JPG_170/", "/JPG_500/");

	                if (!imageUrls.Contains(url))
	                {
	                    //Console.WriteLine("Found url: {0}", url);
	                    imageUrls.Add(url);
	                }
	            }
	            return imageUrls;
	        }
	        catch (Exception ex)
	        {
                Console.WriteLine("DownloadImageProvider - Failed to load image: {0}", ex);
	            throw;
	        }
	    }
	}
}
