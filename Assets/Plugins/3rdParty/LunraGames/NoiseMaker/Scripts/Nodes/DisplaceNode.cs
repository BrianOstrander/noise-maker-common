using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class DisplaceNode : Node<IModule>
	{
		public override IModule GetValue (List<INode> nodes)
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

			var displace = Value == null ? new DisplaceInput(sources[0] as IModule, sources[1] as IModule, sources[2] as IModule, sources[3] as IModule) : Value as DisplaceInput;

			displace.SourceModule = sources[0] as IModule;
			displace.XDisplaceModule = sources[1] as IModule;
			displace.YDisplaceModule = sources[2] as IModule;
			displace.ZDisplaceModule = sources[3] as IModule;

			Value = displace;

			return Value;
		}
	}
}