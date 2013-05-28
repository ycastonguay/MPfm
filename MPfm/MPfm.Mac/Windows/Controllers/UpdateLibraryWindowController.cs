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
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP;
using MPfm.Library;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters;
using MPfm.MVP.Bootstrap;
using MPfm.Library.Services.Interfaces;
using MPfm.Library.Objects;

namespace MPfm.Mac
{
    /// <summary>
    /// Update Library window controller.
    /// </summary>
	public partial class UpdateLibraryWindowController : BaseWindowController, IUpdateLibraryView
	{
		MainWindowController _mainWindowController = null;
		IUpdateLibraryPresenter _presenter = null;

		#region Constructors
		
		// Called when created from unmanaged code
		public UpdateLibraryWindowController(IntPtr handle) 
            : base (handle)
		{
			Initialize();
		}
		
		// Call to load from the XIB/NIB file
		public UpdateLibraryWindowController(MainWindowController mainWindowController, Action<IBaseView> onViewReady) 
            : base ("UpdateLibraryWindow", onViewReady)
		{
            _mainWindowController = mainWindowController;
			Initialize();
		}
		
		// Shared initialization code
		void Initialize()
		{
            _presenter = Bootstrapper.GetContainer().Resolve<UpdateLibraryPresenter>();
			_presenter.BindView(this);
			Window.Center();
		}

		public override void AwakeFromNib()
		{
            lblTitle.Font = NSFont.FromFontName("TitilliumText25L-800wt", 16);
            lblSubtitle.Font = NSFont.FromFontName("Junction", 11);
            lblPercentageDone.Font = NSFont.FromFontName("Junction", 11);

			lblTitle.StringValue = string.Empty;
			lblSubtitle.StringValue = string.Empty;
			btnOK.Enabled = false;
			btnCancel.Enabled = true;
			btnSaveLog.Enabled = false;
			textViewErrorLog.Editable = false;
		}
		
		#endregion
		
		public void StartProcess(UpdateLibraryMode mode, List<string> filePaths, string folderPath)
		{
			_presenter.UpdateLibrary(mode, filePaths, folderPath);
		}

		partial void btnOK_Click(NSObject sender)
		{
            _mainWindowController.RefreshAll();
			this.Close();
		}

		partial void btnCancel_Click(NSObject sender)
		{
			_presenter.Cancel();
		}

		partial void btnSaveLog_Click(NSObject sender)
		{

		}

		#region IUpdateLibraryView implementation

        public Action<UpdateLibraryMode, List<string>, string> OnStartUpdateLibrary { get; set; }
        public Action OnCancelUpdateLibrary { get; set; }

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

