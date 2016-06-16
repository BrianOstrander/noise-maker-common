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
		public override INode Draw(Graph graph, INode node)
		{
			var exponent = node as ExponentNode;

			if (exponent.GetValue(graph.Nodes) != null)
			{
				var preview = GetPreview<IModule>(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();

				exponent.Exponent = Deltas.DetectDelta<float>(exponent.Exponent, EditorGUILayout.FloatField("Exponent", exponent.Exponent), ref preview.Stale);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return exponent;
		}
	}
}