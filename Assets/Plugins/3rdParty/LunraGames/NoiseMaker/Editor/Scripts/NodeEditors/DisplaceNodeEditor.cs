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
		public override INode Draw(Graph graph, INode node)
		{
			var blend = node as DisplaceNode;

			if (blend.GetValue(graph.Nodes) != null)
			{
				var preview = GetModulePreview(graph, node as Node<IModule>);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox("Specify a source and an x, y, and z source.", MessageType.Warning);

			return blend;
		}
	}
}