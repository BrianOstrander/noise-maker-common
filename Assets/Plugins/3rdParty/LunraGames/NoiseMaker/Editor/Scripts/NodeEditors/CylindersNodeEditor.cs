using UnityEngine;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(CylindersNode), Strings.Generators, "Cylinders")]
	public class CylindersNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var cylinders = node as CylindersNode;

			var preview = GetPreview(graph, node as Node<IModule>);

			GUILayout.Box(preview.Preview, GUILayout.MaxWidth(PreviewWidth), GUILayout.ExpandWidth(true));

			GUILayout.FlexibleSpace();

			cylinders.Frequency = Deltas.DetectDelta<float>(cylinders.Frequency, EditorGUILayout.FloatField("Frequency", cylinders.Frequency), ref preview.Stale);

			return cylinders;
		}
	}
}