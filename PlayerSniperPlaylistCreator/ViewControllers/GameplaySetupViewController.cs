using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage;
using System;
using System.Collections.Generic;
using PlayerSniperPlaylistCreator.Configuration;
using PlayerSniperPlaylistCreator.Playlist;
using Newtonsoft.Json.Linq;
using Loader = SongCore.Loader;
using BeatSaberMarkupLanguage.Parser;
using TMPro;
using BeatSaberMarkupLanguage.Components;
using UnityEngine.UI;
using IPA.Utilities;
using System.IO;
using Zenject;
using PlaylistManager;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Settings;

namespace PlayerSniperPlaylistCreator.ViewControllers
{
    [ViewDefinition("PlayerSniperPlaylistCreator.ViewControllers.GameplaySetupViewController.bsml")]
    public class GameplaySetupViewController : BSMLAutomaticViewController, IInitializable
    {

        [Inject] private readonly PlaylistDataManager _playlistDataManager;

        public void Initialize()
        {
            if (!PluginConfig.Instance.enabled)
            {
                GameplaySetup.instance.RemoveTab("PSPC");
            }
            else
            {
                GameplaySetup.instance.AddTab("PSPC", "PlayerSniperPlaylistCreator.ViewControllers.GameplaySetupViewController.bsml", this, MenuType.Solo);
                Plugin.Log.Info("PSPC view controller initialized");
            }
        }

        public GameplaySetupViewController() 
        {

            if (PluginConfig.Instance.selectedPlayerId == -1)
            {
                PluginConfig.Instance.selectedPlayerName = "None";
                createButtonInteractable = false;
            }
            else
            {
                createButtonInteractable = true;
            }
        }

        

        #region Variables

        private int _positionInArr = 0;
        internal int positionInArr
        {
            get { return _positionInArr; }
            set {
                _positionInArr = value;
                nameText.text = playerArr[value]["name"].ToString();
                rankText.text = $"#{playerArr[value]["rank"]}";
                scoresaberPfp.SetImage(playerArr[value]["profilePicture"].ToString());
                resultsAmtText.text = $"Showing result {positionInArr + 1} out of {playerArr.Count}";

                leftPageButton.gameObject.SetActive(value > 0);
                rightPageButton.gameObject.SetActive(value < playerArr.Count - 1);
            }
        }

        private JArray playerArr;

        [UIParams]
        private BSMLParserParams parserParams = null;

        #region BSMLComponent

        [UIValue("createButtonInteractable")]
        private bool createButtonInteractable = false;

        [UIComponent("createButton")]
        private Button createButton;

        [UIComponent("selectedPlayer")]
        private TextMeshProUGUI selectedPlayerText;

        [UIValue("selectedText")]
        private string selectedText = $"Selected Player: {PluginConfig.Instance.selectedPlayerName}";

        #region ModalComponents

        [UIComponent("leftPageButton")]
        private PageButton leftPageButton;

        [UIComponent("rightPageButton")]
        private PageButton rightPageButton;

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
                long targetID = PluginConfig.Instance.selectedPlayerId;

                var sniperData = await ApiHelper.getScoresaberPlayerAsync(sniperID);
                var targetData = await ApiHelper.getScoresaberPlayerAsync(targetID);

                string targetName = targetData.GetValue("name").ToString();
                Playlist.Image targetPfp = await ApiHelper.getScoresaberPfpAsync(targetID);
                var playlist = await PlaylistCreator.createPlaylist(
                    sniperID, 
                    targetID, 
                    $"{targetName} Snipe Playlist", 
                    targetPfp, 
                    PluginConfig.Instance.includeUnplayed, 
                    PluginConfig.Instance.rankedOnly, 
                    PluginConfig.Instance.playlistOrder
                );

                writePlaylistToFile(playlist);
                Loader.Instance.RefreshSongs();
                BeatSaberPlaylistsLib.PlaylistManager.DefaultManager.RefreshPlaylists(true);

                _playlistDataManager.selectedPlaylist = BeatSaberPlaylistsLib.PlaylistManager.DefaultManager.GetPlaylist($"{playlist.playlistTitle}.bplist");

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

                playerArr = (JArray)JObject.Parse(ApiHelper.getResponseData(response))["players"];

                resultsAmtText.text = $"Showing result {positionInArr + 1} out of {playerArr.Count}";
                nameText.text = $"{playerArr[positionInArr]["name"]}";
                rankText.text = $"#{playerArr[positionInArr]["rank"]}";
                scoresaberPfp.SetImage($"{playerArr[positionInArr]["profilePicture"]}");

                positionInArr = 0;

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
                JObject playerToAdd = (JObject) playerArr[positionInArr];
                PluginConfig.Instance.selectedPlayerId = long.Parse(playerToAdd["id"].ToString());
                PluginConfig.Instance.selectedPlayerName = playerToAdd["name"].ToString();

                selectedPlayerText.text = $"Selected Player: {playerToAdd["name"]}";

                createButton.interactable = true;

                showResult("Successfully selected player!");
            }
            catch (Exception e)
            {
                showResult($"An error occured selecting this player!", e);
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

        private static void writePlaylistToFile(Playlist.Playlist playlist)
        {
            File.WriteAllText($"{UnityGame.InstallPath}\\Playlists\\{playlist.playlistTitle}.bplist", playlist.toJson());
        }

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


        #endregion Internal Methods

    }
}
