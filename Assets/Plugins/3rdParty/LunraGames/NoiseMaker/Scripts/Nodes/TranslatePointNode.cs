using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class TranslatePointNode : Node
	{
		public Vector3 Position = Vector3.zero;

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

			var translatePoint = Module == null ? new TranslateInput(sources[0], Position.x, Position.y, Position.z) : Module as TranslateInput;

			translatePoint.SourceModule = sources[0];

			translatePoint.X = Position.x;
			translatePoint.Y = Position.y;
			translatePoint.Z = Position.z;

			Module = translatePoint;
			return Module;
		}
	}
}