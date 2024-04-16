using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSniperPlaylistCreator
{
    public static class Api
    {
        // api register, create clients from here
        // i couldnt think of a better way to do this D:
        public static ApiHelper scoresaber = new ApiHelper("https://scoresaber.com/");
    }
}
