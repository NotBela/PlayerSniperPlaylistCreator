using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PlayerSniperPlaylistCreator.PlayerList
{
    internal static class PlayerWriter
    {
        public static readonly string path = Path.Combine(UnityGame.InstallPath, "UserData\\PlayerSniperPlaylistCreator\\");

        public static void writeToJson(Player player)
        {
            if (player.id == null) throw new Exception("Player id is null, cannot write with no id");
            if (!Directory.Exists(path)) throw new Exception("Userdata folder does not exist!");

            File.WriteAllText($"{path}{player.id}.json", JsonConvert.SerializeObject(player));
        }

        public static Player readFromJson(string id)
        {
            if (!Directory.Exists(path) || !File.Exists($"{path}{id}.json")) throw new Exception("Player file or folder does not exist");

            return JsonConvert.DeserializeObject<Player>($"{path}{id}.json");
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
