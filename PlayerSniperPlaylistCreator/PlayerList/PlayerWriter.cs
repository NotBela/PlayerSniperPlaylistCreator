using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PlayerSniperPlaylistCreator.PlayerList
{
    internal static class PlayerWriter
    {
        private static readonly string path = Path.Combine(UnityGame.InstallPath, "\\UserData\\PlayerSniperPlaylistCreator\\");

        public static void writeToJson(Player player)
        {
            if (player.id == null) throw new Exception("Player id is null, cannot write with no id");

            File.WriteAllText($"{path}{player.id}.json", JsonConvert.SerializeObject(player));
        }

        public static Player readFromJson(string id)
        {
            if (!File.Exists($"{path}{id}.json")) throw new Exception("Player file does not exist");

            return JsonConvert.DeserializeObject<Player>($"{path}{id}.json");
        }

        private static void updateName(Player player, string newName)
        {
            var newPlayer = player;

            newPlayer.name = newName;
            writeToJson(newPlayer);
        }

        public static List<Player> getAllPlayers()
        {
            var list = new List<Player>();

            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                try
                {
                    Player curr = readFromJson(file.Substring(0, file.Length - 5));
                    list.Add(curr);
                }
                catch
                {
                    Plugin.Log.Warn($"Could not read file {file}!");
                }
            }

            return list;
        }

    }
}
