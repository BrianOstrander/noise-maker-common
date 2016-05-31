using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(RotatePointNode), Strings.Transformers, "Rotate Point")]
	public class RotatePointNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var rotatePoint = node as RotatePointNode;

			if (rotatePoint.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				rotatePoint.Rotation = Deltas.DetectDelta<Vector3>(rotatePoint.Rotation, EditorGUILayout.Vector3Field("Rotation", rotatePoint.Rotation), ref preview.Stale);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return rotatePoint;
		}
	}
}