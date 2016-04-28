using System;
using System.Collections.Generic;

namespace LunraGames
{
	public static class Deltas<T>
	{
		/// <summary>
		/// Detects a delta between the supplied start value and the result of the modification lambda.
		/// </summary>
		/// <returns>The result of the modification lambda.</returns>
		/// <param name="startValue">Starting value.</param>
		/// <param name="modification">Modification lambda that could result in a value different from the <c>startValue</c>.</param>
		/// <param name="changed">Changed returns <c>true</c> if the <c>startValue</c> and the result of the <c>modification</c> lambda are different.</param>
		/// <param name="changedStaysTrue">If set to <c>true</c> then an already <c>true</c> value for <c>changed</c> stays true, even if the <c>startValue</c> and the result of <c>modification</c> are equal.</param>
		public static T DetectDelta(T startValue, Func<T> modification, ref bool changed, bool changedStaysTrue = true)
		{
			var result = modification();
			changed = changed && changedStaysTrue ? true : !EqualityComparer<T>.Default.Equals(startValue, result);
			return result;
		}
	}
}