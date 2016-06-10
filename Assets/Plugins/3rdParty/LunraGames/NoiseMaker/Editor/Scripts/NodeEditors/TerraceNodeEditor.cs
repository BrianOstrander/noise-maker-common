using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEditor;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;


namespace LunraGames.NoiseMaker
{
	// I've hidden this because the LibNoise side of things keeps breaking when using multithreading.
	[NodeDrawer(typeof(TerraceNode), Strings.Hidden, "Terrace")]
	public class TerraceNodeEditor : NodeEditor
	{
		static Dictionary<string, Vector2> ScrollPositions = new Dictionary<string, Vector2>();

		public override Node Draw(Graph graph, Node node)
		{
			var terrace = node as TerraceNode;

			if (terrace.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				var lastPoints = new List<float>(terrace.Points);

				var scrollPos = Vector2.zero;
				if (ScrollPositions.ContainsKey(terrace.Id)) scrollPos = ScrollPositions[terrace.Id];
				else ScrollPositions.Add(terrace.Id, scrollPos);

				if (GUILayout.Button("Add")) terrace.Points.Add(0f);

				scrollPos = GUILayout.BeginScrollView(new Vector2(0f, scrollPos.y), false, false, GUILayout.Height(96f));
				{
					var showDelete = 2 < terrace.Points.Count;
					int? deletedIndex = null;
					for (var i = 0; i < terrace.Points.Count; i++)
					{
						var unmodifiedI = i;
						GUILayout.BeginHorizontal();
						terrace.Points[i] = EditorGUILayout.FloatField(terrace.Points[unmodifiedI]);
						if (showDelete && GUILayout.Button("X")) deletedIndex = unmodifiedI;
						GUILayout.EndHorizontal();
					}

					if (deletedIndex.HasValue) terrace.Points.RemoveAt(deletedIndex.Value);

					GUILayout.FlexibleSpace();
				}
				GUILayout.EndScrollView();

				ScrollPositions[terrace.Id] = scrollPos;

				preview.Stale = preview.Stale || lastPoints.Count != terrace.Points.Count;

				if (!preview.Stale)
				{
					for (var i = 0; i < lastPoints.Count; i++)
					{
						preview.Stale = preview.Stale || lastPoints[i] != terrace.Points[i];
					}
				}
			}
			else 
			{
				EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);
				GUILayout.FlexibleSpace();
			}

			return terrace;
		}

	}
}