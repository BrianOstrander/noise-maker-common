using System.Collections.Generic;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	public class NoiseQualityNode : PropertyNode<NoiseQuality>
	{
		public NoiseQuality NoiseQualityValue;

		public override NoiseQuality GetValue (List<INode> nodes)
		{
			Value = NoiseQualityValue;
			return Value;
		}

		public override void SetProperty (NoiseQuality value)
		{
			NoiseQualityValue = value;
		}
	}
}