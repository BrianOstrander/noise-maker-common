using System;
using System.Linq;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public class RadialDomain : Domain
	{
		public Vector2 Center;
		public float Radius;

		public override float GetPlaneWeight(float x, float y, float altitude)
		{
			var distance = Vector2.Distance(Center, new Vector2(x, y));
			if (Radius < distance) return 0f;
			return 1f;// distance / Radius;
		}

		public override Color GetPlaneColor(float x, float y, float altitude, Mercator mercator)
		{
			// todo: this should be done in the parent domain class and sent down...
			var biome = mercator.Biomes.FirstOrDefault(b => b.Id == BiomeId);
			if (biome == null) return Color.magenta;
			return biome.GetPlaneColor(x, y, altitude, mercator);
		}
	}
}
