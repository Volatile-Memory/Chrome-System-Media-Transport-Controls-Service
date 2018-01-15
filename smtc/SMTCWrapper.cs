using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;

namespace smtc
{
    /// <summary>
    /// Implements System Media Transport Controls with publicly settable properties
    /// </summary>
    class SMTCWrapper
    {
        static TraceSwitch generalSwitch = new TraceSwitch("General", "Entire Application");

        private SystemMediaTransportControlsDisplayUpdater _updater;
        private SystemMediaTransportControls _systemMediaTransportControls;
        private MediaPlayer _mediaPlayer;

        #region Properties
        private string _status = "Uninitialized Status";
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                Debug.WriteLine(value); //TODO
                OnPropertyChanged("Status");
            }
        }

        private RandomAccessStreamReference _albumart;
        public RandomAccessStreamReference AlbumArt
        {
            get { return _albumart; }
            set
            {
                _albumart = value;
                _updater.Thumbnail = value;
                _updater.Update();
                OnPropertyChanged("AlbumArt");
            }
        }

        private string _artist      = "artist";
        public string Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                _updater.MusicProperties.Artist = value;
                _updater.Update();
                OnPropertyChanged("Artist");
            }
        }

        private string _albumArtist = "album artist";
        public string AlbumArtist
        {
            get { return _albumArtist; }
            set
            {
                _albumArtist = value;
                _updater.MusicProperties.AlbumArtist = value;
                _updater.Update();
                OnPropertyChanged("AlbumArtist");
            }
        }

        private string _title       = "song title";
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                _updater.MusicProperties.Title = value;
                _updater.Update();
                OnPropertyChanged("Title");
            }
        }
#endregion

        public SMTCWrapper()
        {
            _mediaPlayer = new MediaPlayer();
            _systemMediaTransportControls = _mediaPlayer.SystemMediaTransportControls;
            _updater = _systemMediaTransportControls.DisplayUpdater;

            _updater.Type = MediaPlaybackType.Music;

            _mediaPlayer.CommandManager.IsEnabled = false; // TODO: Figure out what this is doing and if it needs to be enabled or if it can be disabled and worked around

            _systemMediaTransportControls.IsEnabled = true;

            _systemMediaTransportControls.IsPlayEnabled = true;
            _systemMediaTransportControls.IsPauseEnabled = true;
            _systemMediaTransportControls.IsStopEnabled = true;


            _systemMediaTransportControls.IsPreviousEnabled = true;
            _systemMediaTransportControls.IsRewindEnabled = true;

            _systemMediaTransportControls.IsNextEnabled = true;
            _systemMediaTransportControls.IsFastForwardEnabled = true;


            _systemMediaTransportControls.PlaybackRate = 1.0;

            _systemMediaTransportControls.IsChannelDownEnabled = true;
            _systemMediaTransportControls.IsChannelUpEnabled = true;


            _systemMediaTransportControls.IsRecordEnabled = true;

            _systemMediaTransportControls.ShuffleEnabled = true;

            _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;

            
            _systemMediaTransportControls.AutoRepeatModeChangeRequested     += _systemMediaTransportControls_AutoRepeatModeChangeRequested; ;
            _systemMediaTransportControls.ButtonPressed                     += _systemMediaTransportControls_ButtonPressed;
            _systemMediaTransportControls.PlaybackPositionChangeRequested   += _systemMediaTransportControls_PlaybackPositionChangeRequested;
            _systemMediaTransportControls.PlaybackRateChangeRequested       += _systemMediaTransportControls_PlaybackRateChangeRequested;
            _systemMediaTransportControls.PropertyChanged                   += _systemMediaTransportControls_PropertyChanged;
            _systemMediaTransportControls.ShuffleEnabledChangeRequested     += _systemMediaTransportControls_ShuffleEnabledChangeRequested;

        }

        private void _systemMediaTransportControls_AutoRepeatModeChangeRequested(SystemMediaTransportControls sender, AutoRepeatModeChangeRequestedEventArgs args)
        {
            switch (args.RequestedAutoRepeatMode)
            {
                case MediaPlaybackAutoRepeatMode.None:
                    //if (sender.AutoRepeatMode == MediaPlaybackAutoRepeatMode.List)
                    //{

                    //}
                    break;
                case MediaPlaybackAutoRepeatMode.Track:
                    break;
                case MediaPlaybackAutoRepeatMode.List:
                    break;
                default:
                    break;
            }
        }

        private void _systemMediaTransportControls_PlaybackPositionChangeRequested(SystemMediaTransportControls sender, PlaybackPositionChangeRequestedEventArgs args)
        {
            //args.RequestedPlaybackPosition;
        }

        private void _systemMediaTransportControls_ShuffleEnabledChangeRequested(SystemMediaTransportControls sender, ShuffleEnabledChangeRequestedEventArgs args)
        {
            //args.RequestedShuffleEnabled; 
        }

        private void _systemMediaTransportControls_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {
            //args.Property
            //TODO: is this sound level change events?
        }

        private void _systemMediaTransportControls_PlaybackRateChangeRequested(SystemMediaTransportControls sender, PlaybackRateChangeRequestedEventArgs args)
        {
            //args.RequestedPlaybackRate;
        }

        private void _systemMediaTransportControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    Status = "Play Button Pressed";
                    if (sender.IsPlayEnabled)
                    {
                          PressedPlayButton();
                    }
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    Status = "Pause Button Pressed";
                    if (sender.IsPauseEnabled)
                    {
                          PressedPauseButton();
                    }
                    break;
                case SystemMediaTransportControlsButton.Stop:
                    Status = "Stop Button Pressed";
                    if (sender.IsStopEnabled)
                    {
                          PressedStopButton();
                    }
                    break;
                case SystemMediaTransportControlsButton.Record:
                    Status = "Record Button Pressed";
                    if (sender.IsRecordEnabled)
                    {
                          PressedRecordButton();
                    }
                    break;
                case SystemMediaTransportControlsButton.FastForward:
                    Status = "FastForward Button Pressed";
                    if (sender.IsFastForwardEnabled)
                    {
                          PressedFastForwardButton();
                    }
                    break;
                case SystemMediaTransportControlsButton.Rewind:
                    Status = "Rewind Button Pressed";
                    if (sender.IsRewindEnabled)
                    {
                          PressedRewindButton();
                    }
                    break;
                case SystemMediaTransportControlsButton.Next:
                    Status = "Next Button Pressed";
                    if (sender.IsNextEnabled)
                    {
                          PressedNextButton();
                    }
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    Status = "Previous Button Pressed";
                    if (sender.IsPreviousEnabled)
                    {
                          PressedPreviousButton();
                    }
                    break;
                case SystemMediaTransportControlsButton.ChannelUp:
                    Status = "ChannelUp Button Pressed";
                    if (sender.IsChannelUpEnabled)
                    {
                          PressedChannelUpButton();
                    }
                    break;
                case SystemMediaTransportControlsButton.ChannelDown:
                    Status = "ChannelDown Button Pressed";
                    if (sender.IsChannelDownEnabled)
                    {
                          PressedChannelDownButton();
                    }
                    break;
                default:
                    break;
            }
        }

        private void PressedChannelDownButton()
        {
        }

        private void PressedChannelUpButton()
        {
        }

        private void PressedPreviousButton()
        {
        }

        private void PressedNextButton()
        {
        }

        private void PressedRewindButton()
        {
        }

        private void PressedFastForwardButton()
        {
        }

        private void PressedRecordButton()
        {
        }

        private void PressedStopButton()
        {
        }

        private void PressedPauseButton()
        {
        }

        private void PressedPlayButton()
        {
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
