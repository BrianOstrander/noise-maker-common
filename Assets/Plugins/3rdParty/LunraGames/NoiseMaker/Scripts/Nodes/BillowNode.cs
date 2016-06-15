using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using System;

namespace LunraGames.NoiseMaker
{
	public class BillowNode : Node<IModule>
	{
		public float Frequency = 0.02f;
		public float Lacunarity;
		public NoiseQuality Quality = NoiseQuality.Standard;
		public int OctaveCount = 1;
		public float Persistence;
		public int Seed = NoiseUtility.Seed;

		public override IModule GetValue (List<INode> nodes)
		{
			var billow = Value == null ? new Billow() : Value as Billow;
			billow.Frequency = Frequency;
			billow.Lacunarity = Lacunarity;
			billow.NoiseQuality = Quality;
			billow.OctaveCount = OctaveCount;
			billow.Persistence = Persistence;
			billow.Seed = Seed;

			Value = billow;
			return Value;
		}
	}
}