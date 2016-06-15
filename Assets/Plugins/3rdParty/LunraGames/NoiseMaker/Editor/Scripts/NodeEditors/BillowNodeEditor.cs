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
			var billow = node as BillowNode;

			var preview = GetPreview(graph, node as Node<IModule>);

			GUILayout.Box(preview.Preview, GUILayout.MaxWidth(PreviewWidth), GUILayout.ExpandWidth(true));

			GUILayout.FlexibleSpace();

			billow.Frequency = Deltas.DetectDelta<float>(billow.Frequency, EditorGUILayout.FloatField("Frequency", billow.Frequency), ref preview.Stale);
			billow.Lacunarity = Deltas.DetectDelta<float>(billow.Lacunarity, EditorGUILayout.FloatField("Lacunarity", billow.Lacunarity), ref preview.Stale);
			billow.Quality = Deltas.DetectDelta<NoiseQuality>(billow.Quality, (NoiseQuality)EditorGUILayout.EnumPopup("Noise Quality", billow.Quality), ref preview.Stale);
			billow.OctaveCount = Deltas.DetectDelta<int>(billow.OctaveCount, EditorGUILayout.IntSlider("Octave Count", billow.OctaveCount, 1, 29), ref preview.Stale);
			billow.Persistence = Deltas.DetectDelta<float>(billow.Persistence, EditorGUILayout.FloatField("Persistence", billow.Persistence), ref preview.Stale);
			billow.Seed = Deltas.DetectDelta<int>(billow.Seed, EditorGUILayout.IntField("Seed", billow.Seed), ref preview.Stale);

			return billow;
		}
	}
}