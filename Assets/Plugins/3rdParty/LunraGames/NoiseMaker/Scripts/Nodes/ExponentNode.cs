using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class ExponentNode : Node
	{
		public float Exponent;

		public override IModule GetModule (List<Node> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 1)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
			}
			if (StringExtensions.IsNullOrWhiteSpace(SourceIds[0])) return null;
			var sources = Sources(nodes);
			if (sources.Count != 1) return null;

			var exponent = Module == null ? new ExponentialOutput(sources[0], Exponent) : Module as ExponentialOutput;

			exponent.Exponent = Exponent;

			Module = exponent;
			return Module;
		}
	}
}