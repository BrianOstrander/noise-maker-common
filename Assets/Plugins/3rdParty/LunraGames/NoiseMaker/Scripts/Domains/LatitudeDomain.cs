using System.Linq;
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
			var delta = latitude - MinLatitude;
			var scalar = delta / (MaxLatitude - MinLatitude);
			return 1f - (Mathf.Abs(scalar - 0.5f) / 0.5f);
		}

		public override Color GetColor (float latitude, float longitude, float altitude, Mercator mercator)
		{
			var biome = mercator.Biomes.FirstOrDefault(b => b.Id == BiomeId);
			if (biome == null) return Color.magenta;
			return biome.GetColor(latitude, longitude, altitude, mercator);
		}
	}
}
