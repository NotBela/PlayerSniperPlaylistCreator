using IPA.Utilities;
using Newtonsoft.Json.Linq;
using PlayerSniperPlaylistCreator.Playlist;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator.Utils
{
    public static class Utils
    {
        public static readonly string path = UnityGame.InstallPath;

        public static string getResponseData(HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result;
        }

        public static void writePlaylistToFile(Playlist.Playlist playlist)
        {
            File.WriteAllText($"{path}\\Playlists\\{playlist.playlistTitle}.bplist", playlist.toJson());
        }

        public static async Task<JObject> getScoresaberPlayerAsync(long id)
        {
            JObject player = JObject.Parse(getResponseData(await ApiHelper.getResponse($"/api/player/{id}/full")));

            return player;
        }

        public static async Task<Image> getScoresaberPfpAsync(long id)
        {
            var player = await getScoresaberPlayerAsync(id);

            string pfpUrl = player.GetValue("profilePicture").ToString();

            return new Image(await ApiHelper.downloadData(pfpUrl));
        }


    }
}
