using UnityEngine;
using System.Collections;
using Tweener;

namespace LunraGames.Juice
{
	/// <summary>
	/// Spins an object around.
	/// </summary>
	class Spinner : Juicer 
	{
		#region Inspector
#pragma warning disable 649
		// ReSharper disable MemberCanBePrivate.Global
		[Tooltip("Reverse direction?")]
		public bool CounterClockwise;
		// ReSharper restore MemberCanBePrivate.Global
#pragma warning restore 649
		#endregion

		protected override void OnEase (float linearScalar)
		{
			var scalar = Easing.EaseIn(linearScalar, EaseType);
			transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, scalar * (CounterClockwise ? 360f : -360f)));
		}
	}
}