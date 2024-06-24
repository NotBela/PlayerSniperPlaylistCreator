using IPA;
using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace PlayerSniperPlaylistCreator.Configuration
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual long selectedPlayerId { get; set; } = -1;
        public virtual string selectedPlayerName { get; set; } = "None";
        public virtual bool includeUnplayed { get; set; } = false;
        public virtual bool rankedOnly { get; set; } = true;
        public virtual string playlistOrder { get; set; } = "targetPP";
        public virtual bool scoresaberPrimary { get; set; } = true;
    }
}