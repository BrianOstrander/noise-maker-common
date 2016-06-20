using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class BooleanNode : PropertyNode<bool>
	{
		public bool BooleanValue;

		public override bool GetValue (List<INode> nodes)
		{
			Value = BooleanValue;
			return Value;
		}

		public override void SetProperty (bool value)
		{
			BooleanValue = value;
		}
	}
}