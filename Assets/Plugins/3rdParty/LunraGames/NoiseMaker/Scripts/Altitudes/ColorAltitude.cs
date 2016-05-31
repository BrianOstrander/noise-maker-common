using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public class ColorAltitude : Altitude
	{
		public Color Color;

		public override Color GetColor (float latitude, float longitude)
		{
			return Color;
		}
	}
}