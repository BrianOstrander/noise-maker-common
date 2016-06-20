using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class FloatNode : Node<float>
	{
		public override float GetValue (List<INode> nodes)
		{
			return Value;
		}
	}
}