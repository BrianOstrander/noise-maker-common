using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(FloatNode), Strings.Properties, "Float")]
	public class FloatNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var floatNode = node as FloatNode;

			var preview = GetPreview<float>(graph, node);

			floatNode.FloatValue = Deltas.DetectDelta<float>(floatNode.FloatValue, EditorGUILayout.FloatField("Value", floatNode.FloatValue), ref preview.Stale);

//			if (exponent.GetValue(graph.Nodes) != null)
//			{
//				var preview = GetModulePreview(graph, node as Node<IModule>);
//				GUILayout.Box(preview.Preview);
//
//				GUILayout.FlexibleSpace();
//
//				exponent.Exponent = Deltas.DetectDelta<float>(exponent.Exponent, EditorGUILayout.FloatField("Exponent", exponent.Exponent), ref preview.Stale);
//			}
//			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return floatNode;
		}
	}
}