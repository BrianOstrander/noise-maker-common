﻿using UnityEngine;
using System;
using LibNoise.Models;
using LibNoise.Modifiers;

namespace LunraGames.NoiseMaker
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class NoiseFilter : MonoBehaviour 
	{
		public const float DefaultDatum = 0.5f;
		public const float DefaultDeviation = 0.1f;

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
		public float Datum = DefaultDatum;
		public float Deviation = DefaultDeviation;
		#endregion

		Mesh CachedMesh;

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

			var graph = NoiseGraph.GraphInstantiation;
			var map = MercatorMap.MercatorInstantiation;

			if (graph == null) throw new NullReferenceException("Couldn't instantiate the NoiseGraph");
			if (map == null) throw new NullReferenceException("Couldn't instantiate the MercatorMap");

			var root = graph.Root;

			if (root == null) throw new NullReferenceException("Couldn't find root IModule");

			if (Translation != Vector3.zero) root = new TranslateInput(root, Translation.x, Translation.y, Translation.z);
			if (Rotation != Vector3.zero) root = new RotateInput(root, Rotation.x, Rotation.y, Rotation.z);
			if (Scale != Vector3.one) root = new ScaleInput(root, Scale.x, Scale.y, Scale.z);

			var sphere = new Sphere(root);

			var mesh = Instantiate(CachedMesh);

			var verts = mesh.vertices;
			Graph.GetSphereAltitudes(sphere, ref verts, Datum, Deviation);
			mesh.vertices = verts;

			meshFilter.sharedMesh = mesh;

			var texture = new Texture2D(MapWidth, MapHeight);
			var colors = new Color[MapWidth * MapHeight];

			map.GetSphereColors(MapWidth, MapHeight, sphere, ref colors);
			texture.SetPixels(colors);
			texture.Apply();

			meshRenderer.material.mainTexture = texture;
		}
	}
}