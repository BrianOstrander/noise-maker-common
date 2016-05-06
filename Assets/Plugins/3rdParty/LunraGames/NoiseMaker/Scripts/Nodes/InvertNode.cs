using UnityEngine;
using LibNoise;
using System.Collections.Generic;
using LibNoise.Modifiers;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class InvertNode : Node 
	{
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

			var invert = Module == null ? new InvertOutput(sources[0]) : Module as InvertOutput;

			invert.SourceModule = sources[0];

			Module = invert;

			return Module;
		}
	}
}