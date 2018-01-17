using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickType;
using System;
using System.Diagnostics;
using System.IO;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace smtc
{
    /// <summary>
    /// System Media Transport Control API Wrapper
    /// Allows control from JSON stubs over STDIO (e.g. Google Chrome Native Messaging)
    /// </summary>
    internal class SystemMediaTransportControlsWrapper
    {
        private MediaPlayer _mediaPlayer;
        private SystemMediaTransportControls _smtc;

        public SystemMediaTransportControlsWrapper()
        {        
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.CommandManager.IsEnabled = false; // TODO: Figure out what this is doing and if it needs to be enabled or if it can be disabled and worked
            _smtc = _mediaPlayer.SystemMediaTransportControls;

            _smtc.AutoRepeatModeChangeRequested     += _SMTC_AutoRepeatModeChangeRequested; ;
            _smtc.ButtonPressed                     += _SMTC_ButtonPressed;
            _smtc.PlaybackPositionChangeRequested   += _SMTC_PlaybackPositionChangeRequested;
            _smtc.PlaybackRateChangeRequested       += _SMTC_PlaybackRateChangeRequested;
            _smtc.PropertyChanged                   += _SMTC_PropertyChanged;
            _smtc.ShuffleEnabledChangeRequested     += _SMTC_ShuffleEnabledChangeRequested;

            _smtc.DisplayUpdater.Type = MediaPlaybackType.Music;
            _smtc.IsEnabled = true;
        }

        public void StartJsonStdIOMessagePump()
        {
            SwayFmMediaKeysState data;
            while ((data = ReadChromeNativeMessageFromSTDIO()) != null)
            {
                ProcessMessage(data);
            }
        }

        private SwayFmMediaKeysState ReadChromeNativeMessageFromSTDIO()
        {
            var stdin = Console.OpenStandardInput();
            var length = 0;

            var lengthBytes = new byte[4];
            stdin.Read(lengthBytes, 0, 4);
            length = BitConverter.ToInt32(lengthBytes, 0);

            var buffer = new char[length];
            using (var reader = new StreamReader(stdin))
            {
                while (reader.Peek() >= 0)
                {
                    reader.Read(buffer, 0, buffer.Length);
                }
            }

            return SwayFmMediaKeysState.FromJson(new string(buffer));
        }

        private void WriteChromeNativeMessageToSTDIO(JToken data)
        {
            var json = new JObject();
            json.Add(data);

            var bytes = System.Text.Encoding.UTF8.GetBytes(json.ToString(Formatting.None));

            var stdout = Console.OpenStandardOutput();
            stdout.WriteByte((byte)((bytes.Length >> 0) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 8) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 16) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 24) & 0xFF));
            stdout.Write(bytes, 0, bytes.Length);
            stdout.Flush();
        }

        public void ProcessMessage(SwayFmMediaKeysState data)
        {
            try
            {
                _smtc.DisplayUpdater.MusicProperties.Title = data.Title;
                _smtc.DisplayUpdater.MusicProperties.AlbumArtist = data.Artist;
                _smtc.DisplayUpdater.MusicProperties.Artist = data.Artist;
                //smtc.Status = (bool)playing["playing"]?.Value<bool>() ? "Playing" : "Stopped";

                Debug.WriteLine("[SMTC] Set Track Data");

                _smtc.IsPlayEnabled = data.Supports.Playpause;
                _smtc.IsPauseEnabled = data.Supports.Playpause;
                //_smtc.IsStopEnabled = data.Supports.;

                _smtc.IsPreviousEnabled = data.Supports.Previous;
                //_smtc.IsRewindEnabled = true;

                _smtc.IsNextEnabled = data.Supports.Next;
                //_smtc.IsFastForwardEnabled = true;

                _smtc.PlaybackRate = 1.0;

                //_smtc.IsChannelDownEnabled = true;
                //_smtc.IsChannelUpEnabled = true;

                //_smtc.IsRecordEnabled = true;

                //_smtc.ShuffleEnabled = true;

                //_smtc.PlaybackStatus = data.Playing; //TODO: Playing dont allways be an object, sometimes its a bool
            }
            catch (Exception)
            {
                Debug.WriteLine("[Debug] Failed to parse json or set smtc metadata.");
            }
            Debug.WriteLine("[SMTC] Set Player Capabilities.");

            try
            {
                if (!string.IsNullOrEmpty(data.AlbumArt))
                {
                    _smtc.DisplayUpdater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(data.AlbumArt.Replace("\"", "")));
                }
                else
                {
                    Debug.WriteLine("[Debug] AlbumArt String Empty.");
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("[Debug] Failed to create albumart from URI: " + data.AlbumArt);
            }

            Debug.WriteLine("[SMTC] Set all data from JSON: " + data.ToJson());

            _smtc.DisplayUpdater.Update();
        }

        #region WriteChromeNativeMessageToSTDIO for every event.

        private void _SMTC_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    //WriteChromeNativeMessageToSTDIO(new JProperty("Command", "Play"));
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", new JProperty("Command", "Play")));
                    break;

                case SystemMediaTransportControlsButton.Pause:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", "Pause"));
                    break;

                case SystemMediaTransportControlsButton.Stop:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", "Stop"));
                    break;

                case SystemMediaTransportControlsButton.Record:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", "Record"));
                    break;

                case SystemMediaTransportControlsButton.FastForward:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", "FastForward"));
                    break;

                case SystemMediaTransportControlsButton.Rewind:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", "Rewind"));
                    break;

                case SystemMediaTransportControlsButton.Next:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", "Next"));
                    break;

                case SystemMediaTransportControlsButton.Previous:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", "Previous"));
                    break;

                case SystemMediaTransportControlsButton.ChannelUp:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", "ChannelUp"));
                    break;

                case SystemMediaTransportControlsButton.ChannelDown:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", "ChannelDown"));
                    break;

                default:
                    break;
            }
        }

        private void _SMTC_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {
            switch (args.Property)
            {
                case SystemMediaTransportControlsProperty.SoundLevel:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", new JProperty("SystemMediaTransportControlsProperty", "SoundLevel")));
                    break;

                default:
                    break;
            }
        }

        private void _SMTC_AutoRepeatModeChangeRequested(SystemMediaTransportControls sender, AutoRepeatModeChangeRequestedEventArgs args)
        {
            switch (args.RequestedAutoRepeatMode)
            {
                case MediaPlaybackAutoRepeatMode.None:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", new JProperty("MediaPlaybackAutoRepeatMode", "None")));
                    break;

                case MediaPlaybackAutoRepeatMode.Track:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", new JProperty("MediaPlaybackAutoRepeatMode", "Track")));
                    break;

                case MediaPlaybackAutoRepeatMode.List:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", new JProperty("MediaPlaybackAutoRepeatMode", "List")));
                    break;

                default:
                    break;
            }
        }

        private void _SMTC_PlaybackRateChangeRequested(SystemMediaTransportControls sender, PlaybackRateChangeRequestedEventArgs args)
        {
            WriteChromeNativeMessageToSTDIO(new JProperty("Command", new JProperty("RequestedPlaybackRate", args.RequestedPlaybackRate)));
        }

        private void _SMTC_PlaybackPositionChangeRequested(SystemMediaTransportControls sender, PlaybackPositionChangeRequestedEventArgs args)
        {
            WriteChromeNativeMessageToSTDIO(new JProperty("Command", new JProperty("RequestedPlaybackPosition", args.RequestedPlaybackPosition)));
        }

        private void _SMTC_ShuffleEnabledChangeRequested(SystemMediaTransportControls sender, ShuffleEnabledChangeRequestedEventArgs args)
        {
            WriteChromeNativeMessageToSTDIO(new JProperty("Command", new JProperty("RequestedShuffleEnabled", args.RequestedShuffleEnabled)));
        }

        #endregion WriteChromeNativeMessageToSTDIO for every event.
    }
}