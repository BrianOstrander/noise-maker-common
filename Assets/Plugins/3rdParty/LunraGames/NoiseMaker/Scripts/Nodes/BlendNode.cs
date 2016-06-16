using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class BlendNode : Node<IModule>
	{
		public override IModule GetValue (List<INode> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 3)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
				SourceIds.Add(null);
				SourceIds.Add(null);
			}
			if (StringExtensions.IsNullOrWhiteSpace(SourceIds[0]) || StringExtensions.IsNullOrWhiteSpace(SourceIds[1]) || StringExtensions.IsNullOrWhiteSpace(SourceIds[2])) return null;
			var sources = Values(nodes);
			if (sources.Count != 3) return null;

			var blend = Value == null ? new Blend(sources[0] as IModule, sources[1] as IModule, sources[2] as IModule) : Value as Blend;

			blend.SourceModule1 = sources[0] as IModule;
			blend.SourceModule2 = sources[1] as IModule;
			blend.WeightModule = sources[2] as IModule;

			Value = blend;

			return Value;
		}
	}
}