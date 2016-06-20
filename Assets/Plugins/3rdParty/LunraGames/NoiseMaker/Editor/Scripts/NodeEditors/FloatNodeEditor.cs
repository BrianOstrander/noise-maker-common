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

			floatNode.FloatValue = Deltas.DetectDelta<float>(floatNode.FloatValue, EditorGUILayout.FloatField("Value", floatNode.FloatValue), ref preview.Stale);

			return floatNode;
		}
	}
}