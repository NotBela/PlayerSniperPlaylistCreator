using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator.Playlist
{
    public class Playlist
    {
        public string playlistTitle { get; private set; }
        public string playlistAuthor { get; private set; } = null;
        public string playlistDiscription { get; private set; } = null;
        public List<Song> songs { get; private set; }
        public string image { get; private set; }

        [JsonConstructor]
        public Playlist(string playlistTitle, List<Song> songs, string image) {
            this.playlistTitle = playlistTitle;
            this.songs = songs;
            this.image = image;
        }

        public string toJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
