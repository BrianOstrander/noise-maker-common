using UnityEngine;
using Tweener;
using System;
using Atesh.MagicAutoLinker;

namespace LunraGames.Juice
{
	/// <summary>
	/// A rotating, shaking effect for UI elements.
	/// </summary>
	// ReSharper disable UnusedMember.Global
	public class Rotater : Juicer 
	// ReSharper restore UnusedMember.Global
	{
		/// <summary>
		/// The point at which we stop adding half the angle to the rotation.
		/// </summary>
		const float OffsetThreshold = 0.333f;

		#region AutoLinks
	#pragma warning disable 649
		[AutoLinkSelf, HideInInspector]
		public RectTransform RectArea;
	#pragma warning restore 649
		#endregion

		#region Inspector
	#pragma warning disable 649
		// ReSharper disable MemberCanBePrivate.Global
		[Tooltip("Angle to rotate between")]
		public float Angle;
		// ReSharper restore MemberCanBePrivate.Global
	#pragma warning restore 649
		#endregion

		protected override void OnEase (float linearScalar)
		{
			var rotation = 0f;

			if (linearScalar < 0.25f) rotation = Angle * 0.5f * Easing.EaseIn(linearScalar / 0.25f, EaseType);
			else if (linearScalar < 0.75f) rotation = (Easing.EaseInOut(1f - ((linearScalar - 0.25f) / 0.5f), EaseType, EaseType) * Angle) - (Angle * 0.5f);
			else rotation = Easing.EaseIn(1f - ((linearScalar - 0.75f) / 0.25f), EaseType) * Angle * -0.5f;

			transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, rotation));
		}
	}
}