using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;
using Atesh;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(RootNode), Strings.Hidden, "Root")]
	public class RootNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var rootNode = node as RootNode;

			if (rootNode.SourceIds != null && !StringExtensions.IsNullOrWhiteSpace(rootNode.SourceIds[0]))
			{
				var targetNode = graph.Nodes.Find(n => n.Id == rootNode.SourceIds[0]);
				var preview = GetPreview(graph, targetNode);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return rootNode;
		}
	}
}