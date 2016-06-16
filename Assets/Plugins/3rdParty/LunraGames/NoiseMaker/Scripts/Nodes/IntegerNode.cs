using UnityEngine;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class IntegerNode : Node<int>
	{
		public override int GetValue (List<INode> nodes)
		{
			return Value;
		}
	}
}