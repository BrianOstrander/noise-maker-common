using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class BlendNode : Node<IModule>
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
		/// <summary>
		/// The source used if SourceIds[2] is null.
		/// </summary>
		[NodeLinker(2, hide: true), JsonIgnore]
		public IModule Weight;

		public override IModule GetValue (Graph graph)
		{
			var values = NullableValues(graph);

			var source0 = GetLocalIfValueNull<IModule>(Source0, 0, values);
			var source1 = GetLocalIfValueNull<IModule>(Source1, 1, values);
			var weight = GetLocalIfValueNull<IModule>(Weight, 2, values);

			if (source0 == null || source1 == null || weight == null) return null;

			var blend = Value == null ? new Blend(source0, source1, weight) : Value as Blend;

			blend.SourceModule1 = source0;
			blend.SourceModule2 = source1;
			blend.WeightModule = weight;

			Value = blend;

			return Value;
		}
	}
}