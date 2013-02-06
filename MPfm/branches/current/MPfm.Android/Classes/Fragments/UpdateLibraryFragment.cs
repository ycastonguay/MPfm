using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Models;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class UpdateLibraryFragment : BaseDialogFragment, IUpdateLibraryView, View.IOnClickListener
    {        
        private View _view;        

        public UpdateLibraryFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Fragment_UpdateLibrary, container, false);
            return _view;
        }

        public void OnClick(View v)
        {
            
        }

        #region IUpdateLibraryView implementation

        public Action<UpdateLibraryMode, List<string>, string> OnStartUpdateLibrary { get; set; }

        public void RefreshStatus(UpdateLibraryEntity entity)
        {
            throw new NotImplementedException();
        }

        public void AddToLog(string entry)
        {
            throw new NotImplementedException();
        }

        public void ProcessEnded(bool canceled)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}