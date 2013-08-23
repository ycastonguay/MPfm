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
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using MPfm.MVP.Views;

namespace MPfm.Windows.Classes.Forms
{
    /// <summary>
    /// Splash screen window. This is where the MPfm logo is displayed with the copyright notices and
    /// credits. This is used for the unskippable splash screen at the start of the application and for
    /// the About screen.
    /// </summary>
    public partial class frmSplash : BaseForm, ISplashView
    {
        // Private variables
        private static frmSplash windowSplash = null;
        private static Thread thread = null;
        private bool fadingIn = true;
        private bool fadingOut = false;
        private bool closeFormAfterFadeOut = false;
        private Rectangle rectLink = new Rectangle(320, 260, 90, 10);
        private string status = "";
        private string error = "";
        private bool InitDone { get; set; }

        public frmSplash(Action<IBaseView> onViewReady) : base (onViewReady)
        {
            InitDone = false;
            InitializeComponent();

            lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine("End of frmSplash ctor");

            ViewIsReady();
        }

        /// <summary>
        /// Returns a random number (for displaying a random background image).
        /// </summary>
        /// <returns>Random number</returns>
        private int RandomNumber()
        {
            Random random = new Random();
            return random.Next(100);
        }

        #region Static Methods

        /// <summary>
        /// Shows the form.
        /// </summary>
        private static void ShowForm()
        {
            //windowSplash = new frmSplash(true);
            //Application.Run(windowSplash);
        }

        /// <summary>
        /// Closes the form without fade out.
        /// </summary>
        public static void CloseFormWithFadeOut()
        {
            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                windowSplash.closeFormAfterFadeOut = true;
                windowSplash.fadingIn = false;
                windowSplash.fadingOut = true;
            };

            // Check if invoking is necessary
            if (windowSplash.InvokeRequired)
            {
                windowSplash.BeginInvoke(methodUIUpdate);
            }
            else
            {
                methodUIUpdate.Invoke();
            }
        }
        
        /// <summary>
        /// Closes the form with fade out.
        /// </summary>
        public static void CloseForm()
        {
            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                windowSplash.Close();
            };

            // Check if invoking is necessary
            if (windowSplash.InvokeRequired)
            {
                windowSplash.BeginInvoke(methodUIUpdate);
            }
            else
            {
                methodUIUpdate.Invoke();
            }                        
        }

        /// <summary>
        /// Shows the splash screen.
        /// </summary>
        public static void ShowSplash()
        {
            // Make sure it is only launched once.
            if (windowSplash != null)
            {
                return;
            }

            // Run splash in another thread
            thread = new Thread(new ThreadStart(frmSplash.ShowForm));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        /// <summary>
        /// Hides the splash screen.
        /// </summary>
        public static void HideSplash()
        {
            // Make sure window already exists
            if (windowSplash == null)
            {
                return;
            }

            windowSplash.fadingIn = false;
            windowSplash.fadingOut = true;
        }

        ///// <summary>
        ///// Sets the status message of the splash screen.
        ///// </summary>
        ///// <param name="status">Status Message</param>
        //public static void SetStatus(string status)
        //{
        //    windowSplash.status = status;
        //}

        ///// <summary>
        ///// Sets an error message on the splash screen.
        ///// </summary>
        ///// <param name="error">Error Message</param>
        //public static void SetError(string error)
        //{
        //    windowSplash.error = error;
        //}

        #endregion

        #region Background Picture Box Events

        /// <summary>
        /// Occurs when the user clicks on the background picture. Closes the
        /// form if in "About" mode. Opens a browser to the MPfm web site if the
        /// user clicks on the web site link.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void pictureBackground_Click(object sender, EventArgs e)
        {
            // Check if the user clicks the link

            // 320, 260
            // 410, 270

            Point pt = this.PointToClient(System.Windows.Forms.Control.MousePosition);

            if (pt.X >= rectLink.X &&
               pt.X <= rectLink.X + rectLink.Width &&
               pt.Y >= rectLink.Y &&
               pt.Y <= rectLink.Y + rectLink.Height)
            {
                Process.Start("http://www.mp4m.org");
                return;
            }

            // The splash screen can be hidden only if the initialization is done                        
            if (InitDone && !fadingIn)
            {
                closeFormAfterFadeOut = true;
                fadingOut = true;
            }            
        }

        /// <summary>
        /// Occus when the mouse cursor is over the background picture. Displays a hand
        /// cursor when the cursor is in front of the mp4m.org website link.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void pictureBackground_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = this.PointToClient(System.Windows.Forms.Control.MousePosition);

            if (pt.X >= rectLink.X &&
               pt.X <= rectLink.X + rectLink.Width &&
               pt.Y >= rectLink.Y &&
               pt.Y <= rectLink.Y + rectLink.Height)
            {
                if (Cursor.Current == Cursors.Default)
                {
                    Cursor.Current = Cursors.Hand;
                }
            }
            else
            {
                if (Cursor.Current == Cursors.Hand)
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        #endregion

        #region Timer Events

        /// <summary>
        /// This timer updates the status message.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void timerUpdateGUI_Tick(object sender, EventArgs e)
        {
            // Display new status
            if (lblStatus.Text != status)
            {
                lblStatus.Text = status;                
            }

            //// Display error
            //if (lblError.Text != error)
            //{
            //    lblError.Visible = true;
            //    lblError.Text = error;
            //}
        }

        /// <summary>
        /// This timer updates the fade in/fade out.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void timerFading_Tick(object sender, EventArgs e)
        {
            if (fadingIn)
            {
                this.Opacity = this.Opacity + 0.05;

                if (this.Opacity == 1)
                {
                    fadingIn = false;
                    return;
                }
            }

            if (fadingOut)
            {
                this.Opacity = this.Opacity - 0.05;

                if (this.Opacity == 0)
                {
                    fadingOut = false;

                    if (closeFormAfterFadeOut)
                    {                        
                        this.Close();
                    }

                    //windowSplash.Hide();
                }
            }
        }

        #endregion

        #region ISplashView implementation

        public Action<IBaseView> OnViewDestroy { get; set; }

        public void ShowView(bool shown)
        {
        }

        public void RefreshStatus(string message)
        {
            MethodInvoker methodUIUpdate = delegate {
                lblStatus.Text = message;
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        void ISplashView.InitDone(bool isAppFirstStart)
        {
            MethodInvoker methodUIUpdate = Close;

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        #endregion

    }
}