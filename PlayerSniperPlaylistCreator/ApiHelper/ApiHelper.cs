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
    public class ApiHelper
    {
        private static ApiHelper instance;
        private RestClient restClient;

        public ApiHelper(string url)
        {
            restClient = new RestClient(url);
        }

        public static ApiHelper getInstance()
        {
            if (instance == null)
                instance = new ApiHelper("https://scoresaber.com");
           
            return instance;
        }


        public RestResponse getResponse(string url)
        {
            var request = new RestRequest(url);
            var response = restClient.ExecuteGet(request);

            if ((int) response.StatusCode / 100 != 2) 
                throw new Exception("Got status code: " + response.StatusCode + ", from request: " + url);

            return response;
        }
    }
}
