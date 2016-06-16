using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(RidgedMultifractalNode), Strings.Generators, "Ridged Multifractal")]
	public class RidgedMultifractalNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var ridged = node as RidgedMultifractalNode;

			var preview = GetPreview<IModule>(graph, node);

			GUILayout.Box(preview.Preview, GUILayout.MaxWidth(PreviewWidth), GUILayout.ExpandWidth(true));

			GUILayout.FlexibleSpace();

			ridged.Frequency = Deltas.DetectDelta<float>(ridged.Frequency, EditorGUILayout.FloatField("Frequency", ridged.Frequency), ref preview.Stale);
			ridged.Lacunarity = Deltas.DetectDelta<float>(ridged.Lacunarity, EditorGUILayout.FloatField("Lacunarity", ridged.Lacunarity), ref preview.Stale);
			ridged.Quality = Deltas.DetectDelta<NoiseQuality>(ridged.Quality, (NoiseQuality)EditorGUILayout.EnumPopup("Noise Quality", ridged.Quality), ref preview.Stale);
			ridged.OctaveCount = Deltas.DetectDelta<int>(ridged.OctaveCount, EditorGUILayout.IntSlider("Octave Count", ridged.OctaveCount, 1, 29), ref preview.Stale);
			ridged.Seed = Deltas.DetectDelta<int>(ridged.Seed, EditorGUILayout.IntField("Seed", ridged.Seed), ref preview.Stale);

			return ridged;
		}
	}
}