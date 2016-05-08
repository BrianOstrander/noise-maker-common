using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class ScalePointNode : Node
	{
		public Vector3 Scale = Vector3.one;

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

			var scalePoint = Module == null ? new ScaleInput(sources[0], Scale.x, Scale.y, Scale.z) : Module as ScaleInput;

			scalePoint.SourceModule = sources[0];

			scalePoint.X = Scale.x;
			scalePoint.Y = Scale.y;
			scalePoint.Z = Scale.z;

			Module = scalePoint;
			return Module;
		}
	}
}