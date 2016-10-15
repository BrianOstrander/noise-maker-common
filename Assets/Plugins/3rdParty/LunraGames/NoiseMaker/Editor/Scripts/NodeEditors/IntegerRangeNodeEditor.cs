using UnityEngine;
using System;
using UnityEditor;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(IntegerRangeNode), Strings.Utility, "Integer Range")]
	public class IntegerRangeNodeEditor : NodeEditor 
	{
		public override INode Draw(Graph graph, INode node)
		{
			var rangeNode = node as IntegerRangeNode;

			if (rangeNode.UpperBound < rangeNode.LowerBound) EditorGUILayout.HelpBox("Upper bound cannot be less than lower bound.", MessageType.Warning);

			rangeNode = DrawFields(graph, rangeNode, false) as IntegerRangeNode;
			var currValue = rangeNode.GetValue(graph);

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