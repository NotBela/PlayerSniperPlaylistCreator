using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS_Utils;
using BS_Utils.Gameplay;

namespace PlayerSniperPlaylistCreator.PlayerList
{
    internal class Player
    {

        public string name;
        public string id;

        public Player(string id, string name) // ADD DEFAULT HERE THAT GETS NAME AUTOMATICALLY
        {
            this.name = name;
            this.id = id;
        }

        public override string ToString()
        {
            return name;
        }

    }
}
