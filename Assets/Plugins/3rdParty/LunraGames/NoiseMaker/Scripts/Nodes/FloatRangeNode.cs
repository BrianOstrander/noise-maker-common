using LunraGames.NumberDemon;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public class FloatRangeNode : Node<float> 
	{
		[NodeLinker(0)]
		public float LowerBound = -1f;
		[NodeLinker(1)]
		public float UpperBound = 1f;
		[NodeLinker(2)]
		public int Seed = DemonUtility.IntSeed;
		[NodeLinker(3)]
		public RangeOverrides RangeOverride;

		float? FloatValue;

		public override float GetValue (Graph graph)
		{
			var values = NullableValues(graph);

			var lowerBound = GetLocalIfValueNull<float>(LowerBound, 0, values);
			var upperBound = GetLocalIfValueNull<float>(UpperBound, 1, values);
			var seed = GetLocalIfValueNull<int>(Seed, 2, values);
			var rangeOverride = GetLocalIfValueNull<RangeOverrides>(RangeOverride, 3, values);

			if (rangeOverride == RangeOverrides.Minimum) return lowerBound;

			if (rangeOverride == RangeOverrides.Maximum) return upperBound;

			// Invalid ranges always return 0.
			if (upperBound < lowerBound) 
			{
				if (!Application.isEditor || (Application.isEditor && Application.isPlaying)) Debug.LogError("UpperBound must be greater than LowerBound");
				return 0f;
			}

			if (rangeOverride == RangeOverrides.Average)
			{
				if (Mathf.Approximately(lowerBound, upperBound)) return lowerBound;
				else return lowerBound + ((upperBound - lowerBound) * 0.5f);
			}

			Random.seed = seed;
			return Random.Range(lowerBound, upperBound);
		}
	}
}