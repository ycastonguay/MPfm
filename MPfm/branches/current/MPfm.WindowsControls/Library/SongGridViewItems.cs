using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MPfm.Library;
using MPfm.Player;
using MPfm.Sound;

namespace MPfm.WindowsControls
{
    public class SongGridViewItem
    {
        //public string Title { get; set; }        
        public bool IsSelected { get; set; }
        public bool IsMouseOverItem { get; set; }
        public SongDTO Song { get; set; }
        //public AudioFile AudioFile { get; set; }

        /// <summary>
        /// Default constructor for GridViewSongItem.
        /// </summary>
        public SongGridViewItem()
        {
            // Set default values
            IsSelected = false;
            IsMouseOverItem = false;
        }
    }

    public class GridViewSongColumn
    {
        public string Title { get; set; }
        public string FieldName { get; set; }
        public bool Visible { get; set; }
        public int Order { get; set; }
        public int Width { get; set; }
        public bool CanBeMoved { get; set; }
        public bool CanBeResized { get; set; }
        public bool CanBeReordered { get; set; }

        // Internal flags
        public bool IsMouseOverColumnHeader { get; set; }
        public bool IsMouseCursorOverColumnLimit { get; set; }
        public bool IsUserResizingColumn { get; set; }
        public bool IsUserMovingColumn { get; set; }

        public GridViewSongColumn(string title, string fieldName)
        {
            Title = title;
            FieldName = fieldName;
            Visible = false;
            Order = -1;
            Width = 100;
            CanBeResized = true;
            CanBeReordered = true;
            CanBeMoved = true;
            IsMouseOverColumnHeader = false;
            IsMouseCursorOverColumnLimit = false;
            IsUserResizingColumn = false;
            IsUserMovingColumn = false;
        }

        public GridViewSongColumn(string title, string fieldName, bool visible, int order)
        {
            Title = title;
            FieldName = fieldName;
            Visible = visible;
            Order = order;
            Width = 100;
            CanBeResized = true;
            CanBeReordered = true;
            CanBeMoved = true;
            IsMouseOverColumnHeader = false;
            IsMouseCursorOverColumnLimit = false;
            IsUserResizingColumn = false;
            IsUserMovingColumn = false;
        }
    }

    public class GridViewSongCache
    {
        public int LineHeight { get; set; }
        public int TotalWidth { get; set; }
        public int TotalHeight { get; set; }
        public int ScrollBarOffsetY { get; set; }
        public int NumberOfLinesFittingInControl { get; set; }
        public List<GridViewSongColumn> ActiveColumns { get; set; }

    }

    public class GridViewImageCache
    {
        public string Key { get; set; }
        public Image Image { get; set; }
    }
}
