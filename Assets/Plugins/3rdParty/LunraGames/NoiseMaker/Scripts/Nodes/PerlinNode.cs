using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using System;

namespace LunraGames.NoiseMaker
{
	public class PerlinNode : Node<IModule>
	{
		public float Frequency = 0.02f;
		public float Lacunarity;
		public NoiseQuality Quality = NoiseQuality.Standard;
		public int OctaveCount = 1;
		public float Persistence;
		public int Seed = NoiseUtility.Seed;

		public override IModule GetValue (List<INode> nodes)
		{
			var perlin = Value == null ? new Perlin() : Value as Perlin;
			perlin.Frequency = Frequency;
			perlin.Lacunarity = Lacunarity;
			perlin.NoiseQuality = Quality;
			perlin.OctaveCount = OctaveCount;
			perlin.Persistence = Persistence;
			perlin.Seed = Seed;

			Value = perlin;
			return Value;
		}
	}
}