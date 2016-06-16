using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class TurbulenceNode : Node<IModule>
	{
		public float Frequency = 0.02f;
		public float Power;
		public int Roughness = 1;
		public int Seed = NoiseUtility.Seed;

		public override IModule GetValue (List<INode> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 1)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
			}
			if (StringExtensions.IsNullOrWhiteSpace(SourceIds[0])) return null;
			var sources = Values(nodes);
			if (sources.Count != 1) return null;

			var turbulence = Value == null ? new Turbulence(sources[0] as IModule) : Value as Turbulence;

			turbulence.SourceModule = sources[0] as IModule;

			turbulence.Frequency = Frequency;
			turbulence.Power = Power;
			turbulence.Roughness = Roughness;
			turbulence.Seed = Seed;

			Value = turbulence;
			return Value;
		}
	}
}