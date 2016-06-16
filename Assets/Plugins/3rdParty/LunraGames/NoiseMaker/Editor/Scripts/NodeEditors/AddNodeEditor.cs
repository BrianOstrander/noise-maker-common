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
		public override INode Draw(Graph graph, INode node)
		{
			var addNode = node as AddNode;

			if (addNode.GetValue(graph.Nodes) != null)
			{
				var preview = GetPreview<IModule>(graph, node);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyTwoInputs, MessageType.Warning);

			return addNode;
		}
	}
}