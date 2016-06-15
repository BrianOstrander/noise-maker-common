using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class ExponentNode : Node<IModule>
	{
		public float Exponent;

		public override IModule GetValue (List<INode> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 1)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
			}
			if (StringExtensions.IsNullOrWhiteSpace(SourceIds[0])) return null;
			var sources = Sources(nodes);
			if (sources.Count != 1) return null;

			var exponent = Value == null ? new ExponentialOutput(sources[0] as IModule, Exponent) : Value as ExponentialOutput;

			exponent.SourceModule = sources[0] as IModule;
			exponent.Exponent = Exponent;

			Value = exponent;
			return Value;
		}
	}
}