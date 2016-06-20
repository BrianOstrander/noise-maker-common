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

			vector3Node.Vector3Value = Deltas.DetectDelta<Vector3>(vector3Node.Vector3Value, EditorGUILayout.Vector3Field("Value", vector3Node.Vector3Value), ref preview.Stale);

			return vector3Node;
		}
	}
}