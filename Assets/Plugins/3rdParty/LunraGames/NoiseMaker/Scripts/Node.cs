using System;
using UnityEngine;
using System.Collections.Generic;
using Atesh;
using LibNoise;
using System.Linq;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	[Serializable]
	public abstract class Node
	{
		#region Inspector
		public Vector2 EditorPosition = Vector2.zero;
		#endregion

		public string Id;
		public string ModuleType;
		public List<string> SourceIds = new List<string>();

		[NonSerialized]
		protected IModule Module;

		public abstract IModule GetModule(List<Node> nodes);

		protected List<IModule> Sources(List<Node> nodes, params string[] sources)
		{
			if (nodes == null) throw new ArgumentNullException("nodes");
			var result = new List<IModule>();
			foreach (var source in sources)
			{
				if (StringExtensions.IsNullOrWhiteSpace(source)) throw new ArgumentNullOrEmptyException("sources", "Array \"sources\" can't contain a null or empty string");
				var node = nodes.FirstOrDefault(n => n.Id == source);
				if (node == null) throw new ArgumentOutOfRangeException("sources", "No node found for \""+sources+"\"");
				result.Add(node.GetModule(nodes));
			}
			return result;
		}
	}
}