using Newtonsoft.Json.Linq;
using PlayerSniperPlaylistCreator.Playlist;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator
{
    public static class ScoresaberApiHelper
    {
        private static HttpClient client;

        static ScoresaberApiHelper()
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri("https://scoresaber.com");
        }

        public static async Task<HttpResponseMessage> getResponse(string url)
        {
            var response = await client.GetAsync(url);

            if ((int) response.StatusCode / 100 != 2) 
                throw new Exception("Got status code: " + response.StatusCode + ", from request: " + url);

            return response;
        }

        public static async Task<byte[]> downloadData(string url) 
        {
            var response = await client.GetByteArrayAsync(url);

            return response;
        }

        public static string getResponseData(HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result;
        }

        public static async Task<JObject> getScoresaberPlayerAsync(long id)
        {
            JObject player = JObject.Parse(getResponseData(await ScoresaberApiHelper.getResponse($"/api/player/{id}/full")));

            return player;
        }

        public static async Task<Image> getScoresaberPfpAsync(long id)
        {
            var player = await getScoresaberPlayerAsync(id);

            string pfpUrl = player.GetValue("profilePicture").ToString();

            return new Image(await downloadData(pfpUrl));
        }
    }
}
