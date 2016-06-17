using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;

namespace LunraGames.NoiseMaker
{
	public class ConstantNode : Node<IModule>
	{
		[NodeLinker(0)]
		public float Constant;

		public override IModule GetValue (List<INode> nodes)
		{
			var constant = Value == null ? new Constant(Constant) : Value as Constant;

			constant.Value = GetLocalIfValueNull<float>(Constant, 0, nodes);

			Value = constant;
			return Value;
		}
	}
}