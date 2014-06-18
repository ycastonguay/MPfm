using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Sessions.MVP.Views;
using Sessions.Sound.AudioFiles;

namespace MPfm.OSX
{
    public partial class EditSongMetadataWindowController : BaseWindowController, IEditSongMetadataView
    {
        // Called when created from unmanaged code
        public EditSongMetadataWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public EditSongMetadataWindowController(Action<IBaseView> onViewReady) 
            : base ("EditSongMetadataWindow", onViewReady)
        {
            Initialize();
        }

        void Initialize()
        {
        }

        //strongly typed window accessor
        public new EditSongMetadataWindow Window
        {
            get
            {
                return (EditSongMetadataWindow)base.Window;
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        #region IEditSongMetadataView implementation

        public Action<AudioFile> OnSaveAudioFile { get; set; }

        public void EditSongMetadataError(Exception ex)
        {
            ShowError(ex);
        }

        public void RefreshAudioFile(AudioFile audioFile)
        {
        }

        #endregion
    }
}

