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

			var preview = GetPreview<NoiseQuality>(graph, node);

			noiseQualityNode.NoiseQualityValue = Deltas.DetectDelta<NoiseQuality>(noiseQualityNode.NoiseQualityValue, (NoiseQuality)EditorGUILayout.EnumPopup("Value", noiseQualityNode.NoiseQualityValue), ref preview.Stale);

			return noiseQualityNode;
		}
	}
}