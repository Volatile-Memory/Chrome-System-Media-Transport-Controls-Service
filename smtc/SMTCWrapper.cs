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
            string data;
            while ((data = ReadChromeNativeMessageFromSTDIO()) != null)
            {
                ProcessMessage(data);
            }
        }

        private string ReadChromeNativeMessageFromSTDIO()
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

            return new string(buffer);
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

        public void ProcessMessage(string jsonMsg)
        {
            Debug.WriteLine("[SMTC] Got JSON: " + jsonMsg);
            SMTC_API.SystemMediaTransportControls data = JsonConvert.DeserializeObject<SMTC_API.SystemMediaTransportControls>(jsonMsg, Converter.Settings);

            try
            {
                _smtc.DisplayUpdater.MusicProperties.Title = data.DisplayUpdater.MusicProperties.Title;
                _smtc.DisplayUpdater.MusicProperties.AlbumArtist = data.DisplayUpdater.MusicProperties.AlbumArtist;
                _smtc.DisplayUpdater.MusicProperties.Artist = data.DisplayUpdater.MusicProperties.Artist;
                _smtc.PlaybackStatus = (MediaPlaybackStatus)data.PlaybackStatus;

                Debug.WriteLine("[SMTC] Set Track Data");

                _smtc.IsPlayEnabled = data.IsPlayEnabled;
                _smtc.IsPauseEnabled = data.IsPauseEnabled;
                _smtc.IsStopEnabled = data.IsStopEnabled;

                _smtc.IsPreviousEnabled = data.IsPreviousEnabled;
                _smtc.IsRewindEnabled = data.IsRewindEnabled;

                _smtc.IsNextEnabled = data.IsNextEnabled;
                _smtc.IsFastForwardEnabled = data.IsFastForwardEnabled;

                _smtc.PlaybackRate = data.PlaybackRate;

                _smtc.IsChannelDownEnabled = data.IsChannelDownEnabled;
                _smtc.IsChannelUpEnabled = data.IsChannelUpEnabled;

                _smtc.IsRecordEnabled = data.IsRecordEnabled;

                _smtc.ShuffleEnabled = data.ShuffleEnabled;
                Debug.WriteLine("[SMTC] Set Player Capabilities.");
            }
            catch (Exception)
            {
                Debug.WriteLine("[Debug] Failed to parse json or set smtc metadata.");
            }

            try
            {
                if (!string.IsNullOrEmpty(data.DisplayUpdater.ThumbnailURI))
                {
                    _smtc.DisplayUpdater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(data.DisplayUpdater.ThumbnailURI.Replace("\"", "")));
                    Debug.WriteLine("[Debug] Set AlbumArt.");
                }
                else
                {
                    Debug.WriteLine("[Debug] AlbumArt String Empty.");
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("[Debug] Failed to create albumart from URI: " + data.DisplayUpdater.ThumbnailURI);
            }

            _smtc.DisplayUpdater.Update();
        }

        #region WriteChromeNativeMessageToSTDIO for every event.

        private void _SMTC_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    WriteChromeNativeMessageToSTDIO(new JProperty("Command", "Play"));
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