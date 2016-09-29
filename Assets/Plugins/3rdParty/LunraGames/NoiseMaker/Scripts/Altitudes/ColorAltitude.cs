using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public class ColorAltitude : Altitude
	{
		public Color Color = Color.white;

		public override Color GetSphereColor (float latitude, float longitude)
		{
			return Color;
		}

		public override Color GetPlaneColor(float x, float y)
		{
			return Color;
		}
	}
}