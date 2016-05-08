using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class SelectNode : Node
	{
		public float EdgeFalloff = 0f;
		public float LowerBound = -1f;
		public float UpperBound = 1f;

		public override IModule GetModule (List<Node> nodes)
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

			var sources = Sources(nodes);
			if (sources.Count != 3) return null;

			var selector = Module == null ? new Select(sources[0], sources[1], sources[2]) : Module as Select;

			selector.SourceModule1 = sources[0];
			selector.SourceModule2 = sources[1];
			selector.ControlModule = sources[2];
			selector.EdgeFalloff = EdgeFalloff;
			selector.SetBounds(LowerBound, UpperBound);

			Module = selector;

			return Module;
		}
	}
}