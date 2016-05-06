using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(ExponentNode), Strings.Modifiers, "Exponent")]
	public class ExponentNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var exponent = node as ExponentNode;

			if (exponent.GetModule(graph.Nodes) != null)
			{
				var preview = GetPreview(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				exponent.Exponent = Deltas.DetectDelta<float>(exponent.Exponent, EditorGUILayout.FloatField("Exponent", exponent.Exponent), ref preview.Stale);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return exponent;
		}
	}
}