using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class MaxNode : Node
	{
		public override IModule GetModule (List<Node> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 2)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
				SourceIds.Add(null);
			}
			if (StringExtensions.IsNullOrWhiteSpace(SourceIds[0]) || StringExtensions.IsNullOrWhiteSpace(SourceIds[1])) return null;
			var sources = Sources(nodes);
			if (sources.Count != 2) return null;

			var max = Module == null ? new LargerOutput(sources[0], sources[1]) : Module as LargerOutput;

			max.SourceModule1 = sources[0];
			max.SourceModule2 = sources[1];

			Module = max;

			return Module;
		}
	}
}