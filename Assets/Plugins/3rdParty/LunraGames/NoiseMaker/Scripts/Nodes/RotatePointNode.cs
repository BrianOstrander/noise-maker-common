using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class RotatePointNode : Node<IModule>
	{
		public Vector3 Rotation = Vector3.zero;

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

			var rotatePoint = Value == null ? new RotateInput(sources[0] as IModule, Rotation.x, Rotation.y, Rotation.z) : Value as RotateInput;

			rotatePoint.SourceModule = sources[0] as IModule;

			rotatePoint.SetAngles(Rotation.x, Rotation.y, Rotation.z);

			Value = rotatePoint;
			return Value;
		}
	}
}