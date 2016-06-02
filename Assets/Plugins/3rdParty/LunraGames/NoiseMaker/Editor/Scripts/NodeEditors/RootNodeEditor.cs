﻿using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEditor;
using LibNoise;
using Atesh;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(RootNode), Strings.Hidden, "Root")]
	public class RootNodeEditor : NodeEditor
	{
		static string LastRootSource;

		public override Node Draw(Graph graph, Node node)
		{
			var rootNode = node as RootNode;

			if (rootNode.GetModule(graph.Nodes) != null)// rootNode.SourceIds != null && !StringExtensions.IsNullOrWhiteSpace(rootNode.SourceIds.FirstOrDefault()))
			{
				//var targetNode = graph.Nodes.Find(n => n.Id == rootNode.SourceIds[0]);
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);

				var source = rootNode.SourceIds.FirstOrDefault();

				preview.Stale = preview.Stale || LastRootSource != source;

				LastRootSource = source;
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return rootNode;
		}
	}
}