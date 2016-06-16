using UnityEngine;
using System.Collections;
using System.Linq;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class RootNode : Node<IModule>
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
			if (sources.Count != 1 || sources[0] == null) return null;

			// This is my lazy hack, I couldn't grok how I can do a root node without actually defining a new IModule...
			var root = Value == null ?  new TranslateInput(sources[0] as IModule, 0f, 0f ,0f) : Value as TranslateInput;

			root.SourceModule = sources[0] as IModule;

			Value = root;

			return Value;
		}
	}
}