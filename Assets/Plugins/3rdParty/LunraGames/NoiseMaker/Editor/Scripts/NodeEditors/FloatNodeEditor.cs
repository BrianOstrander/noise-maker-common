﻿using UnityEditor;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(FloatNode), Strings.Properties, "Float")]
	public class FloatNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var floatNode = node as FloatNode;

			var preview = GetPreview(graph, node);

			floatNode.PropertyValue = Deltas.DetectDelta(floatNode.PropertyValue, EditorGUILayout.FloatField("Value", floatNode.PropertyValue), ref preview.Stale);

			return floatNode;
		}
	}
}