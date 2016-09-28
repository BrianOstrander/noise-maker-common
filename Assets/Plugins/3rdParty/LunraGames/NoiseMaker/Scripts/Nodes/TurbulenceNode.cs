using System.Collections.Generic;
using LibNoise;
using Newtonsoft.Json;
using LunraGames.NumberDemon;

namespace LunraGames.NoiseMaker
{
	public class TurbulenceNode : Node<IModule>
	{
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source;
		[NodeLinker(1)]
		public float Frequency = 0.02f;
		[NodeLinker(2)]
		public float Power;
		[NodeLinker(3, 1, 29)]
		public int Roughness = 1;
		[NodeLinker(4)]
		public int Seed = DemonUtility.NextInteger;

		public override IModule GetValue (Graph graph)
		{
			var values = NullableValues(graph);
			var source = GetLocalIfValueNull(Source, 0, values);

			if (source == null) return null;

			var turbulence = Value == null ? new Turbulence(source) : Value as Turbulence;

			turbulence.SourceModule = source;

			turbulence.Frequency = GetLocalIfValueNull(Frequency, 1, values);
			turbulence.Power = GetLocalIfValueNull(Power, 2, values);
			turbulence.Roughness = GetLocalIfValueNull(Roughness, 3, values);
			turbulence.Seed = GetLocalIfValueNull(Seed, 4, values);

			Value = turbulence;
			return Value;
		}
	}
}