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
		public override Node Draw(Graph graph, Node node)
		{
			var curve = node as CurveNode;

			if (curve.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				var frames = new List<Keyframe>();
				foreach (var point in curve.Points) frames.Add(new Keyframe((float)point.x, (float)point.y));
				var animation = new AnimationCurve(frames.ToArray());

				animation = EditorGUILayout.CurveField("Control Points", animation);

				var lastPoints = new List<Vector2>(curve.Points);


				curve.Points = new List<Vector2>();
				foreach (var frame in animation.keys) curve.Points.Add(new Vector2(frame.time, frame.value));

				if (3 < curve.Points.Count)
				{
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
					curve.Points = lastPoints;
					preview.Stale = true;
					UnityEditor.EditorUtility.DisplayDialog("Invalid", "Curve nodes require four or more control points.", "Okay");
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