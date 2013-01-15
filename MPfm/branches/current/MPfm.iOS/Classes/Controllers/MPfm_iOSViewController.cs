using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.Player;
using MPfm.Sound.BassNetWrapper;
using MPfm.Sound.BassWrapper;
using System.IO;
using System.Timers;
using MPfm.Sound;
using MPfm.Core;

namespace MPfm.iOS
{
	public partial class MPfm_iOSViewController : UIViewController
	{
		private IPlayer player;
        private Timer timer;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public MPfm_iOSViewController ()
			: base (UserInterfaceIdiomIsPhone ? "MPfm_iOSViewController_iPhone" : "MPfm_iOSViewController_iPad", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
            timer = new Timer();
            timer.Interval = 100;
            timer.Elapsed += (sender, e) => {
                InvokeOnMainThread(() => {
                    try
                    {
                        long bytes = MPfm.Player.Player.CurrentPlayer.GetPosition();
                        long samples = ConvertAudio.ToPCM(bytes, (uint)MPfm.Player.Player.CurrentPlayer.Playlist.CurrentItem.AudioFile.BitsPerSample, 2);
                        long ms = ConvertAudio.ToMS(samples, (uint)MPfm.Player.Player.CurrentPlayer.Playlist.CurrentItem.AudioFile.SampleRate);
                        string pos = Conversion.MillisecondsToTimeString((ulong)ms);
                        lblPosition.Text = pos + " / " + player.Playlist.CurrentItem.LengthString;
                    }
                    catch
                    {
                        lblPosition.Text = "Error";
                    }
                });
            };

			// Perform any additional setup after loading the view, typically from a nib.
			Device device = new Device(){
				DriverType = DriverType.DirectSound,
				Id = -1
			};
            string test = BassWrapperGlobals.DllImportValue_Bass;
			player = new MPfm.Player.Player(device, 44100, 5000, 100, true);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string path2 = NSBundle.MainBundle.BundlePath;
            string filePath = Path.Combine(path2, "01.flac");
            string filePath2 = Path.Combine(path2, "02.flac");
            string filePath3 = Path.Combine(path2, "03.flac");
            string filePath4 = Path.Combine(path2, "04.flac");
            string filePath5 = Path.Combine(path2, "05.flac");
            string filePath6 = Path.Combine(path2, "06.flac");
            //filePath = filePath.Replace("/Documents", "");
            bool exists = File.Exists(filePath);
            player.PlayFiles(new List<string> { filePath, filePath2, filePath3, filePath4, filePath5, filePath6 });
            player.OnPlaylistIndexChanged += (data) => {
                InvokeOnMainThread(() => {
                    RefreshAudioFile(data.AudioFileStarted);
                });
            };
            timer.Start();
            RefreshAudioFile(player.Playlist.CurrentItem.AudioFile);
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			if (UserInterfaceIdiomIsPhone) {
				return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			} else {
				return true;
			}
		}

        private void RefreshAudioFile(AudioFile audioFile)
        {
            lblArtistName.Text = audioFile.ArtistName;
            lblAlbumTitle.Text = audioFile.AlbumTitle;
            lblTitle.Text = audioFile.Title;
        }

        partial void actionPlay(NSObject sender)
        {
            player.Play();
        }

        partial void actionPause(NSObject sender)
        {
            player.Pause();
        }

        partial void actionStop(NSObject sender)
        {
            player.Stop();
        }

        partial void actionPrevious(NSObject sender)
        {
            player.Previous();
        }

        partial void actionNext(NSObject sender)
        {
            player.Next();
        }
	}
}

