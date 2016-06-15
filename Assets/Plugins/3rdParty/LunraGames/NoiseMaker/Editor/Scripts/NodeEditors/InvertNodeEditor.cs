using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(InvertNode), Strings.Modifiers, "Invert")]
	public class InvertNodeEditer : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var invert = node as InvertNode;

			if (invert.GetValue(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node as Node<IModule>);
				GUILayout.Box(preview.Preview);
			}
			else 
			{
				EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);
				GUILayout.FlexibleSpace();
			}

			return invert;
		}
	}
}