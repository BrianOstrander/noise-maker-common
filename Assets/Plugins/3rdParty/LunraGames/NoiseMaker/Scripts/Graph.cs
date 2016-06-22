using System;
using System.Collections.Generic;
using LibNoise;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;
using LibNoise.Models;
using Random = System.Random;

namespace LunraGames.NoiseMaker
{
	public class Graph
	{
		const float DefaultDatum = 0.5f;
		const float DefaultDeviation = 0.1f;

		public List<INode> Nodes = new List<INode>();

		public string RootId;

		public int Seed;

		Random _Random;

		public Random Random { get { return _Random ?? (_Random = new Random(Seed)); } }

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
					_Root = node.GetRawValue(Nodes) as IModule;
				}
				return _Root;
			}
		}

		RootNode _RootNode;

		[JsonIgnore]
		public RootNode RootNode
		{
			get
			{
				if (StringExtensions.IsNullOrWhiteSpace(RootId)) throw new NullReferenceException("No RootId has been set");
				if (_RootNode == null || _RootNode.Id != RootId)
				{
					var node = Nodes.FirstOrDefault(n => n.Id == RootId);
					if (node == null) throw new NullReferenceException("No node found for the RootId \""+RootId+"\"");
					_RootNode = node as RootNode;
				}
				return _RootNode;
			}
		}

		public void Apply(params Property[] properties)
		{
			foreach (var property in properties)
			{
				if (property == null)
				{
					Debug.LogWarning("Can't provide null properties, skipping");
					continue;
				}
				else if (property.Value == null)
				{
					Debug.LogWarning("Can't provide properties with null values, skipping");
					continue;
				}

				var node = Nodes.FirstOrDefault(n => n.Id == property.Id);

				if (node == null)
				{
					Debug.LogWarning("No node found for property \""+property.Name+"\", skipping");
					continue;
				}
				else if (!typeof(IPropertyNode).IsAssignableFrom(node.GetType()))
				{
					Debug.LogWarning("Node for property \""+property.Name+"\" is not a node that impliments IPropertyNode, skipping");
					continue;
				}

				var typedNode = node as IPropertyNode;

				typedNode.RawPropertyValue = property.Value;
			}
		}

		public void Remove(INode node)
		{
			if (node == null) throw new ArgumentNullException("node");

			foreach (var curr in Nodes)
			{
				if (curr.SourceIds == null) continue;
				for (var i = 0; i < curr.SourceIds.Count; i++)
				{
					if (curr.SourceIds[i] == node.Id) curr.SourceIds[i] = null;
				}
			}
			Nodes.Remove(node);
		}

		/// <summary>
		/// Populates an array with values derived from a spherical model of this graph.
		/// </summary>
		/// <param name="vertices">Vertices array to update.</param>
		/// <param name="datum">Datum is similar to a "sea level" that all values are relative to.</param>
		/// <param name="deviation">Deviation is the scalar of the datam to multiply the values by.</param>
		public void GetSphereAltitudes(ref Vector3[] vertices, float datum = DefaultDatum, float deviation = DefaultDeviation)
		{
			var root = Root;

			if (root == null) throw new ArgumentNullException("Couldn't find root IModule");

			var sphere = root is Sphere ? root as Sphere : new Sphere(root);

			GetSphereAltitudes(sphere, ref vertices, datum, deviation);
		}

		/// <summary>
		/// Gets the sphere altitudes.
		/// </summary>
		/// <param name="sphere">Sphere module to get values from.</param>
		/// <param name="vertices">Vertices array to update.</param>
		/// <param name="datum">Datum is similar to a "sea level" that all values are relative to.</param>
		/// <param name="deviation">Deviation is the scalar of the datam to multiply the values by.</param>
		public static void GetSphereAltitudes(Sphere sphere, ref Vector3[] vertices, float datum = DefaultDatum, float deviation = DefaultDeviation)
		{
			if (sphere == null) throw new ArgumentNullException("sphere");

			for (var i = 0; i < vertices.Length; i++)
			{
				// Get the value of the specified vert, by converting it's euler position to a latitude and longitude.
				var vert = vertices[i];
				var latLong = SphereUtils.CartesianToPolar(vert.normalized);
				vertices[i] = (vert.normalized * datum) + (vert.normalized * (float)sphere.GetValue(latLong.x, latLong.y) * (datum * deviation));
			}
		}
	}
}