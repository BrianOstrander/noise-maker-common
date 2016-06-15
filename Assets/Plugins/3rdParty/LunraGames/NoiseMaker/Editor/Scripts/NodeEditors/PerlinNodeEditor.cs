using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(PerlinNode), Strings.Generators, "Perlin")]
	public class PerlinNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var perlin = node as PerlinNode;

			var preview = GetModulePreview(graph, node as Node<IModule>);

			GUILayout.Box(preview.Preview, GUILayout.MaxWidth(PreviewWidth), GUILayout.ExpandWidth(true));

			GUILayout.FlexibleSpace();

			perlin.Frequency = Deltas.DetectDelta<float>(perlin.Frequency, EditorGUILayout.FloatField("Frequency", perlin.Frequency), ref preview.Stale);
			perlin.Lacunarity = Deltas.DetectDelta<float>(perlin.Lacunarity, EditorGUILayout.FloatField("Lacunarity", perlin.Lacunarity), ref preview.Stale);
			perlin.Quality = Deltas.DetectDelta<NoiseQuality>(perlin.Quality, (NoiseQuality)EditorGUILayout.EnumPopup("Noise Quality", perlin.Quality), ref preview.Stale);
			perlin.OctaveCount = Deltas.DetectDelta<int>(perlin.OctaveCount, EditorGUILayout.IntSlider("Octave Count", perlin.OctaveCount, 1, 29), ref preview.Stale);
			perlin.Persistence = Deltas.DetectDelta<float>(perlin.Persistence, EditorGUILayout.FloatField("Persistence", perlin.Persistence), ref preview.Stale);
			perlin.Seed = Deltas.DetectDelta<int>(perlin.Seed, EditorGUILayout.IntField("Seed", perlin.Seed), ref preview.Stale);

			return perlin;
		}
	}
}