using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class AddNode : Node
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

			var addModule = Module == null ? new Add(sources[0], sources[1]) : Module as Add;

			addModule.SourceModule1 = sources[0];
			addModule.SourceModule2 = sources[1];

			Module = addModule;

			return Module;
		}
	}
}