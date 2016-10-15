using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;

namespace LunraGames.NoiseMaker
{
	public class CheckerboardNode : Node<IModule>
	{
		public override IModule GetValue (Graph graph)
		{
			var checkerboard = Value == null ? new Checkerboard() : Value as Checkerboard;

			Value = checkerboard;
			return Value;
		}
	}
}