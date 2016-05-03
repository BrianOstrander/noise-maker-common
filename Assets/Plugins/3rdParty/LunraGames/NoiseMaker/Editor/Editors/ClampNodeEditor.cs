using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(ClampNode), Strings.Modifiers, "Clamp")]
	public class ClampNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var clamp = node as ClampNode;

			if (clamp.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				clamp.LowerBound = Deltas.DetectDelta<float>(clamp.LowerBound, Mathf.Min(EditorGUILayout.FloatField("Lower Bound", clamp.LowerBound), clamp.UpperBound - 0.00001f), ref preview.Stale);
				clamp.UpperBound = Deltas.DetectDelta<float>(clamp.UpperBound, Mathf.Max(EditorGUILayout.FloatField("Upper Bound", clamp.UpperBound), clamp.LowerBound + 0.00001f), ref preview.Stale);
			}
			else 
			{
				EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);
				GUILayout.FlexibleSpace();
			}

			return clamp;
		}
	}
}