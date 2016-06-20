using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class FloatNode : Node<float>
	{
		public float FloatValue;

		public override float GetValue (List<INode> nodes)
		{
			Value = FloatValue;
			return Value;
		}
	}
}