using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(SelectNode), Strings.Selectors, "Select")]
	public class SelectNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var selector = node as SelectNode;

			if (selector.GetValue(graph) != null)
			{
				var preview = GetPreview<IModule>(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				selector.EdgeFalloff = Deltas.DetectDelta<float>(selector.EdgeFalloff, EditorGUILayout.FloatField("Edge Falloff", selector.EdgeFalloff), ref preview.Stale);

				selector.LowerBound = Deltas.DetectDelta<float>(selector.LowerBound, Mathf.Min(EditorGUILayout.FloatField("Lower Bound", selector.LowerBound), selector.UpperBound - 0.001f), ref preview.Stale);
				selector.UpperBound = Deltas.DetectDelta<float>(selector.UpperBound, Mathf.Max(EditorGUILayout.FloatField("Upper Bound", selector.UpperBound), selector.LowerBound + 0.001f), ref preview.Stale);
			}
			else EditorGUILayout.HelpBox("Specify two sources and a control.", MessageType.Warning);

			return selector;
		}
	}
}