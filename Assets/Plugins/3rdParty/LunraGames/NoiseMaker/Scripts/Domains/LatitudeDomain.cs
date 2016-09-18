using System;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public class LatitudeDomain : Domain
	{
		public float MinLatitude;
		public float MaxLatitude;

		public override float GetWeight (float latitude, float longitude, float altitude)
		{
			if (latitude < MinLatitude || MaxLatitude < latitude) return 0f;
			return 1f;
		}

		public override Color GetColor (float latitude, float longitude, float altitude, Mercator mercator, out float weight)
		{
			weight = GetWeight(latitude, longitude, altitude);
			return Color.white;
		}
	}
}
