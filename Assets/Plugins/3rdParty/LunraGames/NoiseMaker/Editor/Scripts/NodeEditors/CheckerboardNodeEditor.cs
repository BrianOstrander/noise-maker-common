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
		public override INode Draw(Graph graph, INode node)
		{
			var preview = GetPreview(graph, node as Node<IModule>);
			GUILayout.Box(preview.Preview);
			return node as CheckerboardNode;
		}
	}
}