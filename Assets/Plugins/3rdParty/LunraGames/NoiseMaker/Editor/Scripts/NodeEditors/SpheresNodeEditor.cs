using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(SpheresNode), Strings.Generators, "Spheres")]
	public class SpheresNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var spheres = node as SpheresNode;

			var preview = GetModulePreview(graph, node as Node<IModule>);

			GUILayout.Box(preview.Preview, GUILayout.MaxWidth(PreviewWidth), GUILayout.ExpandWidth(true));

			GUILayout.FlexibleSpace();

			spheres.Frequency = Deltas.DetectDelta<float>(spheres.Frequency, EditorGUILayout.FloatField("Frequency", spheres.Frequency), ref preview.Stale);

			return spheres;
		}
	}
}