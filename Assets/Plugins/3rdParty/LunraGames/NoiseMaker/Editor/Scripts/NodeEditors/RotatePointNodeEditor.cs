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
		public override INode Draw(Graph graph, INode node)
		{
			var rotatePoint = node as RotatePointNode;

			if (rotatePoint.GetValue(graph.Nodes) != null)
			{
				var preview = GetPreview<IModule>(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				rotatePoint.Rotation = Deltas.DetectDelta<Vector3>(rotatePoint.Rotation, EditorGUILayout.Vector3Field("Rotation", rotatePoint.Rotation), ref preview.Stale);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return rotatePoint;
		}
	}
}