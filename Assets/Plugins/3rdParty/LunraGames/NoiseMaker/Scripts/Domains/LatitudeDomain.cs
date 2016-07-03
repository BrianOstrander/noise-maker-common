using System;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public class LatitudeDomain : Domain
	{
		public float MinLatitude;
		public float MaxLatitude;

		public override float GetWeight (float latitude, float longitude, float altitude, Mercator mercator)
		{
			if (latitude < MinLatitude || MaxLatitude < latitude) return 0f;
			else return 1f;
		}

		public override Color GetColor (float latitude, float longitude, float altitude, Mercator mercator)
		{
			return Color.white;
		}
	}
}
