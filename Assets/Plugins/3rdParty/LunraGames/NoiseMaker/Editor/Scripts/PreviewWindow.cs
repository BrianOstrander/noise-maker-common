﻿using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LibNoise.Models;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	public class PreviewWindow : EditorWindow
	{
		[SerializeField]
		MercatorMap MercatorMap;
		[SerializeField]
		NoiseGraph NoiseGraph;

		Graph Graph;
		long PreviewLastUpdated;
		Texture2D PreviewTexture;
		Mesh PreviewMesh;
		Editor PreviewObjectEditor;
		bool Updating;

		[MenuItem ("Window/Lunra Games/Noise Maker/Preview")]
		static void Init () 
		{
			var window = GetWindow(typeof (PreviewWindow), false, "Noise Maker Preview") as PreviewWindow;
			window.titleContent = new GUIContent("Preview", NoiseMakerConfig.Instance.PreviewTab);
			window.minSize = new Vector2(650f, 650f);
			window.Show();
		}

		#region Messages
		void OnGUI()
		{
			var splashImage = NoiseMakerConfig.Instance.SplashMini;
			GUI.Box (new Rect (0f, position.height - splashImage.height, splashImage.width, splashImage.height), splashImage, GUIStyle.none);
				
			var overridePreview = false;

			var noise = EditorGUILayout.ObjectField("Noise", NoiseGraph, typeof(NoiseGraph), false);
			var wasNoise = NoiseGraph;
			NoiseGraph = noise == null ? null : noise as NoiseGraph;

			var map = EditorGUILayout.ObjectField("Mercator", MercatorMap, typeof(MercatorMap), false);
			var wasMap = MercatorMap;
			MercatorMap = map == null ? null : map as MercatorMap;

			if (NoiseGraph == null)
			{
				var selections = Selection.assetGUIDs;
				if (selections == null || selections.Length == 0)
				{
					DrawCentered("Select a Noise Maker\nFile to Preview");
					return;
				}
				else if (1 < selections.Length)
				{
					DrawCentered("Select Only a Single\nNoise Maker File to Preview");
					return;
				}

				var selection = Selection.activeObject as NoiseGraph;

				if (selection == null)
				{
					DrawCentered("Selection is an Invalid\nNoise Maker File");
					return;
				}
				else NoiseGraph = selection;
			}

			overridePreview = overridePreview || wasNoise != NoiseGraph || wasMap != MercatorMap;

			if (NoiseGraph == null)
			{
				DrawCentered("Select or specify a\nNoise Maker File to Preview");
				return;
			}

			if (overridePreview || Graph == null) Graph = NoiseGraph.GraphInstantiation;

			if (Graph == null)
			{
				DrawCentered("Something Went Wrong\nWhile Deserializing Graph");
				return;
			}

			if (Graph.RootNode == null)
			{
				DrawCentered("No Root Node Found");
				return;
			}

			DrawElevationPreview(Graph.RootNode, new Rect(0f, 48f, position.width, position.height - 32f), overridePreview);
		}

		void OnSelectionChange() { Repaint(); }
		#endregion

		#region Previews
		void DrawElevationPreview(Node<IModule> node, Rect area, bool overridePreview)
		{
			var lastUpdate = overridePreview ? DateTime.Now.Ticks : NodeEditor.LastUpdated(node.Id);
			if (lastUpdate != PreviewLastUpdated) 
			{
				NoiseMakerConfig.Instance.Ico5Vertex.GetComponent<MeshFilter>().sharedMesh = NoiseMakerConfig.Instance.Ico5VertexMesh;

				if (PreviewMesh == null) PreviewMesh = (Mesh)Instantiate(NoiseMakerConfig.Instance.Ico5VertexMesh);

				var module = node.GetValue(Graph);
				var sphere = new Sphere(module);

				var verts = PreviewMesh.vertices;
				Graph.GetSphereAltitudes(sphere, ref verts, 0.75f);
				PreviewMesh.vertices = verts;

				Updating = true;
				PreviewTexture = NoiseMakerWindow.GetSphereTexture(module, 512, MercatorMap == null ? null : MercatorMap.MercatorInstantiation, PreviewTexture, () => Updating = false);

				PreviewLastUpdated = lastUpdate;

				Repaint();
			}
			var filter = NoiseMakerConfig.Instance.Ico5Vertex.GetComponent<MeshFilter>();

			if (filter.sharedMesh != PreviewMesh)
			{
				filter.sharedMesh = PreviewMesh;
				Repaint();
			}

			var mat = NoiseMakerConfig.Instance.Ico5Vertex.GetComponent<MeshRenderer>().sharedMaterial;

			if (mat.mainTexture != PreviewTexture)
			{
				mat.mainTexture = PreviewTexture;
				Repaint();
			}

			if (PreviewObjectEditor == null) PreviewObjectEditor = Editor.CreateEditor(NoiseMakerConfig.Instance.Ico5Vertex);

			var previewArea = new Rect(area.x, area.y, area.width, area.height);
			PreviewObjectEditor.OnPreviewGUI(previewArea, Styles.OptionBox);

			if (Updating) 
			{
				Pinwheel.Draw(previewArea.center);
				Repaint();
			}
		}
		#endregion

		void DrawCentered(string text)
		{
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				GUILayout.Label(text, Styles.NoPreviewLabel);
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
		}
	}
}