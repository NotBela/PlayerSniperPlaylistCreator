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
using TMPro;
using UnityEngine.UI;

namespace PlayerSniperPlaylistCreator.ViewControllers
{
    [ViewDefinition("PlayerSniperPlaylistCreator.ViewControllers.GameplaySetupViewController.bsml")]
    public class GameplaySetupViewController
    {

        private int positionInArr = 0;

        private JArray playerArr;

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

        [UIComponent("scoresaberPfp")]
        private UnityEngine.UI.Image scoresaberPfp;

        [UIComponent("resultsAmtText")]
        private TextMeshProUGUI resultsAmtText;

        [UIComponent("rankText")]
        private TextMeshProUGUI rankText;

        [UIComponent("nameText")]
        private TextMeshProUGUI nameText;

        [UIComponent("idText")]
        private TextMeshProUGUI idText;

        [UIAction("addPlayerButtonClick")]
        private void addPlayerButtonClick()
        {
            hideAllModals("keyboardShow");
        }

        [UIAction("keyboardOnEnter")]
        private async void keyboardOnEnter(string input)
        {
            try
            {
                hideAllModals("loadingModalShow");

                playerArr = (JArray)JObject.Parse(Utils.Utils.getResponseData(await ApiHelper.getResponse($"/api/players?search={input}")))["players"];

                resultsAmtText.text = $"Showing result {positionInArr + 1} out of {playerArr.Count}";
                nameText.text = $"{playerArr[positionInArr]["name"]}";
                rankText.text = $"#{playerArr[positionInArr]["rank"]}";
                scoresaberPfp.SetImage($"{playerArr[positionInArr]["profilePicture"]}");

                hideAllModals("playerListModalShow");
            }
            catch (Exception e)
            {
                showError(e);
            }
            
        }

        [UIAction("addPlayerModalAddButtonClick")]
        private void addPlayerModalAddButtonClick()
        {
            // add try catch here
            try
            {
                Player playerToAdd = new Player(playerArr[positionInArr]["id"].ToString(), playerArr[positionInArr]["name"].ToString());
                PlayerWriter.writeToJson(playerToAdd);

                hideAllModals("successModalShow");

                updatePlayerList();
            }
            catch(Exception e)
            {
                showError(e);
            }
            
        }

        #endregion


        #region modals
        [UIAction("createButtonOnClick")]
        private async void createButtonOnClick()
        {
            try
            {
                hideAllModals("loadingModalShow");

                long sniperID = 76561199003743737;
                long targetID = 76561199367121661;

                var sniperData = await Utils.Utils.getScoresaberPlayerAsync(sniperID);
                var targetData = await Utils.Utils.getScoresaberPlayerAsync(targetID);

                string targetName = targetData.GetValue("name").ToString();
                Playlist.Image targetPfp = await Utils.Utils.getScoresaberPfpAsync(targetID);
                var playlist = await PlaylistCreator.createPlaylist(sniperID, targetID, $"{targetName} Snipe Playlist", targetPfp);

                Utils.Utils.writePlaylistToFile(playlist);
                Loader.Instance.RefreshSongs();

                hideAllModals("successModalShow");
            }
            catch (Exception e)
            {
                showError(e);
            }
            
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
            parserParams.EmitEvent("failModalHide");

            if (modalToShow != null) parserParams.EmitEvent(modalToShow);
        }
        #endregion modals

        private void showError(Exception ex)
        {
            hideAllModals("failModalShow");
            Plugin.Log.Error($"An error occured: {ex}");
        }

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
