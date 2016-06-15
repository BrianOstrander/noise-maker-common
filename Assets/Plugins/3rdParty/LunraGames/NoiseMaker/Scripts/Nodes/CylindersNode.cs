using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;

namespace LunraGames.NoiseMaker
{
	public class CylindersNode : Node<IModule>
	{
		public float Frequency = 0.02f;

		public override IModule GetValue (List<INode> nodes)
		{
			var cylinders = Value == null ? new Cylinders() : Value as Cylinders;

			cylinders.Frequency = Frequency;

			Value = cylinders;
			return Value;
		}
	}
}