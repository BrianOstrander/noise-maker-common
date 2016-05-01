using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(ConstantNode), Strings.Generators, "Constant")]
	public class ConstantNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var constant = node as ConstantNode;

			var preview = GetPreview(graph, node);

			GUILayout.Box(preview.Preview, GUILayout.MaxWidth(PreviewSize), GUILayout.ExpandWidth(true));

			GUILayout.FlexibleSpace();

			constant.Value = Deltas.DetectDelta<float>(constant.Value, EditorGUILayout.FloatField("Value", constant.Value), ref preview.Stale);

			return constant;
		}
	}
}