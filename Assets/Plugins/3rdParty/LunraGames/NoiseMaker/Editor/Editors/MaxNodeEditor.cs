using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(MaxNode), Strings.Combiners, "Max")]
	public class MaxNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var max = node as MaxNode;

			if (max.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyTwoInputs, MessageType.Warning);

			return max;
		}
	}
}