using System;
using System.Collections.Generic;
using LibNoise;
using Atesh;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;
using LibNoise.Models;

namespace LunraGames.NoiseMaker
{
	public class Graph
	{
		const float DefaultDatum = 0.5f;
		const float DefaultDeviation = 0.1f;

		public List<INode> Nodes = new List<INode>();
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
					_Root = node.GetRawValue(Nodes) as IModule;
				}
				return _Root;
			}
		}

		INode _RootNode;

		[JsonIgnore]
		public INode RootNode
		{
			get
			{
				if (StringExtensions.IsNullOrWhiteSpace(RootId)) throw new NullReferenceException("No RootId has been set");
				if (_RootNode == null || _RootNode.Id != RootId)
				{
					var node = Nodes.FirstOrDefault(n => n.Id == RootId);
					if (node == null) throw new NullReferenceException("No node found for the RootId \""+RootId+"\"");
					_RootNode = node;
				}
				return _RootNode;
			}
		}

		public void Remove(INode node)
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