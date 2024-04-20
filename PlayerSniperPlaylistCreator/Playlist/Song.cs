using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator.Playlist
{
    internal class Song
    {
        public string hash;
        public List<Difficulty> difficulties;

        public Song(string hash, Difficulty difficulty)
        {
            this.hash = hash;
            difficulties = new List<Difficulty>();
            difficulties.Add(difficulty);
        }
    }
}
