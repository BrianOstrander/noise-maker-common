using UnityEngine;
using LibNoise;
using System.Collections.Generic;
using LibNoise.Modifiers;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class ClampNode : Node 
	{
		public float LowerBound = -1f;
		public float UpperBound = 1f;

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

			var clamp = Module == null ? new ClampOutput(sources[0]) : Module as ClampOutput;

			clamp.SourceModule = sources[0];

			clamp.SetBounds(LowerBound, UpperBound);

			Module = clamp;

			return Module;
		}
	}
}