using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class DisplaceNode : Node
	{
		public override IModule GetModule (List<Node> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 4)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
				SourceIds.Add(null);
				SourceIds.Add(null);
				SourceIds.Add(null);
			}
			foreach (var curr in SourceIds)
			{
				if (StringExtensions.IsNullOrWhiteSpace(curr)) return null;
			}

			var sources = Sources(nodes);
			if (sources.Count != 4) return null;

			var displace = Module == null ? new DisplaceInput(sources[0], sources[1], sources[2], sources[3]) : Module as DisplaceInput;

			displace.SourceModule = sources[0];
			displace.XDisplaceModule = sources[1];
			displace.YDisplaceModule = sources[2];
			displace.ZDisplaceModule = sources[3];

			Module = displace;

			return Module;
		}
	}
}