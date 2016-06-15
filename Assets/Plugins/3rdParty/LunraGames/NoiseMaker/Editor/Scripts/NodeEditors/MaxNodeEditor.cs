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
		public override INode Draw(Graph graph, INode node)
		{
			var max = node as MaxNode;

			if (max.GetValue(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node as Node<IModule>);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyTwoInputs, MessageType.Warning);

			return max;
		}
	}
}