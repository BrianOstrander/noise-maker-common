using LibNoise;
using System.Collections.Generic;
using LibNoise.Modifiers;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class ClampNode : Node<IModule> 
	{
		/// <summary>
		/// The source used if SourceIds[0] is null.
		/// </summary>
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source;
		[NodeLinker(1)]
		public float LowerBound = -1f;
		[NodeLinker(2)]
		public float UpperBound = 1f;

		public override IModule GetValue (Graph graph)
		{
			var values = NullableValues(graph);
			var source = GetLocalIfValueNull<IModule>(Source, 0, values);

			if (source == null) return null;

			var clamp = Value == null ? new ClampOutput(source) : Value as ClampOutput;

			clamp.SourceModule = source;

			LowerBound = GetLocalIfValueNull<float>(LowerBound, 1, values);
			UpperBound = GetLocalIfValueNull<float>(UpperBound, 2, values);
			
			try 
			{
				clamp.SetBounds(LowerBound, UpperBound);
			}
			catch
			{
				return null;
			}

			Value = clamp;

			return Value;
		}
	}
}