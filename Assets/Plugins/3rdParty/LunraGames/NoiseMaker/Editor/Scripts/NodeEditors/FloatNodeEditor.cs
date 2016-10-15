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

			booleanNode.PropertyValue = Deltas.DetectDelta<bool>(booleanNode.PropertyValue, EditorGUILayout.Toggle("Value", booleanNode.PropertyValue), ref preview.Stale);

			return booleanNode;
		}
	}
}