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
using Android.Webkit;

namespace MPfm.Android.Classes.Clients
{
    public class MyWebViewClient : WebViewClient
    {
        public delegate void PageFinishedHandler(object sender, EventArgs e);

        public event PageFinishedHandler PageFinished;

        protected virtual void OnPageFinishedEvent(EventArgs e)
        {
            if (PageFinished != null)
                PageFinished(this, e);
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            OnPageFinishedEvent(new EventArgs());
        }
    }
}