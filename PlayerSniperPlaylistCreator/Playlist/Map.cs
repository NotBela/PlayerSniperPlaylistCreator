using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator.Playlist
{
    internal class Map : Song
    {
        public double pp;
        public double acc;

        public Map(double pp, double acc, string hash, Difficulty difficulty) : base(hash, difficulty) {
            this.pp = pp;
            this.acc = acc;
        }

        //returns the song portion of the map object
        public Song getSong() {
            return new Song(hash, difficulties[0]);
        }
    }
}
