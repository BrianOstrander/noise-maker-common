using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;

namespace LunraGames.NoiseMaker
{
	public class ConstantNode : Node
	{
		public float Value;

		public override IModule GetModule (List<Node> nodes)
		{
			var constNode = Module == null ? new Constant(Value) : Module as Constant;

			constNode.Value = Value;

			Module = constNode;
			return Module;
		}
	}
}