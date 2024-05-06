using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator
{
    public static class ApiHelper
    {
        private static HttpClient client;

        static ApiHelper()
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
    }
}
