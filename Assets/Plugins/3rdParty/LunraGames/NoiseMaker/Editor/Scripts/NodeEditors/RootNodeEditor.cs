using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEditor;
using LibNoise;
using Atesh;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(RootNode), Strings.Hidden, "Root", Strings.SpecifyAnInput)]
	public class RootNodeEditor : NodeEditor 
	{
		public override INode Draw(Graph graph, INode node)
		{
			var rootNode = DrawFields(graph, node) as RootNode;

			var preview = GetPreview<float>(graph, node);

			rootNode.Seed = Deltas.DetectDelta<int>(rootNode.Seed, EditorGUILayout.IntField("Seed", rootNode.Seed), ref preview.Stale);

			return rootNode;
		}
	}
}