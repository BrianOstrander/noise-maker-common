using System;
using System.Linq;
using UnityEngine;
using LunraGames.NumberDemon;

namespace LunraGames.NoiseMaker
{
	public class CurveRangeNode : Node<float>
	{
		[NodeLinker(0)]
		public AnimationCurve Curve = new AnimationCurve();
		[NodeLinker(1)]
		public int Seed = DemonUtility.NextInteger;
		[NodeLinker(2)]
		public CurveRangeOverrides RangeOverride;
		[NodeLinker(3, 1, int.MaxValue)]
		public int OverrideSamples = 1;

		public override float GetValue(Graph graph)
		{
			var values = NullableValues(graph);

			var curve = GetLocalIfValueNull(Curve, 0, values);
			var seed = GetLocalIfValueNull(Seed, 1, values);
			var rangeOverride = GetLocalIfValueNull(RangeOverride, 2, values);
			var overrideSamples = GetLocalIfValueNull(OverrideSamples, 3, values);

			if (curve.keys.Count() == 0) return 0f;

			switch(rangeOverride)
			{
				case CurveRangeOverrides.None: return Curve.Evaluate((new Demon(seed)).GetNextFloat(Curve.keys.Min(k => k.time), Curve.keys.Max(k => k.time)));
				case CurveRangeOverrides.MinimumKey: return curve.MinimumKey();
				case CurveRangeOverrides.MaximumKey: return curve.MaximumKey();
				case CurveRangeOverrides.AverageKey: return curve.AverageKey();
				case CurveRangeOverrides.MinimumSample: return curve.MinimumSample(overrideSamples);
				case CurveRangeOverrides.MaximumSample: return curve.MaximumSample(overrideSamples);
				case CurveRangeOverrides.AverageSample: return curve.AverageSample(overrideSamples);
				default: throw new NotImplementedException("Unhandled CurveRangeOverride "+rangeOverride);
			}
		}


	}
}