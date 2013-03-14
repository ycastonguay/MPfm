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

using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Player.Objects;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Markers view presenter.
	/// </summary>
	public class MarkersPresenter : BasePresenter<IMarkersView>, IMarkersPresenter
	{
        readonly MobileNavigationManager _navigationManager;

        public MarkersPresenter(MobileNavigationManager navigationManager)
		{
            _navigationManager = navigationManager;
		}

        public override void BindView(IMarkersView view)
        {            
            // Subscribe to view actions
            view.OnAddMarker = OnAddMarker;
            view.OnEditMarker = OnEditMarker;

            base.BindView(view);
        }

        private void CreateMarkerDetailsView()
        {
            var view = _navigationManager.CreateMarkerDetailsView();
            _navigationManager.PushDialogView(view);
        }

        private void OnAddMarker()
        {
            CreateMarkerDetailsView();
        }

        private void OnEditMarker(Marker marker)
        {
        }
    }
}

