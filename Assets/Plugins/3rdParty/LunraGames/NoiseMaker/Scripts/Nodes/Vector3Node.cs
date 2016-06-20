using UnityEngine;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class Vector3Node : Node<Vector3>
	{
		public Vector3 Vector3Value;

		public override Vector3 GetValue (List<INode> nodes)
		{
			Value = Vector3Value;
			return Value;
		}
	}
}