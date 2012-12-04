
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP;

namespace MPfm.Mac
{
    public class MPfmWindowDelegate : NSWindowDelegate
    {
        Action OnWillClose;

        public MPfmWindowDelegate(Action onWillClose)
        {
            if(onWillClose == null)
                throw new ArgumentNullException("onWillClose");

            this.OnWillClose = onWillClose;
        }

        public override void WillClose(NSNotification notification)
        {
            OnWillClose.Invoke();
        }
    }
}

