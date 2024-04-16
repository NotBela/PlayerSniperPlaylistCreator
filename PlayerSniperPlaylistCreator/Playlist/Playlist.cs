using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator.Playlist
{
    internal class Playlist
    {
        public string playlistTitle;
        public string playlistAuthor = null;
        public string playlistDiscription = null;
        public List<Song> songs;
        public string image = null;

        public Playlist(string playlistTitle) {
            this.playlistTitle = playlistTitle;
        }
    }
}
