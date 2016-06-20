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

			integerNode.IntegerValue = Deltas.DetectDelta<int>(integerNode.IntegerValue, EditorGUILayout.IntField("Value", integerNode.IntegerValue), ref preview.Stale);

			return integerNode;
		}
	}
}