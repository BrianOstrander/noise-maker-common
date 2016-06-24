using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class SelectNode : Node<IModule>
	{
		/// <summary>
		/// The source used if SourceIds[0] is null.
		/// </summary>
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Control;
		/// <summary>
		/// The source used if SourceIds[1] is null.
		/// </summary>
		[NodeLinker(1, hide: true), JsonIgnore]
		public IModule Source0;
		/// <summary>
		/// The source used if SourceIds[2] is null.
		/// </summary>
		[NodeLinker(2, hide: true), JsonIgnore]
		public IModule Source1;
		[NodeLinker(3)]
		public float EdgeFalloff;
		[NodeLinker(4)]
		public float LowerBound = -1f;
		[NodeLinker(5)]
		public float UpperBound = 1f;

		public override IModule GetValue (Graph graph)
		{
			var values = NullableValues(graph);

			var control = GetLocalIfValueNull<IModule>(Control, 0, values);
			var source0 = GetLocalIfValueNull<IModule>(Source0, 1, values);
			var source1 = GetLocalIfValueNull<IModule>(Source1, 2, values);

			if (control == null || source0 == null || source1 == null) return null;

			var edgeFalloff = GetLocalIfValueNull<float>(EdgeFalloff, 3, values);
			var lowerBound = GetLocalIfValueNull<float>(LowerBound, 4, values);
			var upperBound = GetLocalIfValueNull<float>(UpperBound, 5, values);

			var selector = Value == null ? new Select(control, source0, source1) : Value as Select;

			selector.ControlModule = control;
			selector.SourceModule1 = source0;
			selector.SourceModule2 = source1;
			selector.EdgeFalloff = edgeFalloff;

			try 
			{
				selector.SetBounds(lowerBound, upperBound);
			}
			catch
			{
				return null;
			}

			Value = selector;

			return Value;
		}
	}
}