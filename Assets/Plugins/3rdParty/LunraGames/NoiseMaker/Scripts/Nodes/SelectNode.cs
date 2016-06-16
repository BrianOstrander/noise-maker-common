using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class SelectNode : Node<IModule>
	{
		public float EdgeFalloff = 0f;
		public float LowerBound = -1f;
		public float UpperBound = 1f;

		public override IModule GetValue (List<INode> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 3)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
				SourceIds.Add(null);
				SourceIds.Add(null);
			}
			foreach (var curr in SourceIds)
			{
				if (StringExtensions.IsNullOrWhiteSpace(curr)) return null;
			}

			var sources = Values(nodes);
			if (sources.Count != 3) return null;

			var selector = Value == null ? new Select(sources[0] as IModule, sources[1] as IModule, sources[2] as IModule) : Value as Select;

			selector.SourceModule1 = sources[0] as IModule;
			selector.SourceModule2 = sources[1] as IModule;
			selector.ControlModule = sources[2] as IModule;
			selector.EdgeFalloff = EdgeFalloff;
			selector.SetBounds(LowerBound, UpperBound);

			Value = selector;

			return Value;
		}
	}
}