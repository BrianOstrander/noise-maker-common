using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using System;

namespace LunraGames.NoiseMaker
{
	[Serializable]
	public class PerlinNode : Node
	{
		public float Frequency;
		public float Lacunarity;
		//public NoiseQuality Quality;
		public int OctaveCount;
		public float Persistence;
		public int Seed;

		public override IModule GetModule (List<Node> nodes)
		{
			if (Module == null) 
			{
				var perlin = new Perlin();
				perlin.Frequency = Frequency;
				perlin.Lacunarity = Lacunarity;
				//perlin.NoiseQuality = Quality;
				perlin.OctaveCount = OctaveCount;
				perlin.Persistence = Persistence;
				perlin.Seed = Seed;

				Module = perlin;
			}
			return Module;
		}
	}
}