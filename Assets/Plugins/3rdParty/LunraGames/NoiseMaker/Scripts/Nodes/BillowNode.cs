using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LunraGames.NumberDemon;

namespace LunraGames.NoiseMaker
{
	public class BillowNode : Node<IModule>
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
		public float Persistence;
		[NodeLinker(5)]
		public int Seed = DemonUtility.NextInteger;

		public override IModule GetValue (Graph graph)
		{
			var values = NullableValues(graph);

			var billow = Value == null ? new Billow() : Value as Billow;

			billow.Frequency = GetLocalIfValueNull<float>(Frequency, 0, values);
			billow.Lacunarity = GetLocalIfValueNull<float>(Lacunarity, 1, values);
			billow.NoiseQuality = GetLocalIfValueNull<NoiseQuality>(Quality, 2, values);
			billow.OctaveCount = Mathf.Clamp(GetLocalIfValueNull<int>(OctaveCount, 3, values), 1, 29);
			billow.Persistence = GetLocalIfValueNull<float>(Persistence, 4, values);
			billow.Seed = GetLocalIfValueNull<int>(Seed, 5, values);

			Value = billow;
			return Value;
		}
	}
}