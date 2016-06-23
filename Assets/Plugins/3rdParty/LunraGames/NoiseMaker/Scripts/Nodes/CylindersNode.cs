using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;

namespace LunraGames.NoiseMaker
{
	public class CylindersNode : Node<IModule>
	{
		[NodeLinker(0)]
		public float Frequency = 0.02f;

		public override IModule GetValue (Graph graph)
		{
			var cylinders = Value == null ? new Cylinders() : Value as Cylinders;

			cylinders.Frequency = GetLocalIfValueNull<float>(Frequency, 0, graph);

			Value = cylinders;
			return Value;
		}
	}
}