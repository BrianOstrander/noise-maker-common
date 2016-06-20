using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using System;
using Atesh;
using LunraGames.NoiseMaker.Modifiers;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class CurveSimpleNode : Node<IModule>
	{
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source;
		public AnimationCurve Curve = new AnimationCurve();

		public override IModule GetValue (List<INode> nodes)
		{
			var source = GetLocalIfValueNull<IModule>(Source, 0, nodes);

			if (source == null) return null;

			var curveSimple = Value == null ? new CurveSimpleOutput(source, Curve) : Value as CurveSimpleOutput;

			curveSimple.SourceModule = source;
			curveSimple.Curve = Curve;

			Value = curveSimple;
			return Value;
		}
	}
}