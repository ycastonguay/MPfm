//
// APETag.cs: Data structure for APEv1 and APEv2 tags.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MPfm.Sound
{  
    /// <summary>
    /// Data structure for APEv1 and APEv2 tags. The property names are based on the APE keys.
    /// For more information about the APE keys, go to http://wiki.hydrogenaudio.org/index.php?title=APE_key.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class APETag
    {
        /// <summary>
        /// Private value for the TagSize property.
        /// </summary>
        private int tagSize = 0;
        /// <summary>
        /// Defines the APE tag size (including the header if APEv2).
        /// This value excludes the APEv1/APEv2 footer size.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("APEv1/APEv2 tag length (in bytes). Includes the header size for APEv2, but excludes footer size for APEv1 and APEv2.")]
        public int TagSize
        {
            get
            {
                return tagSize;
            }            
            set
            {
                tagSize = value;
            }
        }

        /// <summary>
        /// Private value for the Title property.
        /// </summary>
        private string title = string.Empty;
        /// <summary>
        /// Music piece title, music work.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Music piece title, music work.")]
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        /// <summary>
        /// Private value for the Subtitle property.
        /// </summary>
        private string subtitle = string.Empty;
        /// <summary>
        /// Title when the Title property contains the work or additional subtitle.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Title when the Title property contains the work or additional subtitle.")]
        public string Subtitle
        {
            get
            {
                return subtitle;
            }
            set
            {
                subtitle = value;
            }
        }

        /// <summary>
        /// Private value for the Artist property.
        /// </summary>
        private string artist = string.Empty;
        /// <summary>
        /// Performing artist, list of performing artists.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Performing artist, list of performing artists.")]
        public string Artist
        {
            get
            {
                return artist;
            }
            set
            {
                artist = value;
            }
        }

        /// <summary>
        /// Private value for the Album property.
        /// </summary>
        private string album = string.Empty;
        /// <summary>
        /// Album name.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Album name.")]
        public string Album
        {
            get
            {
                return album;
            }
            set
            {
                album = value;
            }
        }

        /// <summary>
        /// Private value for the DebutAlbum property.
        /// </summary>
        private string debutAlbum = string.Empty;
        /// <summary>
        /// Debut album name.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Debut album name.")]
        public string DebutAlbum
        {
            get
            {
                return debutAlbum;
            }
            set
            {
                debutAlbum = value;
            }
        }

        /// <summary>
        /// Private value for the Publisher property.
        /// </summary>
        private string publisher = string.Empty;
        /// <summary>
        /// Record label or publisher.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Record label or publisher.")]
        public string Publisher
        {
            get
            {
                return publisher;
            }
            set
            {
                publisher = value;
            }
        }

        /// <summary>
        /// Private value for the Conductor property.
        /// </summary>
        private string conductor = string.Empty;
        /// <summary>
        /// Conductor name.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Conductor name.")]
        public string Conductor
        {
            get
            {
                return conductor;
            }
            set
            {
                conductor = value;
            }
        }

        /// <summary>
        /// Private value for the Conductor property.
        /// </summary>
        private int track = 0;
        /// <summary>
        /// Track number, Track number/Total tracks number.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Track number, Track number/Total tracks number.")]
        public int Track
        {
            get
            {
                return track;
            }
            set
            {
                track = value;
            }
        }

        /// <summary>
        /// Private value for the Composer property.
        /// </summary>
        private string composer = string.Empty;
        /// <summary>
        /// Name of the original composer, name of the original arranger.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Name of the original composer, name of the original arranger.")]
        public string Composer
        {
            get
            {
                return composer;
            }
            set
            {
                composer = value;
            }
        }

        /// <summary>
        /// Private value for the Comment property.
        /// </summary>
        private string comment = string.Empty;
        /// <summary>
        /// User comment(s).
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("User comment(s).")]
        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
            }
        }

        /// <summary>
        /// Private value for the Copyright property.
        /// </summary>
        private string copyright = string.Empty;
        /// <summary>
        /// Copyright holder.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Copyright holder.")]
        public string Copyright
        {
            get
            {
                return copyright;
            }
            set
            {
                copyright = value;
            }
        }

        /// <summary>
        /// Private value for the PublicationRight property.
        /// </summary>
        private string publicationRight = string.Empty;
        /// <summary>
        /// Publication right holder.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Publication right holder.")]
        public string PublicationRight
        {
            get
            {
                return publicationRight;
            }
            set
            {
                publicationRight = value;
            }
        }

        /// <summary>
        /// Private value for the File property.
        /// </summary>
        private string file = string.Empty;
        /// <summary>
        /// File location.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("File location.")]
        public string File
        {
            get
            {
                return file;
            }
            set
            {
                file = value;
            }
        }

        /// <summary>
        /// Private value for the EAN_UPC property.
        /// </summary>
        private long ean_UPC = 0;
        /// <summary>
        /// EAN-13/UPC-A bar code identifier.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("EAN-13/UPC-A bar code identifier.")]
        public long EAN_UPC
        {
            get
            {
                return ean_UPC;
            }
            set
            {
                ean_UPC = value;
            }
        }

        /// <summary>
        /// Private value for the ISBN property.
        /// </summary>
        private int isbn = 0;
        /// <summary>
        /// ISBN number with check digit.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("ISBN number with check digit.")]
        public int ISBN
        {
            get
            {
                return isbn;
            }
            set
            {
                isbn = value;
            }
        }

        /// <summary>
        /// Private value for the Catalog property.
        /// </summary>
        private string catalog = string.Empty;
        /// <summary>
        /// ISBN number with check digit.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("ISBN number with check digit.")]
        public string Catalog
        {
            get
            {
                return catalog;
            }
            set
            {
                catalog = value;
            }
        }

        /// <summary>
        /// Private value for the LC property.
        /// </summary>
        private string lc = string.Empty;
        /// <summary>
        /// Label code.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Label code.")]
        public string LC
        {
            get
            {
                return lc;
            }
            set
            {
                lc = value;
            }
        }

        /// <summary>
        /// Private value for the Year property.
        /// </summary>
        private DateTime year = DateTime.MinValue;
        /// <summary>
        /// Release date.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Release date.")]
        public DateTime Year
        {
            get
            {
                return year;
            }
            set
            {
                year = value;
            }
        }

        /// <summary>
        /// Private value for the RecordDate property.
        /// </summary>
        private DateTime recordDate = DateTime.MinValue;
        /// <summary>
        /// Record date.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Record date.")]
        public DateTime RecordDate
        {
            get
            {
                return recordDate;
            }
            set
            {
                recordDate = value;
            }
        }

        /// <summary>
        /// Private value for the RecordLocation property.
        /// </summary>
        private string recordLocation = string.Empty;
        /// <summary>
        /// Record location(s).
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Record location(s).")]
        public string RecordLocation
        {
            get
            {
                return recordLocation;
            }
            set
            {
                recordLocation = value;
            }
        }

        /// <summary>
        /// Private value for the Genre property.
        /// </summary>
        private string genre = string.Empty;
        /// <summary>
        /// Genre(s).
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Genre(s)..")]
        public string Genre
        {
            get
            {
                return genre;
            }
            set
            {
                genre = value;
            }
        }

        /// <summary>
        /// Private value for the Index property.
        /// </summary>
        private string index = string.Empty;
        /// <summary>
        /// Indexes for quick access (index time). Can be a list of index times.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Indexes for quick access (index time). Can be a list of index times.")]
        public string Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

        /// <summary>
        /// Private value for the Related property.
        /// </summary>
        private string related = string.Empty;
        /// <summary>
        /// Location of related information.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Location of related information.")]
        public string Related
        {
            get
            {
                return related;
            }
            set
            {
                related = value;
            }
        }

        /// <summary>
        /// Private value for the Abstract property.
        /// </summary>
        private string _abstract = string.Empty;
        /// <summary>
        /// Abstract (no idea, don't ask me!).
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Abstract (no idea, don't ask me!).")]
        public string Abstract
        {
            get
            {
                return _abstract;
            }
            set
            {
                _abstract = value;
            }
        }

        /// <summary>
        /// Private value for the Language property.
        /// </summary>
        private string language = string.Empty;
        /// <summary>
        /// Used language(s) for music/spoken words.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Used language(s) for music/spoken words.")]
        public string Language
        {
            get
            {
                return language;
            }
            set
            {
                language = value;
            }
        }

        /// <summary>
        /// Private value for the Bibliography property.
        /// </summary>
        private string bibliography = string.Empty;
        /// <summary>
        /// Bibliography/Discography.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Bibliography/Discography.")]
        public string Bibliography
        {
            get
            {
                return bibliography;
            }
            set
            {
                bibliography = value;
            }
        }

        // Media here.

        /// <summary>
        /// Private value for the Dictionary property.
        /// </summary>
        private Dictionary<string, string> dictionary = null;
        /// <summary>
        /// List of key/values contained in the APE tag.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("Dictionary of key/value items found in the APE tag.")]
        public Dictionary<string, string> Dictionary
        {        
            get
            {
                return dictionary;
            }
        }

        /// <summary>
        /// Private value for the Version property.
        /// </summary>
        private APETagVersion version = APETagVersion.Unknown;
        /// <summary>
        /// Defines the APE tag version (APEv1 or APEv2).
        /// Unknown if the APE tag was not found.
        /// </summary>
        [Category("Global"), Browsable(true), ReadOnly(true), Description("APEv1/APEv2 tag version (APEv1 or APEv2).")]
        public APETagVersion Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }

        /// <summary>
        /// Default constructor for the APETag class.
        /// </summary>
        public APETag()
        {
            // Create dictionary
            dictionary = new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// Defines the APE tag version.
    /// </summary>
    public enum APETagVersion
    {
        /// <summary>
        /// The APE tag wasn't found or is in an unknown version.
        /// </summary>
        Unknown = 0, 
        /// <summary>
        /// APE tag version 1.
        /// </summary>
        APEv1 = 1, 
        /// <summary>
        /// APE tag version 2.
        /// </summary>
        APEv2 = 2
    }
}
