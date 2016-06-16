using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class MaxNode : Node<IModule>
	{
		public override IModule GetValue (List<INode> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 2)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
				SourceIds.Add(null);
			}
			if (StringExtensions.IsNullOrWhiteSpace(SourceIds[0]) || StringExtensions.IsNullOrWhiteSpace(SourceIds[1])) return null;
			var sources = Values(nodes);
			if (sources.Count != 2) return null;

			var max = Value == null ? new LargerOutput(sources[0] as IModule, sources[1] as IModule) : Value as LargerOutput;

			max.SourceModule1 = sources[0] as IModule;
			max.SourceModule2 = sources[1] as IModule;

			Value = max;

			return Value;
		}
	}
}