using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MPfm.Library;

namespace MPfm.WindowsControls
{
    public class AlbumViewItem
    {
        public string Title { get; set; }
        public string FilePath { get; set; }
        public bool IsSelected { get; set; }
        public bool IsMouseOverItem { get; set; }        

        /// <summary>
        /// Default constructor for GridViewSongItem.
        /// </summary>
        public AlbumViewItem()
        {
            // Set default values
            IsSelected = false;
            IsMouseOverItem = false;
        }
    }

    public class GridViewAlbumCache
    {
        public int IconHeightWithPadding { get; set; }
        public int NumberOfIconsWidth { get; set; }
        public int NumberOfIconsHeight { get; set; }
        public int TotalMargin { get; set; }
        public int Margin { get; set; }
        public int TotalNumberOfLines { get; set; }
        public int TotalHeight { get; set; }
    }
}
