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

		public abstract Color GetColor(float latitude, float longitude);
	}
}