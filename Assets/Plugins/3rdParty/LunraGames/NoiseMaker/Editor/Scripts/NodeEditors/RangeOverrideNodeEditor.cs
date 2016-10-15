using UnityEditor;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(RangeOverrideNode), Strings.Properties, "Range Override")]
	public class RangeOverrideNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var overrideNode = node as RangeOverrideNode;

			var preview = GetPreview<RangeOverrides>(graph, node);

			overrideNode.PropertyValue = Deltas.DetectDelta<RangeOverrides>(overrideNode.PropertyValue, (RangeOverrides)EditorGUILayout.EnumPopup("Value", overrideNode.PropertyValue), ref preview.Stale);

			return overrideNode;
		}
	}
}