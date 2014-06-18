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
using System.Windows.Data;
using Sessions.MVP.Models;

namespace MPfm.WPF.Classes.Converters
{
    public class LibraryBrowserImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var entityType = (LibraryBrowserEntityType) value;
            switch (entityType)
            {
                case LibraryBrowserEntityType.AllSongs:
                    return "/Resources/Images/Icons/windows.png";
                    break;
                case LibraryBrowserEntityType.Artists:
                    return "/Resources/Images/Icons/user.png";
                    break;
                case LibraryBrowserEntityType.Albums:
                    return "/Resources/Images/Icons/vinyl.png";
                    break;
                case LibraryBrowserEntityType.Artist:
                    return "/Resources/Images/Icons/user.png";
                    break;
                case LibraryBrowserEntityType.ArtistAlbum:
                    return "/Resources/Images/Icons/vinyl.png";
                    break;
                case LibraryBrowserEntityType.Album:
                    return "/Resources/Images/Icons/vinyl.png";
                    break;
                case LibraryBrowserEntityType.Song:
                    break;
                case LibraryBrowserEntityType.Dummy:
                    break;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }

        //private string GetImageName(string text)
        //{
        //    string name = "";
        //    name = text.ToLower() + ".png";
        //    return name;
        //}
    } 
}
