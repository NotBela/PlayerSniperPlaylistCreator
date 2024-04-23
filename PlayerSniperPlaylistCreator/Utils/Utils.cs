using IPA.Utilities;
using PlayerSniperPlaylistCreator.Playlist;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator.Utils
{
    public static class Utils
    {
        public static readonly string userId;
        public static readonly string path = UnityGame.InstallPath;

        static Utils()
        {
            userId = GetUserInfo().Id.ToString();
        }

        private static async Task<UserInfo> GetUserInfo()
        {
            var userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();

            return userInfo;
        }

        public static string getResponseData(HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result;
        }

        public static void writePlaylistToFile(Playlist.Playlist playlist)
        {
            File.WriteAllText($"{path}\\Playlists\\{playlist.playlistTitle}.bplist", playlist.toJson());
        }
    }
}
