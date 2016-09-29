using UnityEngine;

namespace LunraGames.NoiseMaker
{
	// todo: everything in here
	public class TextureAltitude : Altitude
	{
		public Texture2D Texture;

		public override Color GetSphereColor (float latitude, float longitude)
		{
			return Color.white;
		}
	}
}