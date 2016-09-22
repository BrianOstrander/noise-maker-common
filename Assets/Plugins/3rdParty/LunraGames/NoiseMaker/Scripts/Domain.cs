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

		public abstract float GetWeight(float latitude, float longitude, float altitude);

		public abstract Color GetColor(float latitude, float longitude, float altitude, Mercator mercator);

		public Biome GetBiome(Mercator mercator)
		{
			return string.IsNullOrEmpty(BiomeId) ? null : mercator.Biomes.FirstOrDefault(b => b.Id == BiomeId);
		}
	}
}
