using UnityEngine;
using LibNoise;
using System.Collections.Generic;
using LibNoise.Modifiers;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class ClampNode : Node<IModule> 
	{
		public float LowerBound = -1f;
		public float UpperBound = 1f;

		public override IModule GetValue (List<INode> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 1)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
			}
			if (StringExtensions.IsNullOrWhiteSpace(SourceIds[0])) return null;
			var sources = Values(nodes);
			if (sources.Count != 1) return null;

			var clamp = Value == null ? new ClampOutput(sources[0] as IModule) : Value as ClampOutput;

			clamp.SourceModule = sources[0] as IModule;

			clamp.SetBounds(LowerBound, UpperBound);

			Value = clamp;

			return Value;
		}
	}
}