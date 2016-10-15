using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class MaxNode : Node<IModule>
	{
		/// <summary>
		/// The source used if SourceIds[0] is null.
		/// </summary>
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source0;
		/// <summary>
		/// The source used if SourceIds[1] is null.
		/// </summary>
		[NodeLinker(1, hide: true), JsonIgnore]
		public IModule Source1;

		public override IModule GetValue (Graph graph)
		{
			var values = NullableValues(graph);

			var source0 = GetLocalIfValueNull<IModule>(Source0, 0, values);
			var source1 = GetLocalIfValueNull<IModule>(Source1, 1, values);

			if (source0 == null || source1 == null) return null;

			var max = Value == null ? new LargerOutput(source0, source1) : Value as LargerOutput;

			max.SourceModule1 = source0;
			max.SourceModule2 = source1;

			Value = max;

			return Value;
		}
	}
}