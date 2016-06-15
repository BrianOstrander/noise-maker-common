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
		public override INode Draw(Graph graph, INode node)
		{
			var scalePoint = node as ScalePointNode;

			if (scalePoint.GetValue(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node as Node<IModule>);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				scalePoint.Scale = Deltas.DetectDelta<Vector3>(scalePoint.Scale, EditorGUILayout.Vector3Field("Scale", scalePoint.Scale), ref preview.Stale);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return scalePoint;
		}
	}
}