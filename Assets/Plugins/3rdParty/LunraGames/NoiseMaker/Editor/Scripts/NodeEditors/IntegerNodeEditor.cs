using UnityEditor;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(IntegerNode), Strings.Properties, "Integer")]
	public class IntegerNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var integerNode = node as IntegerNode;

			var preview = GetPreview<float>(graph, node);

			integerNode.Value = Deltas.DetectDelta<int>(integerNode.Value, EditorGUILayout.IntField("Value", integerNode.Value), ref preview.Stale);

			return integerNode;
		}
	}
}