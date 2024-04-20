using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator.Playlist
{
    internal class Difficulty
    {
        public string characteristic;
        public string name;

        //if string is given then use that string
        public Difficulty(string characteristic, string name)
        {
            this.characteristic = characteristic;
            this.name = name;
        }

        //if int is given turn into the corresponding string
        public Difficulty(string characteristic, int id)
        {
            this.characteristic = characteristic;
            switch (id)
            {
                case 1:
                    name = "Easy";
                    break;
                case 3:
                    name = "Normal";
                    break;
                case 5:
                    name = "Hard";
                    break;
                case 7:
                    name = "Expert";
                    break;
                case 9:
                    name = "ExpertPlus";
                    break;
            }
        }

        public string toString()
        {
            return characteristic + " " + name;
        }
    }
}
