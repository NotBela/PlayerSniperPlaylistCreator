using Microsoft.SqlServer.Server;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator
{
    public static class ApiHelper
    {
        private static RestClient restClient;

        static ApiHelper()
        {
            restClient = new RestClient("https://scoresaber.com");
        }

        public static RestResponse getResponse(string url)
        {
            var request = new RestRequest(url);
            var response = restClient.ExecuteGet(request);

            if ((int) response.StatusCode / 100 != 2) 
                throw new Exception("Got status code: " + response.StatusCode + ", from request: " + url);

            return response;
        }
    }
}
