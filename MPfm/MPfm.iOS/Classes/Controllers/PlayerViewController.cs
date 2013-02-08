using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.Player;
using MPfm.Sound.BassWrapper;
using System.IO;
using System.Timers;
using MPfm.Sound;
using MPfm.Core;
using System.Linq;
using MonoTouch.CoreGraphics;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Bass.Net;

namespace MPfm.iOS
{
	public partial class PlayerViewController : BaseViewController
	{
		private IPlayer player;
        private Timer timer;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public PlayerViewController()
			: base (UserInterfaceIdiomIsPhone ? "PlayerViewController_iPhone" : "PlayerViewController_iPad", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Set fonts
            lblArtistName.Font = UIFont.FromName("OstrichSans-Black", 28);
            lblAlbumTitle.Font = UIFont.FromName("OstrichSans-Medium", 24);
            lblTitle.Font = UIFont.FromName("OstrichSans-Medium", 18);
            lblPosition.Font = UIFont.FromName("OstrichSans-Black", 18);
            lblLength.Font = UIFont.FromName("OstrichSans-Black", 18);
            btnPrevious.Font = UIFont.FromName("OstrichSans-Black", 18);
            btnPlayPause.Font = UIFont.FromName("OstrichSans-Black", 18);
            btnNext.Font = UIFont.FromName("OstrichSans-Black", 18);
			
            // Reduce the song position slider size
            sliderPosition.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);
            sliderPosition.Frame = new RectangleF(70, sliderPosition.Frame.Y, 180, sliderPosition.Frame.Height);

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
                        lblPosition.Text = pos;
                        sliderPosition.Value = ms;
                    } catch
                    {
                        lblPosition.Text = "0:00.000";
                    }
                });
            };

            // Initialize player
            Device device = new Device(){
				DriverType = DriverType.DirectSound,
				Id = -1
			};
            player = new MPfm.Player.Player(device, 44100, 5000, 100, true);
            Play();
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

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Now Playing");
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

        private void RefreshAudioFile(AudioFile audioFile, bool isSameAlbum)
        {
            InvokeOnMainThread(() => {
                if(!isSameAlbum)
                {
                    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                    NSData imageData = NSData.FromArray(bytesImage);
                    UIImage image = UIImage.LoadFromData(imageData);
                    imageViewAlbumArt.Image = image;
                }

                lblArtistName.Text = audioFile.ArtistName;
                lblAlbumTitle.Text = audioFile.AlbumTitle;
                lblTitle.Text = audioFile.Title;
                lblLength.Text = player.Playlist.CurrentItem.LengthString;
                sliderPosition.MaxValue = player.Playlist.CurrentItem.LengthMilliseconds;
            });
        }

        private void Play()
        {
            // Add files to play
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            List<string> listFiles = Directory.EnumerateFiles(documentsPath).ToList();
            
            if (listFiles.Count > 0)
            {
                player.PlayFiles(listFiles);
            } 
            else
            {
                string path2 = NSBundle.MainBundle.BundlePath;
                string filePath = Path.Combine(path2, "01.mp3");                
                string filePath2 = Path.Combine(path2, "02.mp3");
                string filePath3 = Path.Combine(path2, "03.mp3");
                string filePath4 = Path.Combine(path2, "04.mp3");
                string filePath5 = Path.Combine(path2, "05.mp3");
                player.PlayFiles(new List<string> { filePath, filePath2, filePath3, filePath4, filePath5 });
            }
            
            player.OnPlaylistIndexChanged += (data) => {
                if(data.AudioFileEnded != null &&
                   data.AudioFileEnded.ArtistName == data.AudioFileStarted.ArtistName &&
                   data.AudioFileEnded.AlbumTitle == data.AudioFileStarted.AlbumTitle)
                {
                    RefreshAudioFile(data.AudioFileStarted, true);
                }
                else
                {
                    RefreshAudioFile(data.AudioFileStarted, false);
                }
            };
            timer.Start();
            RefreshAudioFile(player.Playlist.CurrentItem.AudioFile, false);
        }

        partial void actionPause(NSObject sender)
        {
            player.Pause();
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

