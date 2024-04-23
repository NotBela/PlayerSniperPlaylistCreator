using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
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

        public static HttpResponseMessage getResponse(string url)
        {
            var response = client.GetAsync(url).Result;

            if ((int) response.StatusCode / 100 != 2) 
                throw new Exception("Got status code: " + response.StatusCode + ", from request: " + url);

            return response;
        }

        public static byte[] downloadData(string url) 
        {
            var response = client.GetByteArrayAsync(url).Result;

            return response;
        }
    }
}
