using UnityEditor;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(BooleanNode), Strings.Properties, "Boolean")]
	public class BooleanNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var booleanNode = node as BooleanNode;

			var preview = GetPreview<bool>(graph, node);

			booleanNode.BooleanValue = Deltas.DetectDelta<bool>(booleanNode.BooleanValue, EditorGUILayout.Toggle("Value", booleanNode.BooleanValue), ref preview.Stale);

			return booleanNode;
		}
	}
}