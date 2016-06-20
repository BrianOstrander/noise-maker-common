using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class BooleanNode : Node<bool>
	{
		public bool BooleanValue;

		public override bool GetValue (List<INode> nodes)
		{
			Value = BooleanValue;
			return Value;
		}
	}
}