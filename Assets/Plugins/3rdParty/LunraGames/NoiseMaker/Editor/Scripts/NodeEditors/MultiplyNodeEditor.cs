using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(MultiplyNode), Strings.Combiners, "Multiply")]
	public class MultiplyNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var multiply = node as MultiplyNode;

			if (multiply.GetValue(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node as Node<IModule>);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyTwoInputs, MessageType.Warning);

			return multiply;
		}
	}
}