using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(SeedNode), Strings.Utility, "Seed")]
	public class SeedNodeEditor : NodeEditor
	{
		Dictionary<string, int> LastSeeds = new Dictionary<string, int>();

		public override INode Draw(Graph graph, INode node)
		{
			var seedNode = node as SeedNode;
			var currSeed = seedNode.GetValue(graph);
			var preview = GetPreview(graph, node);

			int lastSeed;
			var hadLastSeed = LastSeeds.TryGetValue(node.Id, out lastSeed);

			if (!hadLastSeed || lastSeed != currSeed)
			{
				preview.Stale = true;
				if (hadLastSeed) LastSeeds[node.Id] = currSeed;
				else LastSeeds.Add(node.Id, currSeed);
			}

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Current Value");
				GUILayout.FlexibleSpace();
				EditorGUILayout.SelectableLabel(currSeed.ToString(), GUI.skin.textField, GUILayout.Height(16f), GUILayout.Width(55f));
			}
			GUILayout.EndHorizontal();

			return seedNode;
		}
	}
}