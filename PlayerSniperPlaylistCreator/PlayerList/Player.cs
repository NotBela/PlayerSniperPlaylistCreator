namespace PlayerSniperPlaylistCreator.PlayerList
{
    internal class Player
    {

        public string name;
        public string id;

        public Player(string id, string name) // ADD DEFAULT HERE THAT GETS NAME AUTOMATICALLY
        {
            this.name = name;
            this.id = id;
        }

        public override string ToString()
        {
            return name;
        }

    }
}
