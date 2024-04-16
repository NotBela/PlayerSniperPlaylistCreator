using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using PlayerSniperPlaylistCreator.Configuration;
using PlayerSniperPlaylistCreator.PlayerList;
using Newtonsoft.Json;

namespace PlayerSniperPlaylistCreator.ViewControllers
{
    [ViewDefinition("PlayerSniperPlaylistCreator.ViewControllers.GameplaySetupViewController.bsml")]
    public class GameplaySetupViewController
    {
        [UIValue("playerList")]
        private List<object> playerList = new List<object>() { new Player("9873297438974", "hi"), new Player("7348979835768947", "hey") };  // (sanatizePlayerList());

        [UIValue("selectedPlayer")]
        private object selectedPlayer
        {
            get { return PluginConfig.Instance.selectedPlayer; }
            set { PluginConfig.Instance.selectedPlayer = value as string; }
        }

        // needs to be static or the program bitches about NOTHING !!!!!
        private static List<object> sanatizePlayerList()
        {
            var allPlayers = PlayerWriter.getAllPlayers();

            if (PlayerWriter.getAllPlayers().Count == 0) return new List<object> {"No players added!"};

            // cant convert a list of players to a list of objects apparently so this needs to be here :(

            List<object> returnList = new List<object>();

            foreach(var player in allPlayers)
            {
                returnList.Add(player);
            }

            return returnList;
        }

    }
}
