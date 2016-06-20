using UnityEngine;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class Vector3Node : Node<Vector3>
	{
		public override Vector3 GetValue (List<INode> nodes)
		{
			return Value;
		}
	}
}