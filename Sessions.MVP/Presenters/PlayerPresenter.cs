// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Messages;
using Sessions.MVP.Models;
using Sessions.MVP.Navigation;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using TinyMessenger;
using System.Threading.Tasks;
using Sessions.MVP.Config;

#if WINDOWSSTORE
using Windows.UI.Xaml;
#elif WINDOWS_PHONE
using System.Windows.Threading;
#endif

namespace Sessions.MVP.Presenters
{
	/// <summary>
	/// Player presenter.
	/// </summary>
	public class PlayerPresenter : BasePresenter<IPlayerView>, IPlayerPresenter
	{
        readonly MobileNavigationManager _mobileNavigationManager;
        readonly NavigationManager _navigationManager;
        readonly IPlayerService _playerService;
        readonly ILibraryService _libraryService;
        readonly IAudioFileCacheService _audioFileCacheService;
        readonly ITinyMessengerHub _messageHub;
#if WINDOWS_PHONE
        private System.Windows.Threading.DispatcherTimer _timerRefreshSongPosition = null;
        private System.Windows.Threading.DispatcherTimer _timerSavePlayerStatus = null;
        private System.Windows.Threading.DispatcherTimer _timerOutputMeter = null;
#elif WINDOWSSTORE
        private Windows.UI.Xaml.DispatcherTimer _timerRefreshSongPosition = null;
        private Windows.UI.Xaml.DispatcherTimer _timerSavePlayerStatus = null;
        private Windows.UI.Xaml.DispatcherTimer _timerOutputMeter = null;
#else
        private System.Timers.Timer _timerRefreshSongPosition = null;
        private System.Timers.Timer _timerSavePlayerStatus = null;
        private System.Timers.Timer _timerOutputMeter = null;
#endif

        public PlayerPresenter(ITinyMessengerHub messageHub, IPlayerService playerService, IAudioFileCacheService audioFileCacheService, ILibraryService libraryService)
		{	
            _messageHub = messageHub;
            _playerService = playerService;
            _audioFileCacheService = audioFileCacheService;
            _libraryService = libraryService;

#if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
            _timerRefreshSongPosition = new System.Timers.Timer();			
            _timerRefreshSongPosition.Interval = AppConfigManager.Instance.Root.General.SongPositionUpdateFrequency;
			_timerRefreshSongPosition.Elapsed += HandleTimerRefreshSongPositionElapsed;
            _timerSavePlayerStatus = new System.Timers.Timer();          
            _timerSavePlayerStatus.Interval = 5000;
            _timerSavePlayerStatus.Elapsed += HandleTimerSavePlayerStatusElapsed;
            _timerOutputMeter = new System.Timers.Timer();         
            _timerOutputMeter.Interval = AppConfigManager.Instance.Root.General.OutputMeterUpdateFrequency;
            _timerOutputMeter.Elapsed += HandleOutputMeterTimerElapsed;
#else
            _timerRefreshSongPosition = new DispatcherTimer();
            _timerRefreshSongPosition.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _timerRefreshSongPosition.Tick += HandleTimerRefreshSongPositionElapsed;
            _timerSavePlayerStatus = new DispatcherTimer();
            _timerSavePlayerStatus.Interval = new TimeSpan(0, 0, 0, 0, 5000);
            _timerSavePlayerStatus.Tick += HandleTimerSavePlayerStatusElapsed;
            _timerOutputMeter = new DispatcherTimer();
            _timerOutputMeter.Interval = new TimeSpan(0, 0, 0, 0, 40);
            _timerOutputMeter.Tick += HandleOutputMeterTimerElapsed;
#endif

            // Subscribe to events
            _messageHub.Subscribe<GeneralAppConfigChangedMessage>((GeneralAppConfigChangedMessage m) => {
                UpdateTimerRefreshSongPositionInterval(m.Config.SongPositionUpdateFrequency);
                UpdateTimerOutputMeterInterval(m.Config.OutputMeterUpdateFrequency);
            });
            _messageHub.Subscribe<LibraryBrowserItemDoubleClickedMessage>((LibraryBrowserItemDoubleClickedMessage m) => {
                Play(_audioFileCacheService.SelectAudioFiles(m.Query));
            });
            _messageHub.Subscribe<SongBrowserItemDoubleClickedMessage>((SongBrowserItemDoubleClickedMessage m) => {
                string filePath = m != null ? m.Item.FilePath : null;
                Play(_audioFileCacheService.SelectAudioFiles(m.Query), filePath);
            });
            //messageHub.Subscribe<MobileLibraryBrowserItemClickedMessage>((MobileLibraryBrowserItemClickedMessage m) => {
            //    Play(audioFileCacheService.SelectAudioFiles(m.Query), m.FilePath);
            //});
            _messageHub.Subscribe<PlayerPlaylistIndexChangedMessage>((PlayerPlaylistIndexChangedMessage m) => {
                View.RefreshSongInformation(m.Data.AudioFileStarted, _playerService.CurrentPlaylistItem.LengthBytes, 
                        _playerService.CurrentPlaylist.Items.IndexOf(_playerService.CurrentPlaylistItem), _playerService.CurrentPlaylist.Items.Count);

                var markers = _libraryService.SelectMarkers(m.Data.AudioFileStarted.Id);
                View.RefreshMarkers(markers);
            });
            _messageHub.Subscribe<PlayerStatusMessage>((PlayerStatusMessage m) => {
                View.RefreshPlayerStatus(m.Status, _playerService.RepeatType, _playerService.IsShuffleEnabled);

                if(!View.IsOutputMeterEnabled)
                    return;

                switch(m.Status)
                {
                    case PlayerStatusType.Playing:
                        _timerOutputMeter.Start();
                        break;
                    case PlayerStatusType.Paused:
                        _timerOutputMeter.Stop();
                        break;
                    case PlayerStatusType.Stopped:
                        _timerOutputMeter.Stop();
                        View.RefreshSongInformation(null, 0, 0, 0);
                        View.RefreshPlayerPosition(new PlayerPosition());
                        break;
                }
            });
            _messageHub.Subscribe<MarkerUpdatedMessage>((MarkerUpdatedMessage m) => {
                var markers = _libraryService.SelectMarkers(m.AudioFileId);
                View.RefreshMarkers(markers);
            });
            _messageHub.Subscribe<MarkerPositionUpdatedMessage>((MarkerPositionUpdatedMessage m) => {
                View.RefreshMarkerPosition(m.Marker);
            });
            _messageHub.Subscribe<MarkerActivatedMessage>((MarkerActivatedMessage m) => {
                View.RefreshActiveMarker(m.MarkerId);
            });

#if IOS || ANDROID
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
        }
        
        public override void BindView(IPlayerView view)
        {
            base.BindView(view);
            
            view.OnPlayerPlay = Play;
            view.OnPlayerPause = Pause;
            view.OnPlayerStop = Stop;
            view.OnPlayerPrevious = Previous;
            view.OnPlayerNext = Next;
            view.OnPlayerShuffle = Shuffle;
            view.OnPlayerRepeat = Repeat;
            view.OnPlayerSetPosition = SetPosition;
            view.OnPlayerSetVolume = SetVolume;
            view.OnPlayerRequestPosition = RequestPosition;
            view.OnEditSongMetadata = EditSongMetadata;
            view.OnOpenPlaylist = OpenPlaylist;
            view.OnOpenEffects = OpenEffects;
            view.OnPlayerViewAppeared = PlayerViewAppeared;

            // If the player is already playing, refresh initial data
            if (_playerService.IsPlaying)
            {
                View.RefreshSongInformation(_playerService.CurrentPlaylistItem.AudioFile, _playerService.CurrentPlaylistItem.LengthBytes,
                            _playerService.CurrentPlaylist.Items.IndexOf(_playerService.CurrentPlaylistItem), _playerService.CurrentPlaylist.Items.Count);

                var markers = _libraryService.SelectMarkers(_playerService.CurrentPlaylistItem.AudioFile.Id);
                View.RefreshMarkers(markers);
            }

            #if !IOS
            if (_playerService.Status == PlayerStatusType.WaitingToStart || 
                _playerService.Status == PlayerStatusType.StartPaused)
            {
                _playerService.Resume();
                _timerRefreshSongPosition.Start();
                _timerSavePlayerStatus.Start();
            }
            #endif

            View.RefreshPlayerVolume(new PlayerVolume() {
                Volume = 100,
                VolumeString = "100%"
            });
            View.RefreshPlayerStatus(_playerService.Status, _playerService.RepeatType, _playerService.IsShuffleEnabled);

            if (_playerService.IsPlaying && View.IsOutputMeterEnabled)
                _timerOutputMeter.Start();
        }

	    public override void ViewDestroyed()
	    {
	        base.ViewDestroyed();

            #if !IOS && !ANDROID
            _playerService.Dispose();
            #endif
        }

        private void UpdateTimerRefreshSongPositionInterval(int interval)
        {
            if(_timerRefreshSongPosition.Interval != interval)
            {
                bool isRunning = _timerRefreshSongPosition.Enabled;
                if(isRunning)
                    _timerRefreshSongPosition.Stop();
                _timerRefreshSongPosition.Interval = interval;
                if(isRunning)
                    _timerRefreshSongPosition.Start();
            }
        }

        private void UpdateTimerOutputMeterInterval(int interval)
        {
            if (_timerOutputMeter.Interval != interval)
            {
                bool isRunning = _timerOutputMeter.Enabled;
                if(isRunning)
                    _timerOutputMeter.Stop();
                _timerOutputMeter.Interval = interval;
                if(isRunning)
                    _timerOutputMeter.Start();
            }
        }

        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        private void HandleTimerRefreshSongPositionElapsed(object sender, System.Timers.ElapsedEventArgs e)
        #else
        private void HandleTimerRefreshSongPositionElapsed(object sender, object eventArgs)
        #endif
		{
            //int available = playerService.GetDataAvailable();
			PlayerPosition entity = new PlayerPosition();
		    try
		    {
                // This might throw an exception when the application is closing
                if (_playerService.IsSettingPosition || _playerService.Status != PlayerStatusType.Playing)
                    return;

		        entity = _playerService.GetPosition();
		    }
		    catch (Exception ex)
		    {
		        Tracing.Log(string.Format("PlayerPresenter - HandleTimerRefreshSongPositionElapsed - Failed to get player position: {0}", ex));
		    }

			View.RefreshPlayerPosition(entity);
		}

        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        private void HandleTimerSavePlayerStatusElapsed(object sender, System.Timers.ElapsedEventArgs e)
        #else
        private void HandleTimerSavePlayerStatusElapsed(object sender, object eventArgs)
        #endif
        {
            if(_playerService.IsSettingPosition || !_playerService.IsPlaying)
                return;

            PlayerPosition entity = new PlayerPosition();
            try
            {
                entity = _playerService.GetPosition();
                Task.Factory.StartNew(() => {
                    try
                    {
                        // Store player status locally for resuming playback later
                        AppConfigManager.Instance.Root.ResumePlayback.AudioFileId = _playerService.CurrentPlaylistItem.AudioFile.Id.ToString();
                        AppConfigManager.Instance.Root.ResumePlayback.PlaylistId = _playerService.CurrentPlaylist.PlaylistId.ToString();
                        AppConfigManager.Instance.Root.ResumePlayback.PositionPercentage = (float)entity.PositionBytes / (float)_playerService.CurrentPlaylistItem.LengthBytes;
                        AppConfigManager.Instance.Root.ResumePlayback.Timestamp = DateTime.Now;
                        AppConfigManager.Instance.Save();
                    }
                    catch (Exception ex)
                    {
                        Tracing.Log("PlayerPresenter - HandleTimerSavePlayerStatusElapsed - Failed to save local resume playback info: {0}", ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Tracing.Log(string.Format("PlayerPresenter - HandleTimerSavePlayerStatusElapsed - Failed to get player position: {0}", ex));
            }
        }


        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        private void HandleOutputMeterTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        #else
        private void HandleOutputMeterTimerElapsed(object sender, object eventArgs)
        #endif
        {
            try
            {
                if (_playerService.UseFloatingPoint)
                {
                    Tuple<float[], float[]> data = _playerService.GetFloatingPointMixerData(0.02);
                    View.RefreshOutputMeter(data.Item1, data.Item2);
                }
                else
                {
                    Tuple<short[], short[]> data = _playerService.GetMixerData(0.02);

                    // Convert to floats (TODO: Try to optimize this. I'm sure there's a clever way to do this faster.
                    float[] left = new float[data.Item1.Length];
                    float[] right = new float[data.Item1.Length];
                    for (int a = 0; a < data.Item1.Length; a++)
                    {
                        // The values are already negative to positive, it's just a matter of dividing the value by the max value to get it to -1/+1.
                        left[a] = (float)data.Item1[a] / (float)Int16.MaxValue;
                        right[a] = (float)data.Item2[a] / (float)Int16.MaxValue;
                        //Console.WriteLine("EQPresetPresenter - a: {0} value: {1} newValue: {2}", a, data.Item1[a], left[a]);
                    }

                    View.RefreshOutputMeter(left, right);
                }
            }
            catch(Exception ex)
            {
                // Log a soft error
                Tracing.Log("EqualizerPresetsPresenter - Error fetching output meter data: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

	    public void EditSongMetadata()
	    {
#if IOS || ANDROID
            // Not available yet on mobile devices
#else
            if(_playerService.CurrentPlaylistItem != null)
	            _navigationManager.CreateEditSongMetadataView(_playerService.CurrentPlaylistItem.AudioFile);
#endif
	    }

        private void OpenPlaylist()
        {
            _mobileNavigationManager.CreatePlaylistView(View);
        }

        private void OpenEffects()
        {
            _mobileNavigationManager.CreateEqualizerPresetsView(View);
        }

	    /// <summary>
		/// Starts playback.
		/// </summary>
        private void Play()
		{            
            try
            {
                _playerService.Play();
    			_timerRefreshSongPosition.Start();
                _timerSavePlayerStatus.Start();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
		}
		
		/// <summary>
		/// Starts the playback of a new playlist.
		/// </summary>
		/// <param name='audioFiles'>List of audio files</param>		
        private void Play(IEnumerable<AudioFile> audioFiles)
		{
            try
            {
                _playerService.Play(audioFiles, string.Empty, 0, false, false);
                _timerRefreshSongPosition.Start();
                _timerSavePlayerStatus.Start();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
		}

		/// <summary>
		/// Starts the playback of a new playlist.
		/// </summary>
		/// <param name='filePaths'>List of audio file paths</param>
        private void Play(IEnumerable<string> filePaths)
		{
            try
            {
                _playerService.Play(filePaths);
                _timerRefreshSongPosition.Start();
                _timerSavePlayerStatus.Start();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
		}
		
		/// <summary>
		/// Starts the playback of a new playlist at a specific position.
		/// </summary>
		/// <param name='audioFiles'>List of audio files</param>
		/// <param name='startAudioFilePath'>File path of the first playlist item to play</param>
        private void Play(IEnumerable<AudioFile> audioFiles, string startAudioFilePath)
		{
            try
            {
                _playerService.Play(audioFiles, startAudioFilePath, 0, false, false);
                _timerRefreshSongPosition.Start();
                _timerSavePlayerStatus.Start();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }                
		}
		
		/// <summary>
		/// Stops playback.
		/// </summary>
        private void Stop()
		{
            try
            {
				_timerRefreshSongPosition.Stop();
                _timerSavePlayerStatus.Stop();
                _playerService.Stop();

			    View.RefreshSongInformation(null, 0, 0, 0);
                View.RefreshPlayerPosition(new PlayerPosition());
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
        }
		
		/// <summary>
		/// Pauses playback.
		/// </summary>
        private void Pause()
		{
            try
            {
                _playerService.Pause();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
		}
		
		/// <summary>
		/// Skips to the next song in the playlist.
		/// </summary>
        private void Next()
		{
            try
            {
                Tracing.Log("PlayerPresenter.Next -- Skipping to next item in playlist...");
                _playerService.Next();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
        }
		
		/// <summary>
		/// Skips to the previous song in the playlist.
		/// </summary>
		public void Previous()
		{            
            try
            {
                Tracing.Log("PlayerPresenter.Previous -- Skipping to previous item in playlist...");
                _playerService.Previous();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }                
		}
		
        private void Shuffle()
        {
            _playerService.IsShuffleEnabled = !_playerService.IsShuffleEnabled;
            View.RefreshPlayerStatus(_playerService.Status, _playerService.RepeatType, _playerService.IsShuffleEnabled);
        }

        private void Repeat()
        {
            _playerService.ToggleRepeatType();
            View.RefreshPlayerStatus(_playerService.Status, _playerService.RepeatType, _playerService.IsShuffleEnabled);
        }

        private void PlayerViewAppeared()
        {
            if (_playerService.Status == PlayerStatusType.WaitingToStart || 
                _playerService.Status == PlayerStatusType.StartPaused)
            {
                _playerService.Resume();
                _timerRefreshSongPosition.Start();
                _timerSavePlayerStatus.Start();
            }
        }

        private PlayerPosition RequestPosition(float positionPercentage)
        {
            try
            {
                //Tracing.Log("PlayerPresenter - RequestPosition - positionPercentage: {0}", positionPercentage);
                // Calculate new position from 0.0f/1.0f scale
                long lengthBytes = _playerService.CurrentPlaylistItem.LengthBytes;
                var audioFile = _playerService.CurrentPlaylistItem.AudioFile;
                long positionBytes = (long)(positionPercentage * lengthBytes);
                //long positionBytes = (long)Math.Ceiling((double)Playlist.CurrentItem.LengthBytes * (percentage / 100));
                long positionSamples = ConvertAudio.ToPCM(positionBytes, (uint)audioFile.BitsPerSample, audioFile.AudioChannels);
                int positionMS = (int)ConvertAudio.ToMS(positionSamples, (uint)audioFile.SampleRate);
                string positionString = Conversion.MillisecondsToTimeString((ulong)positionMS);
                PlayerPosition entity = new PlayerPosition();
                entity.Position = positionString;
                entity.PositionBytes = positionBytes;
                entity.PositionSamples = (uint)positionSamples;
                return entity;
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while calculating the player position: " + ex.Message);
                View.PlayerError(ex);
            }
            return new PlayerPosition();
        }
        
        private void SetPosition(float percentage)
        {
            try
            {
                // Make sure the percentage isn't 100% or BASS will return an error.               
                double pct = (double)percentage;
                if(pct > 99.9)
                    pct = 99.9;

                Tracing.Log("PlayerPresenter.SetPosition -- Setting position to " + percentage.ToString("0.00") + "%");
                _timerRefreshSongPosition.Stop();
                _timerSavePlayerStatus.Stop();
                _playerService.SetPosition(pct);
                _timerRefreshSongPosition.Start();
                _timerSavePlayerStatus.Start();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
        }

        private void SetVolume(float volume)
        {
            try
            {
                //Tracing.Log("PlayerPresenter.SetVolume -- Setting volume to " + volume.ToString("0.00") + "%");
                _playerService.Volume = volume / 100;
                View.RefreshPlayerVolume(new PlayerVolume(){ 
                    Volume = volume, 
                    VolumeString = volume.ToString("0") + " %" 
                });
            }
            catch(Exception ex)
            {
                SetError(ex);
            }                
        }

        private void SetError(Exception ex)
        {
            View.PlayerError(ex);
        }
	}
}

