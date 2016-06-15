using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;
using Atesh;
using LunraGames.NoiseMaker.Modifiers;

namespace LunraGames.NoiseMaker
{
	public class CurveSimpleNode : Node<IModule>
	{
		public AnimationCurve Curve = new AnimationCurve();

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

			var curveSimple = Value == null ? new CurveSimpleOutput(sources[0] as IModule, Curve) : Value as CurveSimpleOutput;

			curveSimple.SourceModule = sources[0] as IModule;
			curveSimple.Curve = Curve;

			Value = curveSimple;
			return Value;
		}
	}
}