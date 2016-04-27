using System;
using UnityEngine;
using Tweener;
using UnityEngine.Serialization;

namespace LunraGames.Juice
{
	/// <summary>
	/// A shrinking effect for UI elements.
	/// </summary>
	// ReSharper disable UnusedMember.Global
	public class Shrinker : Juicer 
	// ReSharper restore UnusedMember.Global
	{
	    #region Inspector
	#pragma warning disable 649
	    // ReSharper disable MemberCanBePrivate.Global
		[Tooltip("The scale to tween to and from")]
		public float TargetScale;
	    // ReSharper restore MemberCanBePrivate.Global
	#pragma warning restore 649
	    #endregion

		protected override void OnEase (float linearScalar)
		{
			var scaleDelta = TargetScale - 1f;
			transform.localScale = Vector3.one * (1f + (scaleDelta * Easing.EaseIn(linearScalar, EaseType)));
		}
	}
}