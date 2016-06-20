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
	[NodeDrawer(typeof(CurveNode), Strings.Modifiers, "Curve")]
	public class CurveNodeEditor : NodeEditor
	{
		static Dictionary<string, Vector2> ScrollPositions = new Dictionary<string, Vector2>();

		public override INode Draw(Graph graph, INode node)
		{
			var curve = node as CurveNode;

			if (curve.GetValue(graph.Nodes) != null)
			{
				var preview = GetPreview<IModule>(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				var lastPoints = new List<Vector2>(curve.Points);

				var scrollPos = Vector2.zero;
				if (ScrollPositions.ContainsKey(curve.Id)) scrollPos = ScrollPositions[curve.Id];
				else ScrollPositions.Add(curve.Id, scrollPos);

				if (GUILayout.Button("Add")) curve.Points.Add(Vector2.zero);

				scrollPos = GUILayout.BeginScrollView(new Vector2(0f, scrollPos.y), false, false, GUILayout.Height(96f));
				{
					var showDelete = 4 < curve.Points.Count;
					int? deletedIndex = null;
					for (var i = 0; i < curve.Points.Count; i++)
					{
						var unmodifiedI = i;
						GUILayout.BeginHorizontal();
						var x = EditorGUILayout.FloatField(curve.Points[unmodifiedI].x);
						var y = EditorGUILayout.FloatField(curve.Points[unmodifiedI].y);
						curve.Points[i] = new Vector2(x, y);
						if (showDelete && GUILayout.Button("X")) deletedIndex = unmodifiedI;
						GUILayout.EndHorizontal();
					}

					if (deletedIndex.HasValue) curve.Points.RemoveAt(deletedIndex.Value);

					GUILayout.FlexibleSpace();
				}
				GUILayout.EndScrollView();

				ScrollPositions[curve.Id] = scrollPos;

				preview.Stale = preview.Stale || lastPoints.Count != curve.Points.Count;

				if (!preview.Stale)
				{
					for (var i = 0; i < lastPoints.Count; i++)
					{
						preview.Stale = preview.Stale || lastPoints[i].x != curve.Points[i].x || lastPoints[i].y != curve.Points[i].y;
					}
				}
			}
			else 
			{
				EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);
				GUILayout.FlexibleSpace();
			}

			return curve;
		}

	}
}