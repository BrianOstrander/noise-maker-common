using UnityEngine;
using LibNoise;
using System.Collections.Generic;
using LibNoise.Modifiers;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class TerraceNode : Node<IModule> 
	{
		/// <summary>
		/// The points that define the terraced output, where X is input/time, Y is output/value.
		/// </summary>
		public List<float> Points;

		public override IModule GetValue (List<INode> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 1)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
			}
			if (StringExtensions.IsNullOrWhiteSpace(SourceIds[0])) return null;
			var sources = Sources(nodes);
			if (sources.Count != 1) return null;

			var terrace = Value == null ? new Terrace(sources[0] as IModule) : Value as Terrace;

			terrace.SourceModule = sources[0] as IModule;

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