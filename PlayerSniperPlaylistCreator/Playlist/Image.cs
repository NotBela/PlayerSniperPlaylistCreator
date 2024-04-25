using System;

namespace PlayerSniperPlaylistCreator.Playlist
{
	public class Image
	{

		private byte[] imageInQuestion;

		public Image(byte[] imageInQuestion)
		{
			this.imageInQuestion = imageInQuestion;
		}

		public string convertToBase64()
		{
			return Convert.ToBase64String(imageInQuestion);
		}
	}
}