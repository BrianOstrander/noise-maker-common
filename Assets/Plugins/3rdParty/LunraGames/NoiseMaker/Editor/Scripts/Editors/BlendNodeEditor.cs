using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(BlendNode), Strings.Selectors, "Blend")]
	public class BlendNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var blend = node as BlendNode;

			if (blend.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox("Specify two sources and a weight.", MessageType.Warning);

			return blend;
		}
	}
}