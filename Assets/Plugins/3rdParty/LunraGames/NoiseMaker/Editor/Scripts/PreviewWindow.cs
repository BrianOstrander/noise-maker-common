﻿using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LibNoise.Models;

namespace LunraGames.NoiseMaker
{
	public class PreviewWindow : EditorWindow
	{
		string LastGraph;
		Graph Graph;

		long PreviewLastUpdated;
		Texture2D PreviewTexture;
		Mesh PreviewMesh;
		Editor PreviewObjectEditor;

		[MenuItem ("Window/Lunra Games/Noise Maker - Preview")]
		static void Init () 
		{
			var window = EditorWindow.GetWindow(typeof (PreviewWindow), false, "Noise Maker Preview") as PreviewWindow;
			window.titleContent = new GUIContent("Preview", NoiseMakerConfig.Instance.PreviewTab);
			window.minSize = new Vector2(650f, 650f);
			window.Show();
		}

		#region Messages
		void OnGUI()
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

			var overridePreview = false;

			if (LastGraph != selection.GraphJson) 
			{
				Graph = selection.GraphInstantiation;
				LastGraph = selection.GraphJson;
				overridePreview = true;
			}

			if (Graph == null)
			{
				DrawCentered("Something Went Wrong\nWhile Deserializing Graph");
				return;
			}

			var root = Graph.Nodes.FirstOrDefault(n => n.Id == Graph.RootId);

			if (root == null)
			{
				DrawCentered("No Root Node Found");
				return;
			}

			DrawElevationPreview(root, position, overridePreview);
		}

		void OnSelectionChange() { Repaint(); }
		#endregion

		#region Previews
		void DrawElevationPreview(Node node, Rect area, bool overridePreview)
		{
			var lastUpdate = overridePreview ? DateTime.Now.Ticks : NodeEditor.LastUpdated(node.Id);
			if (lastUpdate != PreviewLastUpdated) 
			{
				NoiseMakerConfig.Instance.Ico5Vertex.GetComponent<MeshFilter>().sharedMesh = NoiseMakerConfig.Instance.Ico5VertexMesh;

				if (PreviewMesh == null) PreviewMesh = (Mesh)Instantiate(NoiseMakerConfig.Instance.Ico5VertexMesh);

				var module = node.GetModule(Graph.Nodes);
				var sphere = new Sphere(module);

				var verts = PreviewMesh.vertices;
				var newVerts = new Vector3[verts.Length];
				for (var i = 0; i < verts.Length; i++)
				{
					var vert = verts[i];
					var latLong = SphereUtils.CartesianToPolar(vert.normalized);
					newVerts[i] = (vert.normalized * NoiseMakerWindow.SphereScalar) + (vert.normalized * (float)sphere.GetValue(latLong.x, latLong.y) * 0.1f);
				}
				PreviewMesh.vertices = newVerts;

				PreviewTexture = NoiseMakerWindow.GetSphereTexture(module);

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
			mat.mainTextureOffset = new Vector2(0.5f, 0f);

			if (mat.mainTexture != PreviewTexture)
			{
				mat.mainTexture = PreviewTexture;
				Repaint();
			}

			if (PreviewObjectEditor == null) PreviewObjectEditor = Editor.CreateEditor(NoiseMakerConfig.Instance.Ico5Vertex);

			PreviewObjectEditor.OnPreviewGUI(new Rect(1f, 0f, area.width - 1f, area.height), Styles.OptionBox);
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