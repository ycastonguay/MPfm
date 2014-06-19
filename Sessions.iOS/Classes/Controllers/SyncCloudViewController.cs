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
using System.Drawing;
using System.Linq;
using DropBoxSync.iOS;
using Sessions.Library;
using Sessions.Library.Services.Interfaces;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Delegates;

namespace MPfm.iOS
{
    public partial class SyncCloudViewController : BaseViewController, ISyncCloudView
    {
        private ICloudLibraryService _cloudLibrary;

        public SyncCloudViewController()
            : base (UserInterfaceIdiomIsPhone ? "SyncCloudViewController_iPhone" : "SyncCloudViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _cloudLibrary = Bootstrapper.GetContainer().Resolve<ICloudLibraryService>();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindSyncCloudView(this);

//            if (_store != null && _store.Open)
//                return;

//            DBError error = null;
//            var account = DBAccountManager.SharedManager.LinkedAccount;
//
//            // TODO: account will be null when the user delogs out
//            _store = DBDatastore.OpenDefaultStoreForAccount(account, out error);
//            if(error != null)
//                throw new Exception(error.Description);
//
//            _store.AddObserver (_store, () => {
//                Console.WriteLine("SyncCloudViewController - DBDatastore - Data changed!");
//                if (_store.Status.HasFlag(DBDatastoreStatus.Incoming)) {
//                    // Handle the updated data
//                    Console.WriteLine("SyncCloudViewController - DBDatastore - Incoming data!");
//                    try
//                    {
//                        DBError error2 = null;
//                        var changes = _store.Sync(error2);
//                        if(error2 != null)
//                            throw new Exception(error2.Description);
//
//                        if (!changes.ContainsKey(new NSString("stuff")))
//                            return;
//
//                        var records = (NSSet)changes["stuff"];
//                        foreach(var record in records)
//                        {
//                            var theRecord = (DBRecord)record;
//                            var timestamp = theRecord.ObjectForKey("timestamp");
//                            var deviceType = theRecord.ObjectForKey("deviceType");
//                            var deviceName = theRecord.ObjectForKey("deviceName");
//                            lblValue.Text = string.Format("{0} {1} {2}", deviceType, deviceName, timestamp);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine("SyncCloudActivity - OnDatastoreStatusChange exception: {0}", ex);
//                        lblValue.Text = string.Format("Error: {0}", ex);
//                    }
//                }
//            });
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Sync (Cloud)");
        }

        partial void actionEnableSync(NSObject sender)
        {
            try
            {
                var account = DBAccountManager.SharedManager.LinkedAccount;
                if(account == null)
                {
                    var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
					DBAccountManager.SharedManager.LinkFromController(appDelegate.MainViewController);
                }
            }
            catch(Exception ex)
            {
                UIAlertView alertView = new UIAlertView("SyncCloud Error", ex.Message, null, "OK", null);
                alertView.Show();
            }
        }

        partial void actionDisableSync(NSObject sender)
        {
            try
            {
                // No such thing on iOS?
                // https://www.dropbox.com/developers/datastore/tutorial/ios
            }
            catch(Exception ex)
            {
                UIAlertView alertView = new UIAlertView("SyncCloud Error", ex.Message, null, "OK", null);
                alertView.Show();
            }
        }

        partial void actionPull(NSObject sender)
        {
//            try
//            {
//                DBError error = null;
//
//                var table = _store.GetTable("stuff");
//                //var record = table.GetRecord("H8f7Q9JRhAkzzzmBa-dDww", out error);
//                var records = table.Query(null, out error);
//                if(error != null)
//                    throw new Exception(error.Description);
//
//                if (records.Length == 0)
//                {
//                    lblValue.Text = "No value!";
//                    return;
//                }
//
//                var firstResult = records[0];
//                var timestamp = firstResult.ObjectForKey("timestamp");
//                var deviceType = firstResult.ObjectForKey("deviceType");
//                var deviceName = firstResult.ObjectForKey("deviceName");
//                lblValue.Text = string.Format("{0} {1} {2}", deviceType, deviceName, timestamp);
//            }
//            catch(Exception ex)
//            {
//                UIAlertView alertView = new UIAlertView("SyncCloud Error", ex.Message, null, "OK", null);
//                alertView.Show();
//            }
        }

        partial void actionPush(NSObject sender)
        {
//            try
//            {
//                DBError error = null;
//                var table = _store.GetTable("stuff");
//                var keys = new NSString[] {
//                    new NSString("hello"),
//                    new NSString("deviceType"),
//                    new NSString("deviceName"),
//                    new NSString("ip"),
//                    new NSString("test"),
//                    new NSString("timestamp")
//                };
//                var values = new NSObject[] {
//                    new NSString("world"),
//                    new NSString(_deviceSpecs.GetDeviceType().ToString()),
//                    new NSString(_deviceSpecs.GetDeviceName()),
//                    new NSString(_deviceSpecs.GetIPAddress()),
//                    NSObject.FromObject(true),
//                    new NSString(DateTime.Now.ToLongTimeString())
//                };
//
//                NSDictionary data = NSDictionary.FromObjectsAndKeys(values, keys);
//                DBRecord stuff = table.Insert(data);
//
//                _store.Sync(error);
//                if(error != null)
//                    throw new Exception(error.Description);
//            }
//            catch(Exception ex)
//            {
//                UIAlertView alertView = new UIAlertView("SyncCloud Error", ex.Message, null, "OK", null);
//                alertView.Show();
//            }
        }

        partial void actionDelete(NSObject sender)
        {
//            try
//            {
//                DBError error = null;
//                var table = _store.GetTable("stuff");
//                var records = table.Query(null, out error);
//                if (records.Length == 0)
//                {
//                    lblValue.Text = "No value!";
//                    return;
//                }
//
//                foreach(var record in records)
//                {
//                    record.DeleteRecord();
//                }
//
//                _store.Sync(error);
//                if(error != null)
//                    throw new Exception(error.Description);
//            }
//            catch(Exception ex)
//            {
//                UIAlertView alertView = new UIAlertView("SyncCloud Error", ex.Message, null, "OK", null);
//                alertView.Show();
//            }
        }

        private void OnCloudDataChanged(string data)
        {
        }

        #region ISyncCloudView implementation

        public void SyncCloudError(Exception ex)
        {
            InvokeOnMainThread(() => {
                UIAlertView alertView = new UIAlertView("SyncCloud Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        #endregion
    }
}

