using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(NoiseQualityNode), Strings.Properties, "Noise Quality")]
	public class NoiseQualityNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var noiseQualityNode = node as NoiseQualityNode;

			var preview = GetPreview(graph, node);

			noiseQualityNode.PropertyValue = Deltas.DetectDelta<NoiseQuality>(noiseQualityNode.PropertyValue, (NoiseQuality)EditorGUILayout.EnumPopup("Value", noiseQualityNode.PropertyValue), ref preview.Stale);

			return noiseQualityNode;
		}
	}
}