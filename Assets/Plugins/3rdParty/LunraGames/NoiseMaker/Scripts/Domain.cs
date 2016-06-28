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
		/// <summary>
		/// The id of the associated Biome.
		/// </summary>
		public string BiomeId;

		public abstract float GetWeight(float latitude, float longitude, float altitude);

		public abstract Color GetColor(float latitude, float longitude, float altitude);

		public Biome GetBiome(Mercator mercator)
		{
			if (string.IsNullOrEmpty(BiomeId)) return null;
			else return mercator.Biomes.FirstOrDefault(b => b.Id == BiomeId);
		}
	}
}
