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
		public override INode Draw(Graph graph, INode node)
		{
			var clamp = node as ClampNode;

			if (clamp.GetValue(graph.Nodes) != null)
			{
				var preview = GetPreview<IModule>(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				clamp.LowerBound = Deltas.DetectDelta<float>(clamp.LowerBound, Mathf.Min(EditorGUILayout.FloatField("Lower Bound", clamp.LowerBound), clamp.UpperBound - 0.001f), ref preview.Stale);
				clamp.UpperBound = Deltas.DetectDelta<float>(clamp.UpperBound, Mathf.Max(EditorGUILayout.FloatField("Upper Bound", clamp.UpperBound), clamp.LowerBound + 0.001f), ref preview.Stale);
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