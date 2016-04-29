using System;
using System.Collections.Generic;

namespace LunraGames
{
	public static class Deltas
	{
		/// <summary>
		/// Detects a delta between the supplied start value and the result of the modification lambda.
		/// </summary>
		/// <returns>The result of the modification lambda.</returns>
		/// <param name="startValue">Starting value.</param>
		/// <param name="resultValue">The value that could be different from <c>startValue</c>.</param>
		/// <param name="changed">Changed returns <c>true</c> if the <c>startValue</c> and the <c>resultValue</c> are different.</param>
		/// <param name="changedStaysTrue">If set to <c>true</c> then an already <c>true</c> value for <c>changed</c> stays true, even if the <c>startValue</c> and the <c>resultValue</c> are equal.</param>
		public static T DetectDelta<T>(T startValue, T resultValue, ref bool changed, bool changedStaysTrue = true)
		{
			changed = changed && changedStaysTrue ? true : !EqualityComparer<T>.Default.Equals(startValue, resultValue);
			return resultValue;
		}
	}
}