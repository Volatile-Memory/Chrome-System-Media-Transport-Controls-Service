using System;
using System.Threading;
using System.Threading.Tasks;
//using System.Windows.Forms;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace smtc
{
    class Program
    {
        static SMTCWrapper smtc = new SMTCWrapper()
        {
            //AlbumArt    = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Music/music1_AlbumArt.jpg")),
            Artist = "Artist1",
            AlbumArtist = "AlbumArtist1",
            Title = "Title1",
        };

        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                await MainAsync(args);
            }).GetAwaiter().GetResult();

            JObject data;
            while ((data = Read()) != null)
            {
                var processed = ProcessMessage(data);
            }
        }

        public static string ProcessMessage(JObject data)
        {
            try
            {
                var title = data["title"].Value<string>();
                var artist = data["artist"].Value<string>();
                var albumArt = data["albumArt"].Value<string>();
                var playing = data["playing"]?.Value<JObject>();
                var service = data["service"].Value<string>();


                var supports = data["supports"].Value<JObject>();


                var supportsPlayPause = supports["playpause"]?.Value<bool>();
                var supportsNext = supports["next"]?.Value<bool>();
                var supportsPrevious = supports["previous"]?.Value<bool>();
                var supportsFavorite = supports["favorite"]?.Value<bool>();

                var thumbsUp = data["thumbsUp"]?.Value<bool>();
                var thumbsDown = data["thumbsDown"]?.Value<bool>();


                var dontScrobble = data["dontScrobble"]?.Value<bool>();
                var action = data["action"].Value<string>();

                smtc.Title = title;
                smtc.AlbumArtist = artist;
                smtc.Artist = artist;
                //smtc.Status = (bool)playing["playing"]?.Value<bool>() ? "Playing" : "Stopped";

                try
                {
                    smtc.AlbumArt = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri(albumArt.Replace("\"", "")));
                }
                catch (Exception)
                {
                    Debug.WriteLine("[Debug] Failed to create albumart.");
                }

            }
            catch (Exception)
            {
                Debug.WriteLine("[Debug] Failed to parse json or set smtc metadata.");
            }

            Debug.WriteLine(data.ToString());

            return "Message Parsed.";
        }

        public static JObject Read()
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

            return (JObject)JsonConvert.DeserializeObject<JObject>(new string(buffer)); 
        }

        public static void Write(JToken data)
        {
            var json = new JObject();
            //Debugger.Launch();
            json.Add("request", data);

            var bytes = System.Text.Encoding.UTF8.GetBytes(json.ToString(Formatting.None));

            var stdout = Console.OpenStandardOutput();
            stdout.WriteByte((byte)((bytes.Length >> 0) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 8) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 16) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 24) & 0xFF));
            stdout.Write(bytes, 0, bytes.Length);
            stdout.Flush();
        }

        static async Task MainAsync(string[] args)
        {
        }
    }
}
