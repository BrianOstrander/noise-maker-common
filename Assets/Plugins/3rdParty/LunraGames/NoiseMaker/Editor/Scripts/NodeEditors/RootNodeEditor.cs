using UnityEditor;
using UnityEngine;
using LunraGames.NumberDemon;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(RootNode), Strings.Hidden, "Root", Strings.SpecifyAnInput)]
	public class RootNodeEditor : NodeEditor 
	{
		public override INode Draw(Graph graph, INode node)
		{
			var rootNode = DrawFields(graph, node) as RootNode;

			var preview = GetPreview<float>(graph, node);

			graph.Seed = Deltas.DetectDelta<int>(graph.Seed, EditorGUILayout.IntField("Seed", graph.Seed), ref preview.Stale);

			if (GUILayout.Button("Randomize"))
			{
				graph.Seed = DemonUtility.IntSeed;
				preview.Stale = true;
			}

			return rootNode;
		}
	}
}