using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(PowerNode), Strings.Combiners, "Power")]
	public class PowerNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var power = node as PowerNode;

			if (power.GetValue(graph.Nodes) != null)
			{
				var preview = GetModulePreview(graph, node as Node<IModule>);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyTwoInputs, MessageType.Warning);

			return power;
		}
	}
}