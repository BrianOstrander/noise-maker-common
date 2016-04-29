using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using System;

namespace LunraGames.NoiseMaker
{
	public class PerlinNode : Node
	{
		public float Frequency;
		public float Lacunarity;
		public NoiseQuality Quality;
		public int OctaveCount = 1;
		public float Persistence;
		public int Seed;

		public override IModule GetModule (List<Node> nodes)
		{
			var perlin = Module == null ? new Perlin() : Module as Perlin;
			perlin.Frequency = Frequency;
			perlin.Lacunarity = Lacunarity;
			perlin.NoiseQuality = Quality;
			perlin.OctaveCount = OctaveCount;
			perlin.Persistence = Persistence;
			perlin.Seed = Seed;

			Module = perlin;
			return Module;
		}
	}
}