using UnityEngine;
using UnityEditor;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(CylindersNode), Strings.Generators, "Cylinders")]
	public class CylindersNodeEditor : NodeEditor
	{
		public override Node Draw(Graph graph, Node node)
		{
			var cylinders = node as CylindersNode;

			var preview = GetPreview(graph, node);

			GUILayout.Box(preview.Preview, GUILayout.MaxWidth(PreviewSize), GUILayout.ExpandWidth(true));

			GUILayout.FlexibleSpace();

			cylinders.Frequency = Deltas.DetectDelta<float>(cylinders.Frequency, EditorGUILayout.FloatField("Frequency", cylinders.Frequency), ref preview.Stale);

			return cylinders;
		}
	}
}