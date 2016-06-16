using UnityEditor;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(FloatNode), Strings.Properties, "Float")]
	public class FloatNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var floatNode = node as FloatNode;

			var preview = GetPreview<float>(graph, node);

			floatNode.Value = Deltas.DetectDelta<float>(floatNode.Value, EditorGUILayout.FloatField("Value", floatNode.Value), ref preview.Stale);

			return floatNode;
		}
	}
}