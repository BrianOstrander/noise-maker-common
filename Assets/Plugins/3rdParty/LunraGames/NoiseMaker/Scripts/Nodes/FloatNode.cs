using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class FloatNode : Node<float>
	{
		public float FloatValue { get { return Value; } set { Value = value; } }



		public override float GetValue (List<INode> nodes)
		{
			return FloatValue;
		}
	}
}