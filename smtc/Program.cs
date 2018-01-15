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
                Write(processed);
                if (processed == "exit")
                {
                    return;
                }
            }

            //Thread.Sleep(Timeout.Infinite);
        }

        public static string ProcessMessage(JObject data)
        {
            var title			= data["title"]			.Value<string>();
            var artist			= data["artist"]		.Value<string>();
            var albumArt		= data["albumArt"]		.Value<string>();
            var playing			= data["playing"]		.Value<bool>();
            var service			= data["service"]		.Value<string>();
            var thumbsUp		= data["thumbsUp"]		.Value<bool>();
            var thumbsDown		= data["thumbsDown"]	.Value<bool>();
            var dontScrobble	= data["dontScrobble"]	.Value<bool>();
            //var supports		= data["supports"]		.Value<string>();
            var action			= data["action"]		.Value<string>();

            smtc.Title = title;
            smtc.AlbumArtist = artist;
            smtc.Artist = artist;
            smtc.Status = playing ? "Playing" : "Stopped";
            smtc.AlbumArt = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri(albumArt));

            return title;
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
            //json["data"] = data;

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
            var AlbumArt = RandomAccessStreamReference.CreateFromFile(
                await StorageFile.GetFileFromPathAsync(@"C:\Users\CMPTRNDKP\Desktop\note.jpg")
                );
            smtc.AlbumArt = AlbumArt;
        }
    }
}
