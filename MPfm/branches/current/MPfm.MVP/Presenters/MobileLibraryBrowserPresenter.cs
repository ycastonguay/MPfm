//
// MobileLibraryBrowserPresenter.cs: Library browser presenter for mobile devices.
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

using System.Collections.Generic;
using System.Linq;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.Core;
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Library browser presenter for mobile devices.
	/// </summary>
	public class MobileLibraryBrowserPresenter : BasePresenter<IMobileLibraryBrowserView>, IMobileLibraryBrowserPresenter
	{
        //ILibraryBrowserView view;
        readonly ITinyMessengerHub messageHub;
		readonly ILibraryService libraryService;
		readonly IAudioFileCacheService audioFileCacheService;
		
		public AudioFileFormat Filter { get; private set; }
		
		#region Constructor and Dispose

		/// <summary>
        /// Initializes a new instance of the <see cref="MobileLibraryBrowserPresenter"/> class.
		/// </summary>
        public MobileLibraryBrowserPresenter(ITinyMessengerHub messageHub,
		                                ILibraryService libraryService,
		                                IAudioFileCacheService audioFileCacheService)
		{						
			// Set properties
            this.messageHub = messageHub;
			this.libraryService = libraryService;
			this.audioFileCacheService = audioFileCacheService;			
			
			// Set default filter
			Filter = AudioFileFormat.All;
		}
        
		#endregion		
		
		#region IMobileLibraryBrowserPresenter implementation

        public override void BindView(IMobileLibraryBrowserView view)
        {
            base.BindView(view);
            
//            view.OnAudioFileFormatFilterChanged = (format) => { AudioFileFormatFilterChanged(format); };
//            view.OnTreeNodeSelected = (entity) => { TreeNodeSelected(entity); };
//            view.OnTreeNodeExpanded = (entity, obj) => { TreeNodeExpanded(entity, obj); };
//            view.OnTreeNodeExpandable = (entity) => { return TreeNodeExpandable(entity); };
//            view.OnTreeNodeDoubleClicked = (entity) => { TreeNodeDoubleClicked(entity); };

////            // Load configuration
////            if (MPfmConfig.Instance.ShowTooltips)

//            // Refresh view (first level nodes)
//            view.RefreshLibraryBrowser(GetFirstLevelNodes());
        }

		#endregion
		
	}
}

