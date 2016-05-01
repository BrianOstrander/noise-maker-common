using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using System;

namespace LunraGames.NoiseMaker
{
	public class RidgedMultifractalNode : Node
	{
		public float Frequency = 0.02f;
		public float Lacunarity;
		public NoiseQuality Quality = NoiseQuality.Standard;
		public int OctaveCount = 1;
		public int Seed = NoiseUtility.Seed;

		public override IModule GetModule (List<Node> nodes)
		{
			var ridged = Module == null ? new RidgedMultifractal() : Module as RidgedMultifractal;
			ridged.Frequency = Frequency;
			ridged.Lacunarity = Lacunarity;
			ridged.OctaveCount = OctaveCount;
			ridged.NoiseQuality = Quality;
			ridged.Seed = Seed;

			Module = ridged;
			return Module;
		}
	}
}