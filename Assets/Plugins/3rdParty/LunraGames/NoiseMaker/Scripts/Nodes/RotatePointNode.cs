using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class RotatePointNode : Node
	{
		public Vector3 Rotation = Vector3.zero;

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

			var rotatePoint = Module == null ? new RotateInput(sources[0], Rotation.x, Rotation.y, Rotation.z) : Module as RotateInput;

			rotatePoint.SetAngles(Rotation.x, Rotation.y, Rotation.z);

			Module = rotatePoint;
			return Module;
		}
	}
}