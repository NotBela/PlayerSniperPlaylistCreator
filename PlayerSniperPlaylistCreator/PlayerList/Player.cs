namespace PlayerSniperPlaylistCreator.PlayerList
{
    internal class Player
    {

        public string name;
        public long id;

        public Player(long id, string name) // ADD DEFAULT HERE THAT GETS NAME AUTOMATICALLY
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
