using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(BillowNode), Strings.Generators, "Billow")]
	public class BillowNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			return DrawFields(graph, node);
		}
	}
}