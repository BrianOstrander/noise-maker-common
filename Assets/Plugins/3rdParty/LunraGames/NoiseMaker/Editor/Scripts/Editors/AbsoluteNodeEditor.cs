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
		public override Node Draw(Graph graph, Node node)
		{
			var absolute = node as AbsoluteNode;

			if (absolute.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox("Specify an input.", MessageType.Warning);

			return absolute;
		}
	}
}