using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class RootNode : Node
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

			var rootModule = Module == null ? sources[0] : Module as IModule;

			Module = rootModule;

			return Module;
		}
	}
}