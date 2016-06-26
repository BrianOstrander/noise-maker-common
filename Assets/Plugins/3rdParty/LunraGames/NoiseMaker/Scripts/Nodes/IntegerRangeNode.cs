using LunraGames.NumberDemon;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public class IntegerRangeNode : Node<int> 
	{
		[NodeLinker(0)]
		public int LowerBound = -1;
		[NodeLinker(1)]
		public int UpperBound = 1;
		[NodeLinker(2)]
		public int Seed = DemonUtility.IntSeed;
		[NodeLinker(3)]
		public RangeOverrides RangeOverride;

		public override int GetValue (Graph graph)
		{
			var values = NullableValues(graph);

			var lowerBound = GetLocalIfValueNull<int>(LowerBound, 0, values);
			var upperBound = GetLocalIfValueNull<int>(UpperBound, 1, values);
			var seed = GetLocalIfValueNull<int>(Seed, 2, values);
			var rangeOverride = GetLocalIfValueNull<RangeOverrides>(RangeOverride, 3, values);

			if (rangeOverride == RangeOverrides.Minimum) return lowerBound;

			if (rangeOverride == RangeOverrides.Maximum) return upperBound;

			// Invalid ranges always return 0.
			if (upperBound < lowerBound) 
			{
				if (!Application.isEditor || (Application.isEditor && Application.isPlaying)) Debug.LogError("UpperBound must be greater than LowerBound");
				return 0;
			}

			if (rangeOverride == RangeOverrides.Average)
			{
				if (lowerBound == upperBound) return lowerBound;
				else return lowerBound + ((upperBound - lowerBound) / 2);
			}

			Random.seed = seed;
			return Random.Range(lowerBound, upperBound);
		}
	}
}