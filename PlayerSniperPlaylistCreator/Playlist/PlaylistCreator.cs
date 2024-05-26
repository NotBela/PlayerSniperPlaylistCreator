﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator.Playlist
{
    internal class PlaylistCreator
    {
        private const string theFunnyMapHash = "ad6c9f88d63259a95e39397c31be2981c4beb744"; //currently set to: rctts
        private const string theFunnyDiff = "ExpertPlus";
        private const string theFunnyCharacteristic = "Standard";
        private const int theFunnyMapNumber = 100;
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
         * - image (default: null): the image for the playlist cover in base64
         */

        //DONE, TESTED, good to go
        public static async Task<Playlist> createPlaylist(long sniperID, long targetID, string name, Image image, bool includeUnplayed = false, bool rankedOnly = true, string order = "targetPP")
        {
            //THE FUNNY
            if (sniperID == targetID)
            {
                List<Song> theFunnies = new List<Song>();
                for (int i = 0; i < theFunnyMapNumber; i++)
                {
                    theFunnies.Add(new Song(theFunnyMapHash, new Difficulty(theFunnyCharacteristic, theFunnyDiff)));
                }
                string theFunnyImage = $"data:image/jpeg;base64,{image.convertToBase64()}";
                Playlist theFunnyList = new Playlist(name, theFunnies, theFunnyImage);
                return theFunnyList;
            }
            //the non funny
            List<Map> maps = new List<Map>();
            //get maps
            List<Map> sniperMaps = await getMaps(sniperID, rankedOnly);
            List<Map> targetMaps = await getMaps(targetID, rankedOnly);

            //keep all of the maps where map is the same and target acc > sniper acc and add them to dictionary
            foreach (Map map1 in targetMaps)
            {
                bool exist = false;
                foreach (Map map2 in sniperMaps)
                {
                    if ((map1.hash == map2.hash) && (map1.difficulties[0].toString() == map2.difficulties[0].toString()))
                    {
                        exist = true;
                        if (map1.acc > map2.acc)
                        {
                            //loop to check if map already exists in maps
                            bool exist2 = false;
                            foreach (Map x in maps)
                            {
                                if ((x.hash == map1.hash) && (x.difficulties[0].toString() == map1.difficulties[0].toString()))
                                    exist2 = true;
                            }
                            if (!exist2)
                            {
                                if (order == "easiest")
                                {
                                    //if order is easiest the acc value becomes the acc difference
                                    map1.acc = map1.acc - map2.acc;
                                    //insert into maps with lowest map1 acc first
                                    bool done = false;
                                    for (int i = 0; i < maps.Count; i++)
                                    {
                                        if (maps[i].acc > map1.acc)
                                        {
                                            maps.Insert(i, map1);
                                            done = true;
                                            break;
                                        }
                                    }
                                    if (!done)
                                        maps.Add(map1);
                                }
                                else if (order == "sniperPP")
                                {
                                    //insert into maps with highest map2 pp first
                                    bool done = false;
                                    for (int i = 0; i < maps.Count; i++)
                                    {
                                        if (maps[i].pp < map2.pp)
                                        {
                                            maps.Insert(i, map2);
                                            done = true;
                                            break;
                                        }
                                    }
                                    if (!done)
                                        maps.Add(map2);
                                }
                                else if (order == "targetPP")
                                    //no insertion here as targetPP SHOULD be the original order
                                    maps.Add(map1);
                            }
                        }
                    }
                }
                //add map even if it hasn't been played (only if includeUnplayed is true)
                if (!exist && includeUnplayed)
                {
                    //loop to check if map already exists in maps
                    bool exist2 = false;
                    foreach (Map x in maps)
                    {
                        if ((x.hash == map1.hash) && (x.difficulties[0].toString() == map1.difficulties[0].toString()))
                            exist2 = true;
                    }
                    if (!exist2)
                    {
                        if (order == "easiest")
                        {
                            //if order is easiest the acc value becomes -1 as there is no map2 to compare to
                            map1.acc = -1;
                            //it will just be added to the end of the playlist
                            maps.Add(map1);
                        }
                        else if (order == "sniperPP")
                        {
                            //if order is sniperPP the pp value becomes -1 as there is no map2 pp value
                            map1.pp = -1;
                            //it will just be added to the end of the playlist
                            maps.Add(map1);
                        }
                        else if (order == "targetPP")
                            maps.Add(map1);
                    }
                }
            }

            //convert list<Map> into list<Song>
            List<Song> songs = new List<Song>();
            foreach (Map x in maps)
                songs.Add(x);
            //create playlist object
            string imageAsBase64 = $"data:image/jpeg;base64,{image.convertToBase64()}";

            Playlist playlist = new Playlist(name, songs, imageAsBase64);

            //return as json string
            /*
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.IncludeFields = true;
            // return JsonSerializer.Serialize(playlist, options);
            */

            // JsonSerializerSettings settings = new JsonSerializerSettings();
            // settings.NullValueHandling = NullValueHandling.Include;

            return playlist; // JsonConvert.SerializeObject(playlist, settings);

        }

        //returns a list of map objects for the given parameters
        private static async Task<List<Map>> getMaps(long id, bool rankedOnly)
        {
            List<Map> maps = new List<Map>();
            HttpResponseMessage response1 = await ApiHelper.getResponse("/api/player/" + id + "/full");

            JObject data1 = JsonConvert.DeserializeObject<JObject>(ApiHelper.getResponseData(response1));
            int total;
            if (rankedOnly)
                total = (int)data1["scoreStats"]["rankedPlayCount"];
            else
                total = (int)data1["scoreStats"]["totalPlayCount"];
            int maxPage = ((total - 1) / 100) + 2;
            for (int i = 1; i < maxPage; i++)
            {
                // Plugin.Log.Info("hello you got to the first for loop");
                int limit;
                if (i < maxPage - 1)
                    limit = 100;
                else
                    limit = total - ((maxPage - 2) * 100);
                HttpResponseMessage response2 = await ApiHelper.getResponse("/api/player/" + id + "/scores?limit=" + limit + "&sort=top&page=" + i);
                JArray data2 = (JArray)JObject.Parse(ApiHelper.getResponseData(response2))["playerScores"];

                foreach (JObject x in data2)
                {
                    double pp = (double)x["score"]["pp"]; 
                    double acc = (double)x["score"]["baseScore"] / (double)x["leaderboard"]["maxScore"];
                    string hash = (string)x["leaderboard"]["songHash"];
                    Difficulty diff = new Difficulty(((string)x["leaderboard"]["difficulty"]["gameMode"]).Substring(4), (int)x["leaderboard"]["difficulty"]["difficulty"]);
                    maps.Add(new Map(pp, acc, hash, diff));
                }
            }
            return maps;
        }
    }
}
