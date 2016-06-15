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
		public override INode Draw(Graph graph, INode node)
		{
			var min = node as MinNode;

			if (min.GetValue(graph.Nodes) != null)
			{
				var preview = GetModulePreview(graph, node as Node<IModule>);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyTwoInputs, MessageType.Warning);

			return min;
		}
	}
}