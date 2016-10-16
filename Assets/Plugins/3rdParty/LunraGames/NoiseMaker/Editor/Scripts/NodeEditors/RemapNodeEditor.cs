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
	[NodeDrawer(typeof(RemapNode), Strings.Modifiers, "Remap")]
	public class RemapNodeEditor : NodeEditor
	{
		static Dictionary<string, Vector2> ScrollPositions = new Dictionary<string, Vector2>();

		public override INode Draw(Graph graph, INode node)
		{
			var remap = node as RemapNode;

			if (remap.GetValue(graph) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				var lastPoints = new List<Vector2>(remap.Points);

				var scrollPos = Vector2.zero;
				if (ScrollPositions.ContainsKey(remap.Id)) scrollPos = ScrollPositions[remap.Id];
				else ScrollPositions.Add(remap.Id, scrollPos);

				if (GUILayout.Button("Add")) remap.Points.Add(Vector2.zero);

				scrollPos = GUILayout.BeginScrollView(new Vector2(0f, scrollPos.y), false, false, GUILayout.Height(96f));
				{
					var showDelete = 4 < remap.Points.Count;
					int? deletedIndex = null;
					for (var i = 0; i < remap.Points.Count; i++)
					{
						var unmodifiedI = i;
						GUILayout.BeginHorizontal();
						var x = EditorGUILayout.FloatField(remap.Points[unmodifiedI].x);
						var y = EditorGUILayout.FloatField(remap.Points[unmodifiedI].y);
						remap.Points[i] = new Vector2(x, y);
						if (showDelete && GUILayout.Button("X")) deletedIndex = unmodifiedI;
						GUILayout.EndHorizontal();
					}

					if (deletedIndex.HasValue) remap.Points.RemoveAt(deletedIndex.Value);

					GUILayout.FlexibleSpace();
				}
				GUILayout.EndScrollView();

				ScrollPositions[remap.Id] = scrollPos;

				preview.Stale = preview.Stale || lastPoints.Count != remap.Points.Count;

				if (!preview.Stale)
				{
					for (var i = 0; i < lastPoints.Count; i++)
					{
						preview.Stale = preview.Stale || lastPoints[i].x != remap.Points[i].x || lastPoints[i].y != remap.Points[i].y;
					}
				}
			}
			else 
			{
				EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);
				GUILayout.FlexibleSpace();
			}

			return remap;
		}

	}
}