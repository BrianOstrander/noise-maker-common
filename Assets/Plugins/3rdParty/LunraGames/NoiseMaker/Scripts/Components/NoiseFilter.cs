using UnityEngine;
using System;
using LibNoise.Models;

namespace LunraGames.NoiseMaker
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class NoiseFilter : MonoBehaviour 
	{
		#region Inspector
		public bool GenerateOnAwake;
		public NoiseGraph NoiseGraph;
		public MercatorMap MercatorMap;
		public int MapWidth;
		public int MapHeight;
		public Vector3 Translation;
		public Vector3 Rotation;
		public Vector3 Scale = Vector3.one;
		public Filtering Filtering;
		#endregion

		Mesh CachedMesh;
		Material CachedMaterial;

		void Awake()
		{
			if (GenerateOnAwake) Regenerate();
		}

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

			if (root == null) throw new NullReferenceException("Couldn't find root IModule");

			var sphere = new Sphere(root);

			var mesh = Instantiate<Mesh>(CachedMesh);

			var verts = mesh.vertices;
			graph.GetSphereAltitudes(ref verts);
			mesh.vertices = verts;

			meshFilter.sharedMesh = mesh;

			var texture = new Texture2D(MapWidth, MapHeight);
			var colors = new Color[MapWidth * MapHeight];

			map.GetSphereColors(MapWidth, MapHeight, sphere, ref colors);
			texture.SetPixels(colors);
			texture.Apply();

			meshRenderer.sharedMaterial.mainTexture = texture;
		}
	}
}