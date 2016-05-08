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
		public override Node Draw(Graph graph, Node node)
		{
			var power = node as PowerNode;

			if (power.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyTwoInputs, MessageType.Warning);

			return power;
		}
	}
}