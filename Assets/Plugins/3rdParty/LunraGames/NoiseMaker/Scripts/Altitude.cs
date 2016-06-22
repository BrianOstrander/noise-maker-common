using System;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	[Serializable]
	public abstract class Altitude
	{
		public string Id;
		public float MinAltitude;
		public float MaxAltitude;

		public abstract Color GetColor(float latitude, float longitude);
	}
}