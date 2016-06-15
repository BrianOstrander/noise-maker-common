using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class AbsoluteNode : Node<IModule>
	{
		public override IModule GetValue (List<INode> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 1)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
			}
			if (StringExtensions.IsNullOrWhiteSpace(SourceIds[0])) return null;
			var sources = Sources(nodes);
			if (sources.Count != 1) return null;

			var source = sources[0] as IModule;

			var absolute = Value == null ? new AbsoluteOutput(source) : Value as AbsoluteOutput;

			absolute.SourceModule = source;

			Value = absolute;

			return Value;
		}
	}
}