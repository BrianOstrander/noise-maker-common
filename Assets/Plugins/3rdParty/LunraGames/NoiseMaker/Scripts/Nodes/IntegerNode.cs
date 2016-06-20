using UnityEngine;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class IntegerNode : Node<int>
	{
		public int IntegerValue;

		public override int GetValue (List<INode> nodes)
		{
			Value = IntegerValue;
			return Value;
		}
	}
}