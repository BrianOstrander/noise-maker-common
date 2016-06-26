using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	[Serializable]
	public abstract class Node<T> : INode
	{
		// Analysis disable StaticFieldInGenericType
		static Dictionary<Type, int> CachedSourceCounts = new Dictionary<Type, int>();
		// Analysis restore StaticFieldInGenericType

		#region Inspector
		public Vector2 EditorPosition { get; set; }
		#endregion
		[JsonIgnore]
		public int SourceCount { get; private set; }
		public string Id { get; set; }
		public List<string> SourceIds { get; set; }

		/// <summary>
		/// The value that represents this Node. It could be stored locally, or derived from several input nodes. 
		/// This really shouldn't be set or retrived directly unless you know what you're doing!
		/// </summary>
		[NonSerialized, JsonIgnore]
		public T Value;

		/// <summary>
		/// Initializes a new instance of the <see cref="LunraGames.NoiseMaker.Node`1"/> class and creates an empty SourceIds array, caching the source count if it hasn't already been done.
		/// </summary>
		/// <remarks> Magic! </remarks>
		protected Node()
		{
			if (!CachedSourceCounts.ContainsKey(GetType()))
			{
				var count = 0;
				var fields = GetType().GetFields();
				foreach (var field in fields)
				{
					var attributes = field.GetCustomAttributes(typeof(NodeLinker), true);
					if (0 < attributes.Length) count++;
				}
				CachedSourceCounts.Add(GetType(), count);
				InitializeSources(count);
			}
			else InitializeSources(CachedSourceCounts[GetType()]);
		}

		/// <summary>
		/// Used to set the initial size of SourceIds and populate it with the default null values.
		/// </summary>
		/// <param name="count">Count.</param>
		void InitializeSources(int count = 0)
		{
			if (count < 0) throw new ArgumentOutOfRangeException("count", "A negative \"count\" is not allowed");
			SourceCount = count;
			SourceIds = SourceIds ?? new List<string>();

			if (SourceIds.Count < count)
			{
				var currIndex = Mathf.Max(0, SourceIds.Count - 1);
				while (currIndex < count)
				{
					SourceIds.Add(null);
					currIndex++;
				}
			}
		}

		public Type OutputType { get { return typeof(T); } }

		public abstract T GetValue(Graph graph);

		public object GetRawValue(Graph graph)
		{
			return GetValue(graph);
		}

		/// <summary>
		/// Returns a list of sources only if each one is actually defined.
		/// </summary>
		/// <param name="graph">The parent graph.</param>
		/// <param name="sources">Sources.</param>
		// todo: this seems very redundent...
		protected List<object> Values(Graph graph, params string[] sources)
		{
			if (graph == null) throw new ArgumentNullException("graph");
			if (graph.Nodes == null) throw new ArgumentException("graph.Nodes");

			var result = new List<object>();
			var ids = sources.Length == 0 ? SourceIds.ToArray() : sources;
			foreach (var source in ids)
			{
				if (StringExtensions.IsNullOrWhiteSpace(source)) throw new ArgumentNullException("sources", "Array \"sources\" can't contain a null or empty string");
				var node = graph.Nodes.FirstOrDefault(n => n.Id == source);
				if (node == null) throw new ArgumentOutOfRangeException("sources", "No node found for \""+sources+"\"");
				result.Add(node.GetRawValue(graph));
			}
			return result;
		}

		/// <summary>
		/// Returns a list of sources, or nulls if no sources are specified.
		/// </summary>
		/// <returns>The sources.</returns>
		/// <param name="graph">The parent graph.</param>
		/// <param name="sources">Sources.</param>
		protected List<object> NullableValues(Graph graph, params string[] sources)
		{
			if (graph == null) throw new ArgumentNullException("graph");
			if (graph.Nodes == null) throw new ArgumentException("graph.Nodes");

			var result = new List<object>();
			var ids = sources.Length == 0 ? SourceIds.ToArray() : sources;
			foreach (var source in ids)
			{
				if (StringExtensions.IsNullOrWhiteSpace(source)) result.Add(null);
				else
				{
					var node = graph.Nodes.FirstOrDefault(n => n.Id == source);
					if (node == null) throw new ArgumentOutOfRangeException("sources", "No node found for \""+sources+"\"");
					result.Add(node.GetRawValue(graph));
				}
			}
			return result;
		}

		/// <summary>
		/// Checks if the specified indices are null, and returns true if they are. If no indices are passed, all indices are checked.
		/// </summary>
		/// <returns><c>true</c>, if any of the specified indices are null, <c>false</c> otherwise.</returns>
		/// <param name="indices">Indices to check, don't specify any to check all indices.</param>
		protected bool AnyNullSources(params int[] indices)
		{
			if (SourceIds == null) throw new NullReferenceException("SourceIds");
			if (SourceIds.Count == 0)
			{
				if (indices.Length == 0) return false;
				else throw new ArgumentOutOfRangeException("indices", "Can't check indices for an empty SourceIds list");
			}

			if (indices.Length == 0)
			{
				for (var i = 0; i < SourceIds.Count; i++)
				{
					if (StringExtensions.IsNullOrWhiteSpace(SourceIds[i])) return true;
				}
			}
			else
			{
				for (var i = 0; i < indices.Length; i++)
				{
					var index = indices[i];
					if (SourceIds.Count <= index) throw new ArgumentOutOfRangeException("indices["+i+"] : "+index);
					if (StringExtensions.IsNullOrWhiteSpace(SourceIds[index])) return true;
				}
			}

			return false;
		}
		/// <summary>
		/// Checks if any sources between start (inclusive) and end (exclusive) are null or empty.
		/// </summary>
		/// <returns><c>true</c>, if any sources are null or empty, <c>false</c> otherwise.</returns>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		protected bool AnyNullSourcesInRange(int start, int end)
		{
			if (SourceIds == null) throw new NullReferenceException("SourceIds");
			if (end < start) throw new ArgumentOutOfRangeException("start", "Can't have a start value greater than the end value");
			if (start < 0) throw new ArgumentOutOfRangeException("start", "Can't start from a negative number");
			if (SourceIds.Count <= end) throw new ArgumentOutOfRangeException("end");

			for (var i = start; i < end; i++)
			{
				if (StringExtensions.IsNullOrWhiteSpace(SourceIds[i])) return true;
			}

			return false;
		}

		/// <summary>
		/// Checks if any sources between 0 (inclusive) and end (exclusive) are null or empty.
		/// </summary>
		/// <returns><c>true</c>, if any sources are null or empty, <c>false</c> otherwise.</returns>
		/// <param name="end">End.</param>
		protected bool AnyNullSourcesInRange(int end)
		{
			return AnyNullSourcesInRange(0, end);	
		}

		protected bool InvalidSourceCount { get { return SourceIds == null || SourceIds.Count != SourceCount; } }

		protected S GetLocalIfValueNull<S>(S localValue, int valueIndex,  Graph graph)
		{
			return GetLocalIfValueNull<S>(localValue, valueIndex, NullableValues(graph));
		}

		protected S GetLocalIfValueNull<S>(S localValue, int valueIndex,  List<object> values)
		{
			if (valueIndex < 0) throw new ArgumentOutOfRangeException("valueIndex", "Can't specify a negative valueIndex");
			if (values == null) throw new ArgumentNullException("values");
			if (values.Count <= valueIndex) throw new ArgumentOutOfRangeException("valueIndex", "Can't specify a valueIndex larger than sources.Count");

			var value = values[valueIndex];
			if (value == null) return localValue;
			if (!typeof(S).IsAssignableFrom(value.GetType())) throw new ArgumentException("Specified source is not the correct type", "values["+valueIndex+"]");
			return (S)value;
		}
	}
}