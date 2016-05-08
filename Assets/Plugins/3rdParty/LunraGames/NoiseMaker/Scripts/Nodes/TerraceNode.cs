using UnityEngine;
using LibNoise;
using System.Collections.Generic;
using LibNoise.Modifiers;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class TerraceNode : Node 
	{
		/// <summary>
		/// The points that define the terraced output, where X is input/time, Y is output/value.
		/// </summary>
		public List<float> Points;

		public override IModule GetModule (List<Node> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 1)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
			}
			if (StringExtensions.IsNullOrWhiteSpace(SourceIds[0])) return null;
			var sources = Sources(nodes);
			if (sources.Count != 1) return null;

			var terrace = Module == null ? new Terrace(sources[0]) : Module as Terrace;

			terrace.SourceModule = sources[0];

			if (Points == null)
			{
				// newtonsoft seems to have a weird tendancy to *add* to an existing list, instead of replacing it, so we define it here.
				Points = new List<float> { -1f, 1f };
			}

			terrace.ControlPoints = new List<double>();
			foreach (var point in Points) terrace.ControlPoints.Add(point);

			Module = terrace;

			return Module;
		}
	}
}