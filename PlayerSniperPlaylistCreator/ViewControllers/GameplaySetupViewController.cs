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

namespace PlayerSniperPlaylistCreator.ViewControllers
{
    [ViewDefinition("PlayerSniperPlaylistCreator.ViewControllers.GameplaySetupViewController.bsml")]
    public class GameplaySetupViewController
    {
        [UIValue("playerList")]
        private List<object> playerList = new List<object>() { "hey guys", "hi" };

        [UIValue("selectedPlayer")]
        private object selectedPlayer
        {
            get { return PluginConfig.Instance.selectedPlayer; }
            set { PluginConfig.Instance.selectedPlayer = value as string; }
        }

    }
}
