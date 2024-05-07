using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Settings;
using IPA;
using IPA.Config.Stores;
using PlayerSniperPlaylistCreator.Configuration;
using PlayerSniperPlaylistCreator.PlayerList;
using System;
using System.IO;
using IPALogger = IPA.Logging.Logger;

namespace PlayerSniperPlaylistCreator
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger, IPA.Config.Config conf)
        {
            Instance = this;
            Log = logger;
            PluginConfig.Instance = conf.Generated<PluginConfig>();
            // Log.Info("PlayerSniperPlaylistCreator initialized.");
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            BSMLSettings.instance.AddSettingsMenu("PSPC", "PlayerSniperPlaylistCreator.ViewControllers.SettingsViewController.bsml", Configuration.PluginConfig.Instance);
            // Log.Debug("OnApplicationStart");

            if (PluginConfig.Instance.enabled)
            {
                ViewControllers.GameplaySetupViewController controller = new ViewControllers.GameplaySetupViewController();

                GameplaySetup.instance.AddTab("PSPC", "PlayerSniperPlaylistCreator.ViewControllers.GameplaySetupViewController.bsml", controller, MenuType.Solo);
            }

        }

        [OnExit]
        public void OnApplicationQuit()
        {
            // Log.Debug("OnApplicationQuit");
            GameplaySetup.instance.RemoveTab("PSPC");

        }
    }
}
