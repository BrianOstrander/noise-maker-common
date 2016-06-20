using UnityEngine;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class Vector3Node : PropertyNode<Vector3>
	{
		public Vector3 Vector3Value;

		public override Vector3 GetValue (List<INode> nodes)
		{
			Value = Vector3Value;
			return Value;
		}

		public override void SetProperty (Vector3 value)
		{
			Vector3Value = value;
		}
	}
}