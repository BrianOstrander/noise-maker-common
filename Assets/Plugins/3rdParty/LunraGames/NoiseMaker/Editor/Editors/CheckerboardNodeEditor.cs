using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(CheckerboardNode), Strings.Generators, "Checkerboard")]
	public class CheckerboardNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var preview = GetPreview(graph, node);
			GUILayout.Box(preview.Preview);
			return node as CheckerboardNode;
		}
	}
}