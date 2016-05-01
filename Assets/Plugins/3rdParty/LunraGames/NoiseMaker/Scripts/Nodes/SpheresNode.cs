using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;

namespace LunraGames.NoiseMaker
{
	public class SpheresNode : Node
	{
		public float Frequency;

		public override IModule GetModule (List<Node> nodes)
		{
			var cylinders = Module == null ? new Spheres() : Module as Spheres;

			cylinders.Frequency = Frequency;

			Module = cylinders;
			return Module;
		}
	}
}