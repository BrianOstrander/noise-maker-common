using UnityEditor;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(SeedNode), Strings.Utility, "Seed")]
	public class SeedNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var seedNode = node as SeedNode;

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Current Value");
				GUILayout.FlexibleSpace();
				EditorGUILayout.SelectableLabel(seedNode.GetValue(graph).ToString(), GUI.skin.textField, GUILayout.Height(16f), GUILayout.Width(55f));
			}
			GUILayout.EndHorizontal();

			return seedNode;
		}
	}
}