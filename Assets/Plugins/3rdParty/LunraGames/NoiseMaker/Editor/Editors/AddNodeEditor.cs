using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(AddNode), Strings.Combiners, "Add")]
	public class AddNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var addNode = node as AddNode;

			if (addNode.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyTwoInputs, MessageType.Warning);

			return addNode;
		}
	}
}