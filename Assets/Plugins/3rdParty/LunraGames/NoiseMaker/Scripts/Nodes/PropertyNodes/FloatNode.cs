using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class FloatNode : PropertyNode<float>
	{
		public float FloatValue;

		public override float GetValue (List<INode> nodes)
		{
			Value = FloatValue;
			return Value;
		}

		public override void SetProperty (float value)
		{
			FloatValue = value;
		}
	}
}