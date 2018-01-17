// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var data = SwayFmMediaKeysState.FromJson(jsonString);

namespace QuickType
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using J = Newtonsoft.Json.JsonPropertyAttribute;

    public partial class SwayFmMediaKeysState
    {
        [J("title")] public string Title { get; set; }
        [J("artist")] public string Artist { get; set; }
        [J("albumArt")] public string AlbumArt { get; set; }
        [J("playing")] public Playing Playing { get; set; }
        [J("service")] public string Service { get; set; }
        [J("dontScrobble")] public bool DontScrobble { get; set; }
        [J("supports")] public Supports Supports { get; set; }
        [J("action")] public string Action { get; set; }
    }

    public partial class Playing
    {
    }

    public partial class Supports
    {
        [J("playpause")] public bool Playpause { get; set; }
        [J("next")] public bool Next { get; set; }
        [J("previous")] public bool Previous { get; set; }
    }

    public partial class SwayFmMediaKeysState
    {
        public static SwayFmMediaKeysState FromJson(string json) => JsonConvert.DeserializeObject<SwayFmMediaKeysState>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SwayFmMediaKeysState self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
