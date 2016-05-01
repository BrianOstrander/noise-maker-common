using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(VoronoiNode), Strings.Generators, "Voronoi")]
	public class VoronoiNoiseEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var voronoi = node as VoronoiNode;

			var preview = GetPreview(graph, node);

			GUILayout.Box(preview.Preview, GUILayout.MaxWidth(PreviewSize), GUILayout.ExpandWidth(true));

			GUILayout.FlexibleSpace();

			voronoi.Displacement = Deltas.DetectDelta<float>(voronoi.Displacement, EditorGUILayout.FloatField("Displacement", voronoi.Displacement), ref preview.Stale);
			voronoi.DistanceEnabled = Deltas.DetectDelta<bool>(voronoi.DistanceEnabled, EditorGUILayout.Toggle("Distance Enabled", voronoi.DistanceEnabled), ref preview.Stale);
			voronoi.Frequency = Deltas.DetectDelta<float>(voronoi.Frequency, EditorGUILayout.FloatField("Frequency", voronoi.Frequency), ref preview.Stale);
			voronoi.Seed = Deltas.DetectDelta<int>(voronoi.Seed, EditorGUILayout.IntField("Seed", voronoi.Seed), ref preview.Stale);

			return voronoi;
		}
	}
}