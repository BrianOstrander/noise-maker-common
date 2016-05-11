using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(RootNode), Strings.Hidden, "Root")]
	public class RootNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var addNode = node as RootNode;

			if (addNode.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return addNode;
		}
	}
}