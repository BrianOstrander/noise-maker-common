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
		public override Node Draw(Graph graph, Node node)
		{
			var invert = node as InvertNode;

			if (invert.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
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