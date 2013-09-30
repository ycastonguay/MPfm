﻿/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.BackgroundAudio;
using System.Collections.Generic;

namespace MyAudioPlaybackAgent
{
    public class AudioPlayer : AudioPlayerAgent
    {
        private static volatile bool _classInitialized;

        // What's the current track?
        static int currentTrackNumber = 0;

        // A playlist made up of AudioTrack items.
        private static List<AudioTrack> _playList = new List<AudioTrack>
        {
            new AudioTrack(new Uri(@"C:\Data\Users\DefApps\AppData\{04E03A6B-A909-40F0-BE84-86ECB9D8E7D5}\Local\Audio\Coltrane John - A Love Supreme - Quartet  Acknowledgement (Part 1).mp3", UriKind.Relative),
                            "A Love Supreme - Acknowledgement (Part 1)", 
                            "John Coltrane", 
                            "A Love Supreme", 
                            null)
                            //new Uri("shared/media/Ring01.jpg", UriKind.Relative)),

            //new AudioTrack(new Uri("Ring01.wma", UriKind.Relative), 
            //                "Ringtone 1", 
            //                "Windows Phone", 
            //                "Windows Phone Ringtones", 
            //                new Uri("shared/media/Ring01.jpg", UriKind.Relative)),

            //new AudioTrack(new Uri("Ring02.wma", UriKind.Relative), 
            //                "Ringtone 2", 
            //                "Windows Phone", 
            //                "Windows Phone Ringtones", 
            //                new Uri("shared/media/Ring02.jpg", UriKind.Relative)),

            //new AudioTrack(new Uri("Ring03.wma", UriKind.Relative), 
            //                "Ringtone 3", 
            //                "Windows Phone", 
            //                "Windows Phone Ringtones", 
            //                new Uri("shared/media/Ring03.jpg", UriKind.Relative)),

            //// A remote URI
            //new AudioTrack(new Uri("http://traffic.libsyn.com/wpradio/WPRadio_29.mp3", UriKind.Absolute), 
            //                "Episode 29", 
            //                "Windows Phone Radio", 
            //                "Windows Phone Radio Podcast",
            //                new Uri("shared/media/Episode29.jpg", UriKind.Relative))
        };


        /// <remarks>
        /// AudioPlayer instances can share the same process. 
        /// Static fields can be used to share state between AudioPlayer instances
        /// or to communicate with the Audio Streaming agent.
        /// </remarks>
        public AudioPlayer()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += AudioPlayer_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void AudioPlayer_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }


        /// <summary>
        /// Increments the currentTrackNumber and plays the correpsonding track.
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        private void PlayNextTrack(BackgroundAudioPlayer player)
        {
            if (++currentTrackNumber >= _playList.Count)
            {
                currentTrackNumber = 0;
            }

            PlayTrack(player);
        }


        /// <summary>
        /// Decrements the currentTrackNumber and plays the correpsonding track.
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        private void PlayPreviousTrack(BackgroundAudioPlayer player)
        {
            if (--currentTrackNumber < 0)
            {
                currentTrackNumber = _playList.Count - 1;
            }

            PlayTrack(player);
        }


        /// <summary>
        /// Plays the track in our playlist at the currentTrackNumber position.
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        private void PlayTrack(BackgroundAudioPlayer player)
        {
            Debug.WriteLine("AudioPlayer - PlayTrack");
            if (PlayState.Paused == player.PlayerState)
            {
                // If we're paused, we already have 
                // the track set, so just resume playing.
                player.Play();
            }
            else
            {
                // Set which track to play. When the TrackReady state is received 
                // in the OnPlayStateChanged handler, call player.Play().
                player.Track = _playList[currentTrackNumber];
            }

        }


        /// <summary>
        /// Called when the playstate changes, except for the Error state (see OnError)
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time the playstate changed</param>
        /// <param name="playState">The new playstate of the player</param>
        /// <remarks>
        /// Play State changes cannot be cancelled. They are raised even if the application
        /// caused the state change itself, assuming the application has opted-in to the callback.
        /// 
        /// Notable playstate events: 
        /// (a) TrackEnded: invoked when the player has no current track. The agent can set the next track.
        /// (b) TrackReady: an audio track has been set and it is now ready for playack.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override void OnPlayStateChanged(BackgroundAudioPlayer player, AudioTrack track, PlayState playState)
        {
            Debug.WriteLine("AudioPlayer - OnPlayStateChanged");
            switch (playState)
            {
                case PlayState.TrackEnded:
                    PlayNextTrack(player);
                    break;

                case PlayState.TrackReady:
                    // The track to play is set in the PlayTrack method.
                    player.Play();
                    break;
            }

            NotifyComplete();
        }


        /// <summary>
        /// Called when the user requests an action using application/system provided UI
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time of the user action</param>
        /// <param name="action">The action the user has requested</param>
        /// <param name="param">The data associated with the requested action.
        /// In the current version this parameter is only for use with the Seek action,
        /// to indicate the requested position of an audio track</param>
        /// <remarks>
        /// User actions do not automatically make any changes in system state; the agent is responsible
        /// for carrying out the user actions if they are supported.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override void OnUserAction(BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param)
        {
            Debug.WriteLine("AudioPlayer - OnUserAction");
            switch (action)
            {
                case UserAction.Play:
                    PlayTrack(player);
                    break;

                case UserAction.Pause:
                    player.Pause();
                    break;

                case UserAction.SkipPrevious:
                    PlayPreviousTrack(player);
                    break;

                case UserAction.SkipNext:
                    PlayNextTrack(player);
                    break;
            }

            NotifyComplete();
        }


        /// <summary>
        /// Called whenever there is an error with playback, such as an AudioTrack not downloading correctly
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track that had the error</param>
        /// <param name="error">The error that occured</param>
        /// <param name="isFatal">If true, playback cannot continue and playback of the track will stop</param>
        /// <remarks>
        /// This method is not guaranteed to be called in all cases. For example, if the background agent 
        /// itself has an unhandled exception, it won't get called back to handle its own errors.
        /// </remarks>
        protected override void OnError(BackgroundAudioPlayer player, AudioTrack track, Exception error, bool isFatal)
        {
            Debug.WriteLine("AudioPlayer - OnError");
            if (isFatal)
            {
                Abort();
            }
            else
            {
                NotifyComplete();
            }

        }

        /// <summary>
        /// Called when the agent request is getting cancelled
        /// </summary>
        /// <remarks>
        /// Once the request is Cancelled, the agent gets 5 seconds to finish its work,
        /// by calling NotifyComplete()/Abort().
        /// </remarks>
        protected override void OnCancel()
        {
            Debug.WriteLine("AudioPlayer - OnCancel");
        }
    }
}
