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
using BeatSaberMarkupLanguage.Tags.Settings;
using BeatSaberMarkupLanguage.Components.Settings;
using PlayerSniperPlaylistCreator.Playlist;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PlayerSniperPlaylistCreator.ViewControllers
{
    [ViewDefinition("PlayerSniperPlaylistCreator.ViewControllers.GameplaySetupViewController.bsml")]
    public class GameplaySetupViewController
    {
        [UIComponent("players")]
        private DropDownListSetting playersDropdown = new DropDownListSetting();

        [UIValue("playerList")]
        private List<object> playerList = new List<object>(sanatizePlayerList());

        [UIValue("selectedPlayer")]
        private object selectedPlayer
        {
            get 
            { 
                if (PluginConfig.Instance.selectedPlayerId == null) return playerList[0];
                return PlayerWriter.readFromJson(PluginConfig.Instance.selectedPlayerId).id;
            }
            set 
            { 
                Player current = value as Player;

                PluginConfig.Instance.selectedPlayerId = current.id;
            }
        }

        [UIAction("testButtonOnClick")]
        private async void testButtonOnClick()
        {
            var hi = await PlaylistCreator.createPlaylist(76561199275799770, 76561199003743737, "hi");

            Plugin.Log.Info(hi);
        }

        private void updatePlayerList()
        {
            playersDropdown.values = new List<object>(sanatizePlayerList());
            playersDropdown.UpdateChoices();
        }

        // needs to be static or the program bitches about NOTHING !!!!!
        private static List<object> sanatizePlayerList()
        {
            var allPlayers = PlayerWriter.getAllPlayers();

            if (PlayerWriter.getAllPlayers().Count == 0) return new List<object> {"No players added!"};

            // cant convert a list of players to a list of objects implicitly apparently so this needs to be here :(

            List<object> returnList = new List<object>();

            foreach(var player in allPlayers)
            {
                returnList.Add(player);
            }

            return returnList;
        }

    }
}
