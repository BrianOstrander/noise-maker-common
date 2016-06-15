using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;

namespace LunraGames.NoiseMaker
{
	public class ConstantNode : Node<IModule>
	{
		public float Constant;

		public override IModule GetValue (List<INode> nodes)
		{
			var constant = Value == null ? new Constant(Constant) : Value as Constant;

			constant.Value = Constant;

			Value = constant;
			return Value;
		}
	}
}