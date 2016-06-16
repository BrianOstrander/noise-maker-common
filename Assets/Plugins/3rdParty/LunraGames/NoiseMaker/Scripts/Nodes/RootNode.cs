using UnityEngine;
using System.Collections;
using System.Linq;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class RootNode : Node<IModule>
	{
		/// <summary>
		/// The source used if SourceIds[0] is null.
		/// </summary>
		[NodeLinker(0, true), JsonIgnore]
		public IModule Source;

		public RootNode()
		{
			InitializeSources(1);
		}

		public override IModule GetValue (List<INode> nodes)
		{
			var values = NullableValues(nodes);

			var source = GetLocalIfValueNull<IModule>(Source, 0, values);

			if (source == null) return null;

			var root = Value == null ?  new TranslateInput(source, 0f, 0f ,0f) : Value as TranslateInput;

			root.SourceModule = source;

			Value = root;

			return Value;
		/*
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
			*/
		}
	}
}