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
using Loader = SongCore.Loader;
using BeatSaberMarkupLanguage.Parser;
using System.Runtime.CompilerServices;
using System.Security.Policy;

namespace PlayerSniperPlaylistCreator.ViewControllers
{
    [ViewDefinition("PlayerSniperPlaylistCreator.ViewControllers.GameplaySetupViewController.bsml")]
    public class GameplaySetupViewController
    {
        [UIParams]
        private BSMLParserParams parserParams = null;

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

        #region add player modals

        [UIAction("addPlayerButtonClick")]
        private void addPlayerButtonClick()
        {
            hideAllModals("keyboardShow");
        }

        [UIValue("keyboardText")]
        private string keyboardText { get; set; }

        [UIAction("keyboardOnEnter")]
        private async void keyboardOnEnter()
        {
            hideAllModals("loadingModalShow");

            JArray playerArr = (JArray) JObject.Parse(Utils.Utils.getResponseData(await ApiHelper.getResponse($"/api/players?search={keyboardText}")))["players"];

            int positionInArr = 0;

            hideAllModals("playerListModalShow");
        }

        #endregion


        #region modals
        [UIAction("createButtonOnClick")]
        private async void createButtonOnClick()
        {
            hideAllModals("loadingModalShow");

            long sniperID = 76561199003743737;
            long targetID = 76561199367121661;

            var sniperData = await Utils.Utils.getScoresaberPlayerAsync(sniperID);
            var targetData = await Utils.Utils.getScoresaberPlayerAsync(targetID);

            string targetName = targetData.GetValue("name").ToString();
            Image targetPfp = await Utils.Utils.getScoresaberPfpAsync(targetID);
            var playlist = await PlaylistCreator.createPlaylist(sniperID, targetID, $"{targetName} Snipe Playlist", targetPfp);
            
            Utils.Utils.writePlaylistToFile(playlist);
            Loader.Instance.RefreshSongs();

            hideAllModals("successModalShow");
        }
        
        [UIAction("successOkButtonClick")]
        private void successOkButtonClick()
        {
            hideAllModals();
        }

        #region Playlist Settings Modal

        [UIValue("includeUnplayedValue")]
        private bool includeUnplayedValue
        {
            get { return PluginConfig.Instance.includeUnplayed; }
            set { PluginConfig.Instance.includeUnplayed = value; }
        }

        [UIValue("rankedOnlyValue")]
        private bool rankedOnlyValue
        {
            get { return PluginConfig.Instance.rankedOnly; }
            set { PluginConfig.Instance.rankedOnly = value; }
        }

        [UIValue("playlistOrderList")]
        private List<object> playlistOrderList = new List<object>() { "Target Highest", "Your Highest", "Closest" };

        [UIValue("orderValue")]
        private object orderValue
        {
            get
            {
                switch(PluginConfig.Instance.playlistOrder)
                {
                    case "targetPP":
                        return "Target Highest";
                    case "sniperPP":
                        return "Your Highest";
                    default:
                        return "Closest";
                }
            }

            set
            {
                switch(value)
                {
                    case "Target Highest":
                        PluginConfig.Instance.playlistOrder = "targetPP";
                        break;
                    case "Your Highest":
                        PluginConfig.Instance.playlistOrder = "sniperPP";
                        break;
                    default:
                        PluginConfig.Instance.playlistOrder = "easiest";
                        break;
                }
            }
        }

        [UIAction("settingsCloseButtonClicked")]
        private void settingsCloseButtonClicked()
        {
            hideAllModals();
        }

        #endregion

        [UIAction("settingsButtonClick")]
        private void settingsButtonClick()
        {
            hideAllModals("settingsModalShow");
        }

        private void hideAllModals(string modalToShow = null)
        {
            parserParams.EmitEvent("loadingModalHide");
            parserParams.EmitEvent("settingsModalHide");
            parserParams.EmitEvent("successModalHide");
            parserParams.EmitEvent("keyboardHide");
            parserParams.EmitEvent("playerListModalHide");

            if (modalToShow != null) parserParams.EmitEvent(modalToShow);
        }

        #endregion modals

        private void updatePlayerList()
        {
            playersDropdown.values = new List<object>(sanatizePlayerList());
            playersDropdown.UpdateChoices();
        }

        // needs to be static or the program bitches about NOTHING !!!!!
        private static List<object> sanatizePlayerList()
        {
            try
            {
                var allPlayers = PlayerWriter.getAllPlayers();

                // cant convert a list of players to a list of objects implicitly apparently so this needs to be here :(

                List<object> returnList = new List<object>();

                foreach (object player in allPlayers)
                {
                    returnList.Add(player);
                }

                if (!Directory.Exists(Path.Combine(Utils.Utils.path, "\\PlayerSniperPlaylistCreator")) || returnList.Count == 0) return new List<object> { "No players added!" };

                return returnList;
            }
            catch (Exception)
            {
                // really dumb but idk a better way
                return new List<object> { "No players added!" };
            }
            
        }

    }
}
