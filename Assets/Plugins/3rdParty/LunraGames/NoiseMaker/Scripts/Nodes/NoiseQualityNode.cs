using System.Collections.Generic;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	public class NoiseQualityNode : Node<NoiseQuality>
	{
		public NoiseQuality NoiseQualityValue;

		public override NoiseQuality GetValue (List<INode> nodes)
		{
			Value = NoiseQualityValue;
			return Value;
		}
	}
}