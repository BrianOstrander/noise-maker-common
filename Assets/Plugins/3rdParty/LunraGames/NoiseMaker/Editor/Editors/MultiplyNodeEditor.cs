﻿using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(MultiplyNode), Strings.Combiners, "Multiply")]
	public class MultiplyNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var multiply = node as MultiplyNode;

			if (multiply.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox("Specify two inputs.", MessageType.Warning);

			GUILayout.FlexibleSpace();

			return multiply;
		}
	}
}