using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;
using System.IO;

namespace SMTC_API
{
    /// <summary>
    /// Construct this class to write example json and the schema for this api shim.
    /// </summary>
    public class SMTC_api_example
    {
        private static SystemMediaTransportControls smtc = new SystemMediaTransportControls()
        {
            AutoRepeatMode = MediaPlaybackAutoRepeatMode.None,
            DisplayUpdater = new SystemMediaTransportControlsDisplayUpdater()
            {
                ThumbnailURI = "https://images-na.ssl-images-amazon.com/images/I/51QFGpP5mOL._SY355_.jpg",
                AppMediaId = "AppMediaId",
                ImageProperties = new ImageDisplayProperties()
                {
                    Subtitle = "ImageSubtitle",
                    Title = "Imagetitle"
                },
                MusicProperties = new MusicDisplayProperties()
                {
                    AlbumArtist = "Music AlbumArtist",
                    AlbumTrackCount = 4,
                    AlbumTitle = "Music AlbumTitle",
                    Artist = "Music Artist",
                    Genres = new List<string>(new string[] { "Genre1", "Genre2", "Genre3" }),
                    Title = "Music Title",
                    TrackNumber = 3
                },
                Type = MediaPlaybackType.Music,
                VideoProperties = new VideoDisplayProperties()
                {
                    Genres = new List<string>(new string[] { "Genre1", "Genre2", "Genre3" }),
                    Subtitle = "Video Subtitle",
                    Title = "Video Title"
                }
            },
            IsChannelDownEnabled = false,
            IsChannelUpEnabled = false,
            IsEnabled = true,
            IsFastForwardEnabled = false,
            IsNextEnabled = true,
            IsPauseEnabled = true,
            IsPlayEnabled = true,
            IsPreviousEnabled = true,
            IsRecordEnabled = false,
            IsRewindEnabled = false,
            IsStopEnabled = true,
            PlaybackRate = 1.0,
            PlaybackStatus = MediaPlaybackStatus.Playing,
            ShuffleEnabled = false,
            SoundLevel = SoundLevel.Full,
        };

        public SMTC_api_example()
        {
            File.WriteAllText("SystemMediaTransportControls.json", JsonConvert.SerializeObject(smtc, Formatting.Indented));

            var jsonSchemaGenerator = new JsonSchemaGenerator();
            var myType = typeof(SystemMediaTransportControls);
            var schema = jsonSchemaGenerator.Generate(myType);
            var writer = new StringWriter();
            var jsonTextWriter = new JsonTextWriter(writer);
            schema.WriteTo(jsonTextWriter);
            dynamic parsedJson = JsonConvert.DeserializeObject(writer.ToString());
            var prettyString = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            var fileWriter = new StreamWriter("SystemMediaTransportControls.Schema.json");
            fileWriter.WriteLine(prettyString);
            fileWriter.Close();
        }
    }

    public class SystemMediaTransportControls
    {
        public bool IsPlayEnabled { get; set; }
        public bool IsPauseEnabled { get; set; }
        public bool IsNextEnabled { get; set; }
        public bool IsPreviousEnabled { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsChannelDownEnabled { get; set; }
        public bool IsFastForwardEnabled { get; set; }
        public bool IsChannelUpEnabled { get; set; }
        public MediaPlaybackStatus PlaybackStatus { get; set; }
        public bool IsStopEnabled { get; set; }
        public bool IsRewindEnabled { get; set; }
        public bool IsRecordEnabled { get; set; }
        public SystemMediaTransportControlsDisplayUpdater DisplayUpdater { get; set; }
        public SoundLevel SoundLevel { get; set; }
        public bool ShuffleEnabled { get; set; }
        public double PlaybackRate { get; set; }
        public MediaPlaybackAutoRepeatMode AutoRepeatMode { get; set; }
    }

    public enum MediaPlaybackStatus
    {
        Closed = 0,
        Changing = 1,
        Stopped = 2,
        Playing = 3,
        Paused = 4
    }

    public enum SoundLevel
    {
        Muted = 0,
        Low = 1,
        Full = 2
    }

    public enum MediaPlaybackAutoRepeatMode
    {
        None = 0,
        Track = 1,
        List = 2
    }

    public class SystemMediaTransportControlsDisplayUpdater
    {
        public MediaPlaybackType Type { get; set; }
        public string AppMediaId { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public ImageDisplayProperties ImageProperties { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public MusicDisplayProperties MusicProperties { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public VideoDisplayProperties VideoProperties { get; set; }

        public string ThumbnailURI { get; set; }
    }

    public enum MediaPlaybackType
    {
        Unknown = 0,
        Music = 1,
        Video = 2,
        Image = 3
    }

    public class ImageDisplayProperties
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
    }

    public class MusicDisplayProperties
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string AlbumArtist { get; set; }
        public uint TrackNumber { get; set; }
        public string AlbumTitle { get; set; }
        public IList<string> Genres { get; set; }
        public uint AlbumTrackCount { get; set; }
    }

    public class VideoDisplayProperties
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public IList<string> Genres { get; set; }
    }
}