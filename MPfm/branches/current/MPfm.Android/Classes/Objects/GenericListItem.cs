using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace MPfm.Android.Classes.Objects
{
    public class GenericListItem
    {
        public string Title { get; set; }
        public string ImageKey { get; set; }
        public string AudioFilePath { get; set; }
    }
}