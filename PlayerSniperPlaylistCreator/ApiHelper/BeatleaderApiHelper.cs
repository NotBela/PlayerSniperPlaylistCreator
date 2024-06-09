using Newtonsoft.Json.Linq;
using PlayerSniperPlaylistCreator.Playlist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator
{
    internal static class BeatleaderApiHelper
    {
        private static HttpClient _httpClient;

        static BeatleaderApiHelper()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.beatleader.xyz");
        }

        public static async Task<HttpResponseMessage> getResponse(string url)
        {
            var response = await _httpClient.GetAsync(url);

            if ((int)response.StatusCode / 100 != 2)
                throw new Exception("Got status code: " + response.StatusCode + ", from request: " + url);
            return response;
        }

        public static async Task<byte[]> downloadData(string url)
        {
            var response = await _httpClient.GetByteArrayAsync(url);
            return response;
        }

        public static string getResponseData(HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result;
        }

        public static async Task<JObject> getBeatLeaderPlayerAsync(long id)
        {
            JObject player = JObject.Parse(getResponseData(await getResponse($"/player/{id}")));

            return player;
        }

        public static async Task<Image> getBeatleaderPfpAsync(long id)
        {
            var player = await getBeatLeaderPlayerAsync(id);

            string pfpUrl = player.GetValue("avatar").ToString();

            return new Image(await downloadData(pfpUrl));
        }


    }
}
