using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class BlendNode : Node
	{
		public override IModule GetModule (List<Node> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 3)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
				SourceIds.Add(null);
				SourceIds.Add(null);
			}
			if (StringExtensions.IsNullOrWhiteSpace(SourceIds[0]) || StringExtensions.IsNullOrWhiteSpace(SourceIds[1]) || StringExtensions.IsNullOrWhiteSpace(SourceIds[2])) return null;
			var sources = Sources(nodes);
			if (sources.Count != 3) return null;

			var blend = Module == null ? new Blend(sources[0], sources[1], sources[2]) : Module as Blend;

			blend.SourceModule1 = sources[0];
			blend.SourceModule2 = sources[1];
			blend.WeightModule = sources[2];

			Module = blend;

			return Module;
		}
	}
}