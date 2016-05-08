using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(ScalePointNode), Strings.Transformers, "Scale Point")]
	public class ScalePointNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var scalePoint = node as ScalePointNode;

			if (scalePoint.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				scalePoint.Scale = Deltas.DetectDelta<Vector3>(scalePoint.Scale, EditorGUILayout.Vector3Field("Scale", scalePoint.Scale), ref preview.Stale);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return scalePoint;
		}
	}
}