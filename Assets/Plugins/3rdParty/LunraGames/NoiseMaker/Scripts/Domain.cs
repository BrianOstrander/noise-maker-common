using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LunraGames.NoiseMaker
{
	[Serializable]
	public abstract class Domain
	{
		public string Id;
		public string Name;
		/// <summary>
		/// The id of the associated Biome.
		/// </summary>
		public string BiomeId;

		public virtual float GetSphereWeight(float latitude, float longitude, float altitude) { return 0f; }
		public virtual Color GetSphereColor(float latitude, float longitude, float altitude, Mercator mercator) { return Color.magenta; }

		public virtual float GetPlaneWeight(float x, float y, float altitude) { return 0f; }
		public virtual Color GetPlaneColor(float x, float y, float altitude, Mercator mercator) { return Color.magenta; }

		public Biome GetBiome(Mercator mercator)
		{
			return string.IsNullOrEmpty(BiomeId) ? null : mercator.Biomes.FirstOrDefault(b => b.Id == BiomeId);
		}
	}
}
