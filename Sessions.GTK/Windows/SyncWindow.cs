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
using System.Text;
using MPfm.GTK.Helpers;
using MPfm.GTK.Windows;
using Sessions.Library.Objects;
using Sessions.MVP.Views;
using Gtk;

namespace MPfm.GTK
{
	public partial class SyncWindow : BaseWindow, ISyncView
	{
        Gtk.TreeStore _storeDevices;
        bool _isDiscovering;

		public SyncWindow(Action<IBaseView> onViewReady) : 
			base(Gtk.WindowType.Toplevel, onViewReady)
		{
			this.Build();

            Title = "Sync Library With Other Devices";
            InitializeDeviceTreeView();

            onViewReady(this);
            btnRefreshDeviceList.GrabFocus(); // the list view changes color when focused by default. it annoys me!
            this.Center();
            this.Show();
		}

		protected override bool OnDeleteEvent(Gdk.Event evnt)
		{
            return base.OnDeleteEvent(evnt);
		}

        private void InitializeDeviceTreeView()
        {
            _storeDevices = new Gtk.TreeStore(typeof(SyncDevice));
            treeViewDevices.ShowExpanders = false;
            treeViewDevices.HeadersVisible = false;

            // Create title column
            Gtk.TreeViewColumn colTitle = new Gtk.TreeViewColumn();
            Gtk.CellRendererPixbuf cellPixbuf = new Gtk.CellRendererPixbuf();
            Gtk.CellRendererText cellTitle = new Gtk.CellRendererText();
            colTitle.PackStart(cellPixbuf, false);
            colTitle.PackStart(cellTitle, true);
            colTitle.SetCellDataFunc(cellPixbuf, new Gtk.TreeCellDataFunc(RenderDeviceCell));
            colTitle.SetCellDataFunc(cellTitle, new Gtk.TreeCellDataFunc(RenderDeviceCell));
            treeViewDevices.AppendColumn(colTitle);
        }

        private void RefreshDeviceListButton()
        {
            if (_isDiscovering)
            {
                btnRefreshDeviceList.Image = new Gtk.Image(Stock.Cancel, IconSize.SmallToolbar);
                btnRefreshDeviceList.Label = "Cancel refresh";
            } 
            else
            {
                btnRefreshDeviceList.Image = new Gtk.Image(Stock.Refresh, IconSize.SmallToolbar);
                btnRefreshDeviceList.Label = "Refresh devices";
            }
        }
        
        private void RenderDeviceCell(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            //Console.WriteLine("SyncWindow - RenderDeviceCell");
            SyncDevice device = (SyncDevice)model.GetValue(iter, 0);

            if (cell is Gtk.CellRendererText)
            {
                (cell as Gtk.CellRendererText).Text = device.Name;
            }
            else if (cell is Gtk.CellRendererPixbuf)
            {
                var cellPixbuf = (Gtk.CellRendererPixbuf)cell;
                Gdk.Pixbuf pixbuf = null;
                switch (device.DeviceType)
                {
                    case SyncDeviceType.Linux:
                        pixbuf = ResourceHelper.GetEmbeddedImageResource("icon_linux.png");
                        break;
                    case SyncDeviceType.OSX:
                        pixbuf = ResourceHelper.GetEmbeddedImageResource("icon_osx.png");
                        break;
                    case SyncDeviceType.Windows:
                        pixbuf = ResourceHelper.GetEmbeddedImageResource("icon_windows.png");
                        break;
					case SyncDeviceType.iPhone:
					case SyncDeviceType.iPad:
                        pixbuf = ResourceHelper.GetEmbeddedImageResource("icon_phone.png");
                        break;
                    case SyncDeviceType.AndroidPhone:
					case SyncDeviceType.AndroidTablet:
                        pixbuf = ResourceHelper.GetEmbeddedImageResource("icon_android.png");
                        break;
                }

                if(pixbuf != null)
                    cellPixbuf.Pixbuf = pixbuf;
            }
        }

        protected void OnClickRefreshDeviceList(object sender, EventArgs e)
        {
//            if(_isDiscovering)
//                OnCancelDiscovery();
//            else
//                OnStartDiscovery();
        }        

        protected void OnClickConnectManual(object sender, EventArgs e)
        {

        }

        protected void OnClickConnect(object sender, EventArgs e)
        {
            TreeModel model;
            TreeIter iter;  
            if(treeViewDevices.Selection.GetSelected(out model, out iter))
            {
                SyncDevice device = (SyncDevice)_storeDevices.GetValue(iter, 0);                              
				//OnConnectDevice(device);
            }
        }

        #region ISyncView implementation

		public System.Action<string> OnAddDeviceFromUrl { get; set; }
		public System.Action<SyncDevice> OnRemoveDevice { get; set; }
		public System.Action<SyncDevice> OnSyncLibrary { get; set; }
		public System.Action<SyncDevice> OnResumePlayback { get; set; }
		public System.Action OnOpenAddDeviceDialog { get; set; }

		public System.Action<SyncDevice> OnRemotePlayPause { get; set; }
		public System.Action<SyncDevice> OnRemotePrevious { get; set; }
		public System.Action<SyncDevice> OnRemoteNext { get; set; }
		public System.Action<SyncDevice> OnRemoteRepeat { get; set; }
		public System.Action<SyncDevice> OnRemoteShuffle { get; set; }

        public void SyncError(Exception ex)
        {
            Gtk.Application.Invoke(delegate{            
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("An error occured in the Sync component:");
                sb.AppendLine(ex.Message);
                sb.AppendLine();
                sb.AppendLine(ex.StackTrace);                                                               
                MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, sb.ToString());
                md.Run();
                md.Destroy();
            });
        }

        public void RefreshIPAddress(string address)
        {
            Gtk.Application.Invoke(delegate{
                lblSubtitle.Text = address;
            });
        }

        public void RefreshDiscoveryProgress(float percentageDone, string status)
        {
            Gtk.Application.Invoke(delegate{
                if(!_isDiscovering)
                {
                    _isDiscovering = true;
                    RefreshDeviceListButton();
                }
                progressBar.Fraction = percentageDone / 100f;
            });
        }

        public void RefreshDevices(IEnumerable<SyncDevice> devices)
        {
            Gtk.Application.Invoke(delegate{
                Console.WriteLine("SyncWindow - RefreshDevices");
                _storeDevices.Clear();
                
                foreach(SyncDevice device in devices)
                    _storeDevices.AppendValues(device);
                            
                treeViewDevices.Model = _storeDevices;
            });
        }

        public void RefreshDevicesEnded()
        {
            Gtk.Application.Invoke(delegate{
                _isDiscovering = false;
                RefreshDeviceListButton();
            });
        }

		public void RefreshStatus(string status)
		{
		}

		public void NotifyAddedDevice(SyncDevice device)
		{
		}

		public void NotifyRemovedDevice(SyncDevice device)
		{
		}

		public void NotifyUpdatedDevice(SyncDevice device)
		{
		}

		public void NotifyUpdatedDevices(IEnumerable<SyncDevice> devices)
		{
		}

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion

	}
}
