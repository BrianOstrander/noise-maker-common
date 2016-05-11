using System;
using System.Collections.Generic;
using LibNoise;
using Atesh;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public class Graph
	{
		public List<Node> Nodes = new List<Node>();
		public string RootId;
		IModule _Root;

		[JsonIgnore]
		public IModule Root 
		{ 
			get 
			{
				if (StringExtensions.IsNullOrWhiteSpace(RootId)) throw new NullReferenceException("No RootId has been set");
				if (_Root == null)
				{
					var node = Nodes.FirstOrDefault(n => n.Id == RootId);
					if (node == null) throw new NullReferenceException("No node found for the RootId \""+RootId+"\"");
					_Root = node.GetModule(Nodes);
				}
				return _Root;
			}
		}

		public void Remove(Node node)
		{
			if (node == null) throw new ArgumentNullException("node");

			foreach (var curr in Nodes)
			{
				for (var i = 0; i < curr.SourceIds.Count; i++)
				{
					if (curr.SourceIds[i] == node.Id) curr.SourceIds[i] = null;
				}
			}
			Nodes.Remove(node);
		}
	}
}