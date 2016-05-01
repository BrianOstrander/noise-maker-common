using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;

namespace LunraGames.NoiseMaker
{
	public class CheckerboardNode : Node
	{
		public override IModule GetModule (List<Node> nodes)
		{
			var checkerboard = Module == null ? new Checkerboard() : Module as Checkerboard;

			Module = checkerboard;
			return Module;
		}
	}
}