using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator.PlayerList
{
    internal class Player
    {
        public string name;
        public string id;

        [JsonConstructor]
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
