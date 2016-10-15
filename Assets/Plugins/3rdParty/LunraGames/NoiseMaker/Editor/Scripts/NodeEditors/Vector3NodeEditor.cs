using UnityEditor;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(Vector3Node), Strings.Properties, "Vector3")]
	public class Vector3NodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var vector3Node = node as Vector3Node;

			var preview = GetPreview<Vector3>(graph, node);

			vector3Node.PropertyValue = Deltas.DetectDelta<Vector3>(vector3Node.PropertyValue, EditorGUILayout.Vector3Field("Value", vector3Node.PropertyValue), ref preview.Stale);

			return vector3Node;
		}
	}
}