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
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Collections.Generic;
using Sessions.MVP.Views;
using Sessions.Library.Objects;

namespace Sessions.OSX
{
    /// <summary>
    /// Update Library window controller.
    /// </summary>
	public partial class UpdateLibraryWindowController : BaseWindowController, IUpdateLibraryView
	{
		public UpdateLibraryWindowController(IntPtr handle) 
            : base (handle)
		{
			Initialize();

		}
		
		public UpdateLibraryWindowController(Action<IBaseView> onViewReady) 
            : base ("UpdateLibraryWindow", onViewReady)
		{
			Initialize();
		}
		
		void Initialize()
        {
            ShowWindowCentered();
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            lblTitle.Font = NSFont.FromFontName("Roboto", 16);
            lblSubtitle.Font = NSFont.FromFontName("Roboto", 11);
            lblPercentageDone.Font = NSFont.FromFontName("Roboto", 11);

			lblTitle.StringValue = string.Empty;
			lblSubtitle.StringValue = string.Empty;
			btnOK.Enabled = false;
			btnCancel.Enabled = true;
			btnSaveLog.Enabled = false;
			textViewErrorLog.Editable = false;

            OnViewReady.Invoke(this);
		}
		
		partial void btnOK_Click(NSObject sender)
		{
			Close();
		}

		partial void btnCancel_Click(NSObject sender)
		{
            OnCancelUpdateLibrary();
		}

		partial void btnSaveLog_Click(NSObject sender)
		{

		}

		#region IUpdateLibraryView implementation

        public Action<List<string>> OnAddFilesToLibrary { get; set; }
        public Action<string> OnAddFolderToLibrary { get; set; }
        public Action OnStartUpdateLibrary { get; set; }
        public Action OnCancelUpdateLibrary { get; set; }
        public Action<string> OnSaveLog { get; set; }

		public void RefreshStatus(UpdateLibraryEntity entity)
		{
            InvokeOnMainThread(() => {
        		lblTitle.StringValue = entity.Title;
        		lblSubtitle.StringValue = entity.Subtitle;
        		lblPercentageDone.StringValue = (entity.PercentageDone * 100).ToString("0.0") + " %";
        		progressBar.DoubleValue = (double)entity.PercentageDone * 100;
            });
		}

		public void AddToLog(string entry)
		{
		
		}

        public void ProcessStarted()
        {
        }

		public void ProcessEnded(bool canceled)
		{
            InvokeOnMainThread(() => {
    			if(canceled) 
                {
    				lblTitle.StringValue = "Library update canceled by user.";
    				lblSubtitle.StringValue = string.Empty;
    			} 
                else 
                {
    				lblTitle.StringValue = "Library updated successfully.";
    				lblSubtitle.StringValue = string.Empty;
    			}

    			btnCancel.Enabled = false;
    			btnOK.Enabled = true;
    			btnSaveLog.Enabled = true;
            });
		}

		#endregion
	}
}

