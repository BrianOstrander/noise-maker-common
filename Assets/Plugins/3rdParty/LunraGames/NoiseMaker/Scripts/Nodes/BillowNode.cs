using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using System;

namespace LunraGames.NoiseMaker
{
	public class BillowNode : Node
	{
		public float Frequency = 0.02f;
		public float Lacunarity;
		public NoiseQuality Quality = NoiseQuality.Standard;
		public int OctaveCount = 1;
		public float Persistence;
		public int Seed = NoiseUtility.Seed;

		public override IModule GetModule (List<Node> nodes)
		{
			var billow = Module == null ? new Billow() : Module as Billow;
			billow.Frequency = Frequency;
			billow.Lacunarity = Lacunarity;
			billow.NoiseQuality = Quality;
			billow.OctaveCount = OctaveCount;
			billow.Persistence = Persistence;
			billow.Seed = Seed;

			Module = billow;
			return Module;
		}
	}
}