using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class TranslatePointNode : Node<IModule>
	{
		public Vector3 Position = Vector3.zero;

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

			var translatePoint = Value == null ? new TranslateInput(sources[0] as IModule, Position.x, Position.y, Position.z) : Value as TranslateInput;

			translatePoint.SourceModule = sources[0] as IModule;

			translatePoint.X = Position.x;
			translatePoint.Y = Position.y;
			translatePoint.Z = Position.z;

			Value = translatePoint;
			return Value;
		}
	}
}