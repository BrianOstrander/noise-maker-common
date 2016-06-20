using LibNoise;
using System.Collections.Generic;
using LibNoise.Modifiers;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class TerraceNode : Node<IModule> 
	{
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source;
		/// <summary>
		/// The points that define the terraced output, where X is input/time, Y is output/value.
		/// </summary>
		public List<float> Points;

		public override IModule GetValue (List<INode> nodes)
		{
			var source = GetLocalIfValueNull<IModule>(Source, 0, nodes);

			if (source == null) return null;

			var terrace = Value == null ? new Terrace(source) : Value as Terrace;

			terrace.SourceModule = source;

			if (Points == null)
			{
				// newtonsoft seems to have a weird tendancy to *add* to an existing list, instead of replacing it, so we define it here.
				Points = new List<float> { -1f, 1f };
			}

			terrace.ControlPoints = terrace.ControlPoints ?? new List<double>();

			for (var i = 0; i < Points.Count; i++)
			{
				if (i < terrace.ControlPoints.Count) terrace.ControlPoints[i] = Points[i];
				else terrace.ControlPoints.Add(Points[i]);
			}

			Value = terrace;

			return Value;
		}
	}
}