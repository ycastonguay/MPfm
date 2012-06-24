//  
//  UpdateLibraryWindow.cs
//  
//  Author:
//       Yanick Castonguay <ycastonguay@mp4m.org>
// 
//  Copyright (c) 2012 2011 - 2012 Yanick Castonguay
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using MPfm.Library;
using MPfm.MVP;
using Ninject;
using Pango;
using Gtk;

namespace MPfm.GTK
{
	/// <summary>
	/// Update Library window.
	/// </summary>
	public partial class UpdateLibraryWindow : Gtk.Window, IUpdateLibraryView
	{
		// Private variables		
		private IUpdateLibraryPresenter presenter = null;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.GTK.SettingsWindow"/> class.
		/// </summary>		
		public UpdateLibraryWindow (UpdateLibraryMode mode, List<string> filePaths, string folderPath) : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build ();	
						
			// Set fonts
			SetFontProperties();
			
			// Create presenter			
			var libraryService = Bootstrapper.GetKernel().Get<LibraryService>();
			var updateLibraryService = Bootstrapper.GetKernel().Get<UpdateLibraryService>();
			presenter = new UpdateLibraryPresenter(libraryService, updateLibraryService);
			presenter.BindView(this);
			
			presenter.UpdateLibrary(mode, filePaths, folderPath);
			
			textviewErrorLog.GrabFocus();
		}
		
		private void SetFontProperties()
		{				
			// Get default font name
			string defaultFontName = this.lblTitle.Style.FontDescription.Family;			
			this.lblTitle.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));
			this.lblSubtitle.ModifyFont(FontDescription.FromString(defaultFontName +" 8"));
			this.lblPercentage.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));
			this.lblTimeElapsed.ModifyFont(FontDescription.FromString(defaultFontName +" 8"));
			this.lblEstimatedTimeLeft.ModifyFont(FontDescription.FromString(defaultFontName +" 8"));
		}
		
		/// <summary>
		/// Raises the delete event (when the form is closing).
		/// </summary>
		/// <param name='o'>Object</param>
		/// <param name='args'>Event arguments</param>
		protected void OnDeleteEvent(object o, Gtk.DeleteEventArgs args)
		{			
			args.RetVal = true;	
			//this.Destroy();
		}

		protected void OnActionCancelActivated (object sender, System.EventArgs e)
		{
			presenter.Cancel();	
		}

		protected void OnActionOKActivated (object sender, System.EventArgs e)
		{
			this.Destroy();
		}

		protected void OnActionSaveLogActivated (object sender, System.EventArgs e)
		{
			//presenter.SaveLog();
		}
		
		public void RefreshStatus(UpdateLibraryEntity entity)
		{
			// Invoke UI changes
			Gtk.Application.Invoke(delegate{
				
				// Check for error
				if(entity.Exception != null)
				{
					// Add error to log				
					TextIter textIter = textviewErrorLog.Buffer.EndIter;
					textviewErrorLog.Buffer.Insert(ref textIter, entity.FilePath + "\n");					
					textviewErrorLog.ScrollToIter(textviewErrorLog.Buffer.EndIter, 0, false, 0, 0);
				}
				else
				{			
					lblTitle.Text = entity.Title;
					lblSubtitle.Text = entity.Subtitle;
					lblPercentage.Text = entity.PercentageDone.ToString("00.00%");
					progressbar.Fraction = entity.PercentageDone;
				}
			});
		}
		
		public void AddToLog(string entry)
		{
//			// Invoke UI changes
//			Gtk.Application.Invoke(delegate{
//				textviewErrorLog.Buffer.Text += entry + "\n";
//			});			
		}
		
		public void ProcessEnded(bool canceled)
		{
			if(canceled)
			{
				lblTitle.Text = "Library update canceled by user.";
				lblSubtitle.Text = string.Empty;
			}
			else
			{
				lblTitle.Text = "Library updated successfully.";
				lblSubtitle.Text = string.Empty;
			}
			
			actionCancel.Sensitive = false;
			actionOK.Sensitive = true;
			actionSaveLog.Sensitive = true;
		}
	}
}

