using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PlayerSniperPlaylistCreator.PlayerList
{
    internal static class PlayerWriter
    {
        public static readonly string path = $"{UnityGame.InstallPath}\\UserData\\PlayerSniperPlaylistCreator\\";

        public static void writeToJson(Player player)
        {
            if (player.id == null) throw new Exception("Player id is null, cannot write with no id");
            if (!Directory.Exists(path)) throw new Exception("Userdata folder does not exist!");

            File.WriteAllText($"{path}{player.id}.json", JsonConvert.SerializeObject(player));
        }

        public static Player readFromJson(string id)
        {
            if (!Directory.Exists(path) || !File.Exists($"{path}{id}")) throw new Exception("Player file or folder does not exist");

            return JsonConvert.DeserializeObject<Player>(File.ReadAllText($"{path}{id}.json"));
        }

        public static List<Player> getAllPlayers()
        {
            string[] fileListWithPath = Directory.GetFiles(path);

            var list = new List<Player>();

            foreach (string file in fileListWithPath)
            {
                list.Add(readFromJson(Path.GetFileNameWithoutExtension(file)));
            }

            return list;
        }

    }
}
