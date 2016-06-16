using UnityEngine;
using LibNoise;
using System.Collections.Generic;
using LibNoise.Modifiers;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class InvertNode : Node<IModule> 
	{
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

			var invert = Value == null ? new InvertOutput(sources[0] as IModule) : Value as InvertOutput;

			invert.SourceModule = sources[0] as IModule;

			Value = invert;

			return Value;
		}
	}
}