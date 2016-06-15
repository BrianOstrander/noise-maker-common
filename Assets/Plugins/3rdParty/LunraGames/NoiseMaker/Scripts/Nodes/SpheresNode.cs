using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;

namespace LunraGames.NoiseMaker
{
	public class SpheresNode : Node<IModule>
	{
		public float Frequency;

		public override IModule GetValue (List<INode> nodes)
		{
			var cylinders = Value == null ? new Spheres() : Value as Spheres;

			cylinders.Frequency = Frequency;

			Value = cylinders;
			return Value;
		}
	}
}