using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class ScalePointNode : Node<IModule>
	{
		public Vector3 Scale = Vector3.one;

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

			var scalePoint = Value == null ? new ScaleInput(sources[0] as IModule, Scale.x, Scale.y, Scale.z) : Value as ScaleInput;

			scalePoint.SourceModule = sources[0] as IModule;

			scalePoint.X = Scale.x;
			scalePoint.Y = Scale.y;
			scalePoint.Z = Scale.z;

			Value = scalePoint;
			return Value;
		}
	}
}