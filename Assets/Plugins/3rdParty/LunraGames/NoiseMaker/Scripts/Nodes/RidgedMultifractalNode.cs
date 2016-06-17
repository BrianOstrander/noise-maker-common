using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using System;

namespace LunraGames.NoiseMaker
{
	public class RidgedMultifractalNode : Node<IModule>
	{
		[NodeLinker(0)]
		public float Frequency = 0.02f;
		[NodeLinker(1)]
		public float Lacunarity;
		[NodeLinker(2)]
		public NoiseQuality Quality = NoiseQuality.Standard;
		[NodeLinker(3, 1, 29)]
		public int OctaveCount = 1;
		[NodeLinker(4)]
		public int Seed = NoiseUtility.Seed;

		public override IModule GetValue (List<INode> nodes)
		{
			var values = NullableValues(nodes);

			var ridged = Value == null ? new RidgedMultifractal() : Value as RidgedMultifractal;

			ridged.Frequency = GetLocalIfValueNull<float>(Frequency, 0, values);
			ridged.Lacunarity = GetLocalIfValueNull<float>(Lacunarity, 1, values);
			ridged.OctaveCount = GetLocalIfValueNull<int>(OctaveCount, 2, values);
			ridged.NoiseQuality = GetLocalIfValueNull<NoiseQuality>(Quality, 3, values);
			ridged.Seed = GetLocalIfValueNull<int>(Seed, 4, values);

			Value = ridged;
			return Value;
		}
	}
}