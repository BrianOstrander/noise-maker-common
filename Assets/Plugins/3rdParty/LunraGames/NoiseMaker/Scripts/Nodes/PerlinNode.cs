using System.Collections.Generic;
using LibNoise;
using LunraGames.NumberDemon;

namespace LunraGames.NoiseMaker
{
	public class PerlinNode : Node<IModule>
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

			var perlin = Value == null ? new Perlin() : Value as Perlin;

			perlin.Frequency = GetLocalIfValueNull<float>(Frequency, 0, values);
			perlin.Lacunarity = GetLocalIfValueNull<float>(Lacunarity, 1, values);
			perlin.NoiseQuality = GetLocalIfValueNull<NoiseQuality>(Quality, 2, values);
			perlin.OctaveCount = GetLocalIfValueNull<int>(OctaveCount, 3, values);
			perlin.Persistence = GetLocalIfValueNull<float>(Persistence, 4, values);
			perlin.Seed = GetLocalIfValueNull<int>(Seed, 5, values);

			Value = perlin;
			return Value;
		}
	}
}