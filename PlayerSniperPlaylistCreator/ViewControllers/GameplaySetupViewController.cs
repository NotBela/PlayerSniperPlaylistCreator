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
using IPA.Utilities;
using UnityEngine;

namespace PlayerSniperPlaylistCreator.ViewControllers
{
    [ViewDefinition("PlayerSniperPlaylistCreator.ViewControllers.GameplaySetupViewController.bsml")]
    public class GameplaySetupViewController
    {
        #region Variables
        private int _positionInArr = 0;
        internal int positionInArr 
        {
            get { return  _positionInArr; }
            set {
                _positionInArr = value;
                nameText.text = playerArr[value]["name"].ToString();
                rankText.text = $"#{playerArr[value]["rank"]}";
                scoresaberPfp.SetImage(playerArr[value]["profilePicture"].ToString());

                leftPageButtonActive = value > 0;
                rightPageButtonActive = value < playerArr.Count;
            }
        }

        private JArray playerArr;

        [UIParams]
        private BSMLParserParams parserParams = null;

        #region BSMLComponent

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

        #region ModalComponents

        [UIValue("leftPageButtonActive")]
        private bool leftPageButtonActive { get; set; } = false;

        [UIValue("rightPageButtonActive")]
        private bool rightPageButtonActive { get; set; } = false;

        [UIComponent("resultModalText")]
        private TextMeshProUGUI resultModalText;

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
                switch (PluginConfig.Instance.playlistOrder)
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
                switch (value)
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


        #endregion

        #endregion ModalComponents

        #endregion BSMLComponent

        #endregion Variables

        #region BSMLActions

        #region Root

        [UIAction("settingsCloseButtonClicked")]
        private void settingsCloseButtonClicked()
        {
            hideAllModals();
        }

        [UIAction("createButtonOnClick")]
        private async void createButtonOnClick()
        {
            try
            {
                hideAllModals("loadingModalShow");
                
                var info = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();

                long sniperID = long.Parse(info.platformUserId);
                long targetID = 76561199367121661;

                var sniperData = await Utils.Utils.getScoresaberPlayerAsync(sniperID);
                var targetData = await Utils.Utils.getScoresaberPlayerAsync(targetID);

                string targetName = targetData.GetValue("name").ToString();
                Playlist.Image targetPfp = await Utils.Utils.getScoresaberPfpAsync(targetID);
                var playlist = await PlaylistCreator.createPlaylist(
                    sniperID, 
                    targetID, 
                    $"{targetName} Snipe Playlist", 
                    targetPfp, 
                    PluginConfig.Instance.includeUnplayed, 
                    PluginConfig.Instance.rankedOnly, 
                    PluginConfig.Instance.playlistOrder
                );

                Utils.Utils.writePlaylistToFile(playlist);
                Loader.Instance.RefreshSongs();

                showResult("Successfully generated playlist!");
            }
            catch (Exception e)
            {
                showResult($"An error occured fetching \n data from scoresaber!", e);
            }

        }

        [UIAction("settingsButtonClick")]
        private void settingsButtonClick()
        {
            hideAllModals("settingsModalShow");
        }

        [UIAction("addPlayerButtonClick")]
        private void addPlayerButtonClick()
        {
            hideAllModals("keyboardShow");
        }

        #endregion

        #region AddPlayerSubModals

        [UIAction("leftPageButtonClick")]
        private void leftPageButtonClick()
        {
            positionInArr--;
        }

        [UIAction("rightPageButtonClick")]
        private void rightPageButtonClick()
        {
            positionInArr++;
        }

        [UIAction("addPlayerModalCancelButtonClick")]
        private void addPlayerModalCancelButtonClick()
        {
            hideAllModals();
        }

        [UIAction("keyboardOnEnter")]
        private async void keyboardOnEnter(string input)
        {
            try
            {
                hideAllModals("loadingModalShow");

                // add check here to make sure user enters more than 3 chars
                if (input.Length < 3)
                {
                    showResult("Search terms must be greater \n than 3 characters!");
                    return;
                }

                var response = await ApiHelper.getResponse($"/api/players?search={input}");

                if ((int) response.StatusCode / 100 == 4)
                {
                    showResult("No players found!");
                    return;
                }

                playerArr = (JArray)JObject.Parse(Utils.Utils.getResponseData(response))["players"];

                resultsAmtText.text = $"Showing result {positionInArr + 1} out of {playerArr.Count}";
                nameText.text = $"{playerArr[positionInArr]["name"]}";
                rankText.text = $"#{playerArr[positionInArr]["rank"]}";
                scoresaberPfp.SetImage($"{playerArr[positionInArr]["profilePicture"]}");

                hideAllModals("playerListModalShow");
            }
            catch (Exception e)
            {
                showResult("An error occured fetching \n data from scoresaber!", e);
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

                showResult("Successfully added player!");

                updatePlayerList();
            }
            catch (Exception e)
            {
                showResult($"An error occured writing player to disk!", e);
            }

        }

        #endregion

        // replace with general result modal
        [UIAction("resultOkButtonClick")]
        private void resultOkButtonClick()
        {
            hideAllModals();
        }

        #endregion

        #region Internal Methods
        private void hideAllModals(string modalToShow = null)
        {
            parserParams.EmitEvent("loadingModalHide");
            parserParams.EmitEvent("settingsModalHide");
            parserParams.EmitEvent("resultModalHide");
            parserParams.EmitEvent("keyboardHide");
            parserParams.EmitEvent("playerListModalHide");

            if (modalToShow != null) parserParams.EmitEvent(modalToShow);
        }

        private void showResult(string text, Exception ex = null)
        {
            resultModalText.text = text;

            hideAllModals("resultModalShow");
            if (ex != null) Plugin.Log.Error($"An error occured: {ex}");
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

        #endregion Internal Methods

    }
}
