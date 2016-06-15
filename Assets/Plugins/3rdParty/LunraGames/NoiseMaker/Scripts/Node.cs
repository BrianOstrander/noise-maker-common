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
	public abstract class Node<T> : INode
	{

		#region Inspector
		public Vector2 EditorPosition { get; set; }
		#endregion

		public string Id { get; set; }
		public List<string> SourceIds { get { return _SourceIds; } set { _SourceIds = value; } }

		List<string> _SourceIds = new List<string>();

		[NonSerialized]
		protected T Value;

		public abstract T GetValue(List<INode> nodes);

		public object GetRawValue(List<INode> nodes)
		{
			return GetValue(nodes);
		}

		protected List<object> Sources(List<INode> nodes, params string[] sources)
		{
			if (nodes == null) throw new ArgumentNullException("nodes");
			var result = new List<object>();
			var ids = sources.Length == 0 ? SourceIds.ToArray() : sources;
			foreach (var source in ids)
			{
				if (StringExtensions.IsNullOrWhiteSpace(source)) throw new ArgumentNullOrEmptyException("sources", "Array \"sources\" can't contain a null or empty string");
				var node = nodes.FirstOrDefault(n => n.Id == source);
				if (node == null) throw new ArgumentOutOfRangeException("sources", "No node found for \""+sources+"\"");
				result.Add(node.GetRawValue(nodes));
			}
			return result;
		}
	}
	/*
	[Serializable]
	public abstract class INode
	{
		#region Inspector
		public Vector2 EditorPosition = Vector2.zero;
		#endregion

		public string Id;
		public List<string> SourceIds = new List<string>();

		[NonSerialized]
		protected IModule Value;

		public abstract IModule GetValue(List<INode> nodes);

		protected List<IModule> Sources(List<INode> nodes, params string[] sources)
		{
			if (nodes == null) throw new ArgumentNullException("nodes");
			var result = new List<IModule>();
			var ids = sources.Length == 0 ? SourceIds.ToArray() : sources;
			foreach (var source in ids)
			{
				if (StringExtensions.IsNullOrWhiteSpace(source)) throw new ArgumentNullOrEmptyException("sources", "Array \"sources\" can't contain a null or empty string");
				var node = nodes.FirstOrDefault(n => n.Id == source);
				if (node == null) throw new ArgumentOutOfRangeException("sources", "No node found for \""+sources+"\"");
				result.Add(node.GetValue(nodes));
			}
			return result;
		}
	}
	*/
}