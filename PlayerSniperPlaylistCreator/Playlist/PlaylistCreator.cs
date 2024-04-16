using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator.Playlist
{
    internal class PlaylistCreator
    {
        private RestClient client = new RestClient("https://scoresaber.com/");
        /*
         * Required Parameters:
         * - sinperID: the number part of the sniper's score saberlink
         * - targetID: the number part of the targets's score saberlink
         * - name: the name for the playlist (and also set this as the playlist file name)
         * 
         * Optional Parameters:
         * - includeUnplayed (default: true): if true the playlist will include maps that the target has a score on but not the sniper
         * - rankedOnly (default: false): if true only ranked maps will be included in the playlist
         * - order (default: "targetPP"): has 3 presets as strings listed here:
         *      - "targetPP": the target's higher pp plays will be put first
         *      - "sniperPP": the sniper's higher pp plays will be put first
         *      - "easiest": the closer the 2 scores are together the higher in the playlist order they will be
         */
        public string createPlaylist(int sniperID, int targetID, string name, bool includeUnplayed = false, bool rankedOnly = true, string order = "targetPP")
        {
            return "";
        }

        //returns a list of map objects for the given parameters
        //DONE but NOT tested
        private List<Map> getMaps(int id, bool rankedOnly)
        {
            List<Map> maps = new List<Map>();
            RestResponse response1 = getResponse("/api/player/" + id + "/full");
            JsonNode data1 = JsonSerializer.Deserialize<JsonNode>(response1.Content);
            int total = (int)data1["scoreStats"]["rankedPlayCount"];
            int maxPage = ((total - 1) / 100) + 2;
            for (int i = 1; i < maxPage; i++)
            {
                int limit = 0;
                if (i < maxPage - 1)
                {
                    limit = 100;
                }
                else
                {
                    limit = total - ((maxPage - 2) * 100);
                }
                RestResponse response2 = getResponse("/api/player/" + id + "/scores?limit=" + limit + "&sort=top&page=" + i);
                JsonArray data2 = JsonSerializer.Deserialize<JsonArray>(response2.Content);
                foreach (JsonNode x in data2)
                {
                    maps.Add(new Map((double)x["score"]["pp"], (double)x["score"]["baseScore"] / (double)x["leaderboard"]["maxScore"], (string)x["leaderboard"]["songHash"], new Difficulty("Standard", (int)x["leaderboard"]["difficulty"]["difficulty"])));
                }
            }
            return maps;
        }

        private RestResponse getResponse(string url)
        {
            RestRequest request = new RestRequest(url);
            RestResponse response = client.ExecuteGet(request);
            if (((int)response.StatusCode) / 100 != 2)
            {
                throw new Exception("Got status code: " + response.StatusCode + ", from request: " + url);
            }
            else
            {
                return response;
            }
        }
    }
}
