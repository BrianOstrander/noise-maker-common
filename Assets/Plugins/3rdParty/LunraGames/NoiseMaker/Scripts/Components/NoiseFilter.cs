﻿using UnityEngine;
using System;

namespace LunraGames.NoiseMaker
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class NoiseFilter : MonoBehaviour 
	{
		#region Inspector
		public NoiseGraph NoiseGraph;
		public MercatorMap MercatorMap;
		public Vector3 Translation;
		public Vector3 Rotation;
		public Vector3 Scale;
		public Filtering Filtering;
		#endregion

		Mesh CachedMesh;
		Material CachedMaterial;

		public void Regenerate()
		{
			if (NoiseGraph == null) throw new NullReferenceException("A NoiseGraph must be specified");
			if (MercatorMap == null) throw new NullReferenceException("A MercatorMap must be specified");

			var meshFilter = GetComponent<MeshFilter>();
			var meshRenderer = GetComponent<MeshRenderer>();

			if (meshFilter == null) throw new NullReferenceException("A MeshFilter is required");
			if (meshRenderer == null) throw new NullReferenceException("A MeshRenderer is required");
			if (meshFilter.sharedMesh == null) throw new NullReferenceException("A sharedMesh must be specified for this gameobject's MeshFilter");
			if (meshRenderer.sharedMaterial == null) throw new NullReferenceException("A sharedMaterial must be specified for this gameobject's MeshRenderer");

			if (CachedMesh == null) CachedMesh = meshFilter.sharedMesh;
			if (CachedMaterial == null) CachedMaterial = meshRenderer.sharedMaterial;

			var graph = NoiseGraph.GraphInstantiation;
			var map = MercatorMap.MercatorInstantiation;

			if (graph == null) throw new NullReferenceException("Couldn't instantiate the NoiseGraph");
			if (map == null) throw new NullReferenceException("Couldn't instantiate the MercatorMap");

			var root = graph.Root;

		}
	}
}