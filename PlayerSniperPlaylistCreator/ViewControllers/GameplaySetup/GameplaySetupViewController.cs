using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;

namespace PlayerSniperPlaylistCreator.ViewControllers.GameplaySetup
{
    [ViewDefinition("PlayerSniperPlaylistCreator.ViewControllers.GameplaySetup.GameplaySetupViewController.bsml")]
    public class GameplaySetupViewController
    {
        // WHY ISNT THIS WORKING :(
        [UIValue("playerList")]
        private List<object> playerList = new List<object>() { "hey guys", "hi" };

        

    }
}
