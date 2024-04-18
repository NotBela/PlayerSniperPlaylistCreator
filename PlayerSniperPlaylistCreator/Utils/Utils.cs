﻿using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator.Utils
{
    public static class Utils
    {
        public static readonly string userId;

        static Utils()
        {
            userId = GetUserInfo().Id.ToString();
        }

        private static async Task<UserInfo> GetUserInfo()
        {
            var userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();

            return userInfo;
        }
    }
}
