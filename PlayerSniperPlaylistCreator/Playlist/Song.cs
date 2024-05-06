using System.Collections.Generic;

namespace PlayerSniperPlaylistCreator.Playlist
{
    public class Song
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
