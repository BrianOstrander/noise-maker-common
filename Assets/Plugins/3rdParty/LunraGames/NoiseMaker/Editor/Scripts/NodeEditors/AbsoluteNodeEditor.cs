using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(AbsoluteNode), Strings.Modifiers, "Absolute")]
	public class AbsoluteNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var absolute = node as AbsoluteNode;

			if (absolute.GetValue(graph.Nodes) != null)
			{
				var preview = GetModulePreview(graph, node as Node<IModule>);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox("Specify an input.", MessageType.Warning);

			return absolute;
		}
	}
}