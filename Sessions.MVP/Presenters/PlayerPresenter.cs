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
using System.Threading.Tasks;
using org.sessionsapp.player;
using Sessions.Core;
using Sessions.Library.Objects;
using Sessions.Library.Services.Interfaces;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Config;
using Sessions.MVP.Messages;
using Sessions.MVP.Models;
using Sessions.MVP.Navigation;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
using Sessions.Sound.AudioFiles;
using TinyMessenger;
using Sessions.Sound.Objects;

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
                var item = _playerService.Playlist.GetCurrentItem();
                View.RefreshSongInformation(new SongInformationEntity() {
                    AudioFile = m.Data.AudioFileStarted,
                    UseFloatingPoint = _playerService.Mixer.UseFloatingPoint,
                    PlaylistIndex = _playerService.Playlist.CurrentIndex,
                    PlaylistCount = _playerService.Playlist.Count
                });

                var markers = _libraryService.SelectMarkers(m.Data.AudioFileStarted.Id);
                View.RefreshMarkers(markers);
            });
            _messageHub.Subscribe<PlayerStatusMessage>((PlayerStatusMessage m) => {
                View.RefreshPlayerState(m.State, _playerService.RepeatType, _playerService.IsShuffleEnabled);

                if(!View.IsOutputMeterEnabled)
                    return;

                switch(m.State)
                {
                    case SSPPlayerState.Playing:                        
                        _timerOutputMeter.Start();
                        break;
                    case SSPPlayerState.Paused:
                        _timerOutputMeter.Stop();
                        break;
                    case SSPPlayerState.Stopped:                        
                        _timerOutputMeter.Stop();
                        View.RefreshSongInformation(new SongInformationEntity());
                        View.RefreshPlayerPosition(SSPPosition.Empty);
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
            view.OnOpenSelectAlbumArt = OpenSelectAlbumArt;
            view.OnPlayerViewAppeared = PlayerViewAppeared;
            view.OnApplyAlbumArtToSong = ApplyAlbumArtToSong;
            view.OnApplyAlbumArtToAlbum = ApplyAlbumArtToAlbum;

            // If the player is already playing, refresh initial data
            if (_playerService.State == SSPPlayerState.Playing)
            {
                View.RefreshPlaylist(_playerService.Playlist);
                View.RefreshSongInformation(new SongInformationEntity() {
                    AudioFile = _playerService.CurrentAudioFile,
                    UseFloatingPoint = _playerService.Mixer.UseFloatingPoint,
                    PlaylistIndex = _playerService.Playlist.CurrentIndex,
                    PlaylistCount = _playerService.Playlist.Count
                });

                var markers = _libraryService.SelectMarkers(_playerService.CurrentAudioFile.Id);
                View.RefreshMarkers(markers);
            }

//            #if !IOS
//            if (_playerService.Status == PlayerStatusType.WaitingToStart || 
//                _playerService.Status == PlayerStatusType.StartPaused)
//            {
//                _playerService.Resume();
//                _timerRefreshSongPosition.Start();
//                _timerSavePlayerStatus.Start();
//            }
//            #endif

            View.RefreshPlayerVolume(new PlayerVolume() {
                Volume = 100,
                VolumeString = "100%"
            });
            View.RefreshPlayerState(_playerService.State, _playerService.RepeatType, _playerService.IsShuffleEnabled);

            if (_playerService.State == SSPPlayerState.Playing && View.IsOutputMeterEnabled)
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
            if (_playerService.State == SSPPlayerState.Playing)
            {
                var position = _playerService.GetPosition();
                View.RefreshPlayerPosition(position);
            }

            if(View.IsPlayerPerformanceEnabled)
            {
                float cpu = _playerService.GetCPU();
                UInt32 bufferDataAvailable = _playerService.GetBufferDataAvailable();
                View.RefreshPlayerPerformance(cpu, bufferDataAvailable);
            }
		}

        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        private void HandleTimerSavePlayerStatusElapsed(object sender, System.Timers.ElapsedEventArgs e)
        #else
        private void HandleTimerSavePlayerStatusElapsed(object sender, object eventArgs)
        #endif
        {
//            if(_playerService.IsSettingPosition || _playerService.State != SSPPlayerState.Playing)
//                return;
//
//            var position = new SSP_POSITION();
//            try
//            {
//                position = _playerService.GetPosition();
//                Task.Factory.StartNew(() => {
//                    try
//                    {
//                        // Store player status locally for resuming playback later
//                        AppConfigManager.Instance.Root.ResumePlayback.AudioFileId = _playerService.CurrentAudioFile.Id.ToString();
//                        //AppConfigManager.Instance.Root.ResumePlayback.PlaylistId = _playerService.CurrentPlaylist.PlaylistId.ToString();
//                        //AppConfigManager.Instance.Root.ResumePlayback.PositionPercentage = (float)position.bytes / (float)_playerService.CurrentPlaylistItem.LengthBytes;
//                        AppConfigManager.Instance.Root.ResumePlayback.Timestamp = DateTime.Now;
//                        AppConfigManager.Instance.Save();
//                    }
//                    catch (Exception ex)
//                    {
//                        Tracing.Log("PlayerPresenter - HandleTimerSavePlayerStatusElapsed - Failed to save local resume playback info: {0}", ex);
//                    }
//                });
//            }
//            catch (Exception ex)
//            {
//                Tracing.Log(string.Format("PlayerPresenter - HandleTimerSavePlayerStatusElapsed - Failed to get player position: {0}", ex));
//            }
        }

        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        private void HandleOutputMeterTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        #else
        private void HandleOutputMeterTimerElapsed(object sender, object eventArgs)
        #endif
        {
            try
            {
                if (_playerService.Mixer.UseFloatingPoint)
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
            catch (Exception ex)
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
            if(_playerService.CurrentAudioFile != null)
	            _navigationManager.CreateEditSongMetadataView(_playerService.CurrentAudioFile);
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

        private void OpenSelectAlbumArt()
        {
            if (_navigationManager != null)
                _navigationManager.CreateSelectAlbumArtView();
            else if(_mobileNavigationManager != null)
                _mobileNavigationManager.CreateSelectAlbumArtView();
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
                View.RefreshPlaylist(_playerService.Playlist);
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
                View.RefreshPlaylist(_playerService.Playlist);
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
                View.RefreshPlaylist(_playerService.Playlist);
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

//			    View.RefreshSongInformation(null, 0, 0);
//                View.RefreshPlayerPosition(SSP.EmptyPosition);
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
            View.RefreshPlayerState(_playerService.State, _playerService.RepeatType, _playerService.IsShuffleEnabled);
        }

        private void Repeat()
        {
            _playerService.ToggleRepeatType();
            View.RefreshPlayerState(_playerService.State, _playerService.RepeatType, _playerService.IsShuffleEnabled);
        }

        private void PlayerViewAppeared()
        {
            if(!_timerRefreshSongPosition.Enabled)
                _timerRefreshSongPosition.Start();

            if(!_timerSavePlayerStatus.Enabled)
                _timerSavePlayerStatus.Start();
        }

        private SSPPosition RequestPosition(float positionPercentage)
        {
            try
            {
                return _playerService.GetPositionFromPercentage(positionPercentage);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while calculating the player position: " + ex.Message);
                View.PlayerError(ex);
            }

            return SSPPosition.Empty;
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

        private void ApplyAlbumArtToSong(byte[] imageData)
        {
            try
            {
                var audioFile = _playerService.CurrentAudioFile;
                AudioFile.SetAlbumArtForAudioFile(audioFile.FilePath, imageData);
            }
            catch(Exception ex)
            {
                SetError(ex);
            }                
        }

        private void ApplyAlbumArtToAlbum(byte[] imageData)
        {
            try
            {
                var currentAudioFile = _playerService.CurrentAudioFile;
                var audioFiles = _audioFileCacheService.SelectAudioFiles(new LibraryQuery(){
                    ArtistName = currentAudioFile.ArtistName,
                    AlbumTitle = currentAudioFile.AlbumTitle
                });

                foreach(var audioFile in audioFiles)
                    AudioFile.SetAlbumArtForAudioFile(currentAudioFile.FilePath, imageData);
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

