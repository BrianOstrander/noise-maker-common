using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class AddNode : Node<IModule>
	{
		public override IModule GetValue (List<INode> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 2)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
				SourceIds.Add(null);
			}
			if (StringExtensions.IsNullOrWhiteSpace(SourceIds[0]) || StringExtensions.IsNullOrWhiteSpace(SourceIds[1])) return null;
			var sources = Values(nodes);
			if (sources.Count != 2) return null;

			var addModule = Value == null ? new Add(sources[0] as IModule, sources[1] as IModule) : Value as Add;

			addModule.SourceModule1 = sources[0] as IModule;
			addModule.SourceModule2 = sources[1] as IModule;

			Value = addModule;

			return Value;
		}
	}
}