using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LunraGames.NoiseMaker.Modifiers;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class CurveSimpleNode : Node<IModule>
	{
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source;
		public AnimationCurve Curve = new AnimationCurve();

		public override IModule GetValue (Graph graph)
		{
			var source = GetLocalIfValueNull<IModule>(Source, 0, graph);

			if (source == null) return null;

			var curveSimple = Value == null ? new CurveSimpleOutput(source, Curve) : Value as CurveSimpleOutput;

			curveSimple.SourceModule = source;
			curveSimple.Curve = Curve;

			Value = curveSimple;
			return Value;
		}
	}
}