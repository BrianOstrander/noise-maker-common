using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public class ColorAltitude : Altitude
	{
		public Color Color = Color.white;

		public override Color GetColor (float latitude, float longitude)
		{
			return Color;
		}
	}
}