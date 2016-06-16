using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(TurbulenceNode), Strings.Transformers, "Turbulence")]
	public class TurbulenceNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var turbulence = node as TurbulenceNode;

			if (turbulence.GetValue(graph.Nodes) != null)
			{
				var preview = GetPreview<IModule>(graph, node);

				GUILayout.Box(preview.Preview, GUILayout.MaxWidth(PreviewWidth), GUILayout.ExpandWidth(true));

				GUILayout.FlexibleSpace();

				turbulence.Frequency = Deltas.DetectDelta<float>(turbulence.Frequency, EditorGUILayout.FloatField("Frequency", turbulence.Frequency), ref preview.Stale);
				turbulence.Power = Deltas.DetectDelta<float>(turbulence.Power, EditorGUILayout.FloatField("Power", turbulence.Power), ref preview.Stale);
				turbulence.Roughness = Deltas.DetectDelta<int>(turbulence.Roughness, EditorGUILayout.IntSlider("Roughness", turbulence.Roughness, 1, 29), ref preview.Stale);
				turbulence.Seed = Deltas.DetectDelta<int>(turbulence.Seed, EditorGUILayout.IntField("Seed", turbulence.Seed), ref preview.Stale);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return turbulence;
		}
	}
}