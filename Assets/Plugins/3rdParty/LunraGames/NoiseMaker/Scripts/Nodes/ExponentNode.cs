using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class ExponentNode : Node<IModule>
	{
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source;
		[NodeLinker(1)]
		public float Exponent;

		public override IModule GetValue (Graph graph)
		{
			var values = NullableValues(graph);
			var source = GetLocalIfValueNull<IModule>(Source, 0, values);

			if (source == null) return null;

			var value = GetLocalIfValueNull<float>(Exponent, 1, values);

			var exponent = Value == null ? new ExponentialOutput(source, value) : Value as ExponentialOutput;

			exponent.SourceModule = source;
			exponent.Exponent = value;

			Value = exponent;
			return Value;
		}
	}
}