using UnityEngine;
using Tweener;
using System;
using Atesh.MagicAutoLinker;
using Random = UnityEngine.Random;

namespace LunraGames.Juice
{
	/// <summary>
	/// A flashing tween.
	/// </summary>
	// ReSharper disable UnusedMember.Global
	[RequireComponent(typeof(CanvasGroup))]
	public class Ghoster : Juicer 
	// ReSharper restore UnusedMember.Global
	{
		#region AutoLinks
	#pragma warning disable 649
		[AutoLinkSelf, HideInInspector]
		public CanvasGroup Group;
	#pragma warning restore 649
		#endregion

		#region Inspector
	#pragma warning disable 649
		// ReSharper disable MemberCanBePrivate.Global
		[Tooltip("Min alpha")]
		public float MinAlpha;
		[Tooltip("Max alpha")]
		public float MaxAlpha;
		// ReSharper restore MemberCanBePrivate.Global
	#pragma warning restore 649
		#endregion

		protected override void OnEase (float linearScalar)
		{
			Group.alpha = MaxAlpha - ((MaxAlpha - MinAlpha) * Easing.EaseIn(linearScalar, EaseType));
		}
	}
}