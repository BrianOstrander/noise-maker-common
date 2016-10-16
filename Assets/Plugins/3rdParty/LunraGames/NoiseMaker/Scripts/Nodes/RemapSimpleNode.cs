using UnityEngine;
using LibNoise;
using LunraGames.NoiseMaker.Modifiers;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class RemapSimpleNode : Node<IModule>
	{
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source;
		[NodeLinker(1)]
		public AnimationCurve Curve = new AnimationCurve();

		public override IModule GetValue (Graph graph)
		{
			var values = NullableValues(graph);
			var source = GetLocalIfValueNull(Source, 0, values);

			if (source == null) return null;

			var curve = GetLocalIfValueNull(Curve, 1, values);

			var curveSimple = Value == null ? new CurveSimpleOutput(source, curve) : Value as CurveSimpleOutput;

			curveSimple.SourceModule = source;
			curveSimple.Curve = curve;

			Value = curveSimple;

			return Value;
		}
	}
}