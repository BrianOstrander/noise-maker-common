using System;
using UnityEngine;
using System.Collections.Generic;
using Atesh;
using LibNoise;
using System.Linq;
using Newtonsoft.Json;

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