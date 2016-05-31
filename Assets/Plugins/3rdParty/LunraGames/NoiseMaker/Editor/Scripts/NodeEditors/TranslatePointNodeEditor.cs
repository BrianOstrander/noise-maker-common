using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(TranslatePointNode), Strings.Transformers, "Translate Point")]
	public class TranslatePointNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var translatePoint = node as TranslatePointNode;

			if (translatePoint.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				translatePoint.Position = Deltas.DetectDelta<Vector3>(translatePoint.Position, EditorGUILayout.Vector3Field("Position", translatePoint.Position), ref preview.Stale);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return translatePoint;
		}
	}
}