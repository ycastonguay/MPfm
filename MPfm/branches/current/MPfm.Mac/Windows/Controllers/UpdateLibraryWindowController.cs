//
// UpdateLibraryWindowController.cs: Update Library window controller.
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
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP;
using MPfm.Library;
using MPfm.MVP.Services.Interfaces;
using MPfm.Library.Gateway;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters;

namespace MPfm.Mac
{
    /// <summary>
    /// Update Library window controller.
    /// </summary>
	public partial class UpdateLibraryWindowController : BaseWindowController, IUpdateLibraryView
	{
		MainWindowController mainWindowController = null;
		ILibraryService libraryService = null;
		IUpdateLibraryService updateLibraryService = null;
		IUpdateLibraryPresenter presenter = null;

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
            this.mainWindowController = mainWindowController;
			Initialize();
		}
		
		// Shared initialization code
		void Initialize()
		{
			// Custom types cannot be used in the constructors under Mac.
			string database = "/Users/animal/.MPfm/MPfm.Database.db";
//			gateway = new MPfmGateway(database);
//			libraryService = new LibraryService(gateway);
//			updateLibraryService = new UpdateLibraryService(libraryService);
//
//			// Create presenter
//			presenter = new UpdateLibraryPresenter(
//				libraryService,
//				updateLibraryService
//			);

            presenter = Bootstrapper.GetContainer().Resolve<UpdateLibraryPresenter>();

			presenter.BindView(this);

			// Center window
			Window.Center();
		}

		public override void AwakeFromNib()
		{
			lblTitle.StringValue = string.Empty;
			lblSubtitle.StringValue = string.Empty;

			btnOK.Enabled = false;
			btnCancel.Enabled = true;
			btnSaveLog.Enabled = false;

			textViewErrorLog.InsertText(new NSString("Test\nTest"));
			textViewErrorLog.Editable = false;
		}
		
		#endregion
		
		//strongly typed window accessor
		public new UpdateLibraryWindow Window {
			get {
				return (UpdateLibraryWindow)base.Window;
			}
		}

		public void StartProcess(UpdateLibraryMode mode, List<string> filePaths, string folderPath)
		{
			presenter.UpdateLibrary(mode, filePaths, folderPath);
		}

		partial void btnOK_Click(NSObject sender)
		{
            mainWindowController.RefreshAll();
			this.Close();
		}

		partial void btnCancel_Click(NSObject sender)
		{
			presenter.Cancel();
		}

		partial void btnSaveLog_Click(NSObject sender)
		{

		}

		#region IUpdateLibraryView implementation

		public void RefreshStatus(UpdateLibraryEntity entity)
		{
			lblTitle.StringValue = entity.Title;
			lblSubtitle.StringValue = entity.Subtitle;
			lblPercentageDone.StringValue = (entity.PercentageDone * 100).ToString() + " %";
			progressBar.DoubleValue = (double)entity.PercentageDone * 100;
		}

		public void AddToLog(string entry)
		{
		
		}

		public void ProcessEnded(bool canceled)
		{
			if(canceled) {
				lblTitle.StringValue = "Library update canceled by user.";
				lblSubtitle.StringValue = string.Empty;
			} else {
				lblTitle.StringValue = "Library updated successfully.";
				lblSubtitle.StringValue = string.Empty;
			}

			btnCancel.Enabled = false;
			btnOK.Enabled = true;
			btnSaveLog.Enabled = true;
		}

		#endregion
	}
}

