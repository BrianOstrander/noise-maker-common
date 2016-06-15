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
		public override INode Draw(Graph graph, INode node)
		{
			var translatePoint = node as TranslatePointNode;

			if (translatePoint.GetValue(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node as Node<IModule>);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				translatePoint.Position = Deltas.DetectDelta<Vector3>(translatePoint.Position, EditorGUILayout.Vector3Field("Position", translatePoint.Position), ref preview.Stale);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return translatePoint;
		}
	}
}