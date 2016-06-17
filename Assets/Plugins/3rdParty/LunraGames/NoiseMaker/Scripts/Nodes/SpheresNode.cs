using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;

namespace LunraGames.NoiseMaker
{
	public class SpheresNode : Node<IModule>
	{
		[NodeLinker(0)]
		public float Frequency;

		public override IModule GetValue (List<INode> nodes)
		{
			var cylinders = Value == null ? new Spheres() : Value as Spheres;

			cylinders.Frequency = GetLocalIfValueNull<float>(Frequency, 0, nodes);

			Value = cylinders;
			return Value;
		}
	}
}