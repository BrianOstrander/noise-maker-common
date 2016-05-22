using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(DisplaceNode), Strings.Transformers, "Displace")]
	public class DisplaceNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var blend = node as DisplaceNode;

			if (blend.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox("Specify a source and an x, y, and z source.", MessageType.Warning);

			return blend;
		}
	}
}