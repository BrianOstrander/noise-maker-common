using System;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	[Serializable]
	public abstract class Altitude
	{
		public string Id;
		public string Name;
		public float MinAltitude = -0.5f;
		public float MaxAltitude = 0.5f;

		public virtual Color GetSphereColor(float latitude, float longitude) { return Color.magenta; }
		public virtual Color GetPlaneColor(float x, float y) { return Color.magenta; }
	}
}