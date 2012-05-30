using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP;
using MPfm.Library;

namespace MPfm.Mac
{
	public partial class UpdateLibraryWindowController : MonoMac.AppKit.NSWindowController, IUpdateLibraryView
	{
		private MainWindowController mainWindowController = null;
		private IMPfmGateway gateway = null;
		private ILibraryService libraryService = null;
		private IUpdateLibraryService updateLibraryService = null;
		private IUpdateLibraryPresenter presenter = null;

		#region Constructors
		
		// Called when created from unmanaged code
		public UpdateLibraryWindowController(IntPtr handle) : base (handle)
		{
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public UpdateLibraryWindowController(NSCoder coder) : base (coder)
		{
			Initialize();
		}
		
		// Call to load from the XIB/NIB file
		public UpdateLibraryWindowController() : base ("UpdateLibraryWindow")
		{
			Initialize();
		}
		
		// Shared initialization code
		void Initialize()
		{

			// Custom types cannot be used in the constructors under Mac.
			string database = "/Users/animal/.MPfm/MPfm.Database.db";
			gateway = new MPfmGateway(database);
			libraryService = new LibraryService(gateway);
			updateLibraryService = new UpdateLibraryService(libraryService);

			// Create presenter
			presenter = new UpdateLibraryPresenter(
				libraryService,
				updateLibraryService
			);
			presenter.BindView(this);

			// Center window
			Window.Center();
		}

		public override void AwakeFromNib()
		{
			lblTitle.StringValue = string.Empty;
			lblSubtitle.StringValue = string.Empty;

			//toolbar.ValidateVisibleItems();

			btnOK.Enabled = false;
			btnCancel.Enabled = true;
			btnSaveLog.Enabled = false;


			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
			textViewErrorLog.InsertText(new NSString("ASFASDFADSF\nasdkasdkasd"));
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

