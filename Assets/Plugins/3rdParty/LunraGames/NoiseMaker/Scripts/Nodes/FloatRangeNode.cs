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

		float? FloatValue;
		int LastSeed;
		float LastLowerBound;
		float LastUpperBound;

		public override float GetValue (Graph graph)
		{
			var values = NullableValues(graph);

			var seed = GetLocalIfValueNull<int>(Seed, 2, values);

			var lowerBound = GetLocalIfValueNull<float>(LowerBound, 0, values);
			var upperBound = GetLocalIfValueNull<float>(UpperBound, 1, values);

			if (seed == LastSeed && LastLowerBound == lowerBound && LastUpperBound == upperBound && FloatValue.HasValue) return FloatValue.Value;

			if (upperBound < lowerBound)
			{
				if (FloatValue.HasValue) return FloatValue.Value;
				else return 0f;
			}

			LastSeed = seed;
			LastLowerBound = lowerBound;
			LastUpperBound = upperBound;

			if (Mathf.Approximately(upperBound, lowerBound)) FloatValue = upperBound;
			else 
			{
				Random.seed = seed;
				FloatValue = Random.Range(lowerBound, upperBound);
			}

			return FloatValue.Value;
		}
	}
}