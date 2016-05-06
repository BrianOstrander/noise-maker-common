using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(MinNode), Strings.Combiners, "Min")]
	public class MinNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var min = node as MinNode;

			if (min.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyTwoInputs, MessageType.Warning);

			return min;
		}
	}
}