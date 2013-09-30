using System;

namespace MPfm.WindowsPhone.Classes.Helpers
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj206974(v=vs.105).aspx
    /// </summary>
    public static class ResolutionHelper
    {
        public enum Resolutions { WVGA, WXGA, HD720p };

        private static bool IsWvga
        {
            get
            {
                return App.Current.Host.Content.ScaleFactor == 100;
            }
        }

        private static bool IsWxga
        {
            get
            {
                return App.Current.Host.Content.ScaleFactor == 160;
            }
        }

        private static bool Is720p
        {
            get
            {
                return App.Current.Host.Content.ScaleFactor == 150;
            }
        }

        public static Resolutions CurrentResolution
        {
            get
            {
                if (IsWvga) return Resolutions.WVGA;
                else if (IsWxga) return Resolutions.WXGA;
                else if (Is720p) return Resolutions.HD720p;
                else throw new InvalidOperationException("Unknown resolution");
            }
        }
    }
}
