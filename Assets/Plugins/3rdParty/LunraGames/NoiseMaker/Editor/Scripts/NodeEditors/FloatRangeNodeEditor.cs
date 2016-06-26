using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(FloatRangeNode), Strings.Utility, "Float Range")]
	public class FloatRangeNodeEditor : NodeEditor 
	{
		public override INode Draw(Graph graph, INode node)
		{
			var rangeNode = DrawFields(graph, node, false) as FloatRangeNode;
			var currValue = rangeNode.GetValue(graph);

			if (rangeNode.UpperBound < rangeNode.LowerBound) EditorGUILayout.HelpBox("Upper bound cannot be lower than lower bound.", MessageType.Warning);

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Current Value");
				GUILayout.FlexibleSpace();
				EditorGUILayout.SelectableLabel(currValue.ToString(), GUI.skin.textField, GUILayout.Height(16f), GUILayout.Width(55f));
			}
			GUILayout.EndHorizontal();

			return rangeNode;
		}
	}
}