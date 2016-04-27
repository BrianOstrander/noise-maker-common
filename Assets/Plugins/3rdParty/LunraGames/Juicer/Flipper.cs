using UnityEngine;
using Tweener;
using System;
using Atesh.MagicAutoLinker;

namespace LunraGames.Juice
{
    public class Flipper : Juicer {
		#region AutoLinks
	#pragma warning disable 649
		[AutoLinkSelf, HideInInspector]
		public RectTransform RectArea;
	#pragma warning restore 649
		#endregion

		#region Inspector
	#pragma warning disable 649
		// ReSharper disable MemberCanBePrivate.Global
		[Tooltip("The rotation that should be tweened from")]
		public Vector3 StartRotation;
		[Tooltip("The target rotation")]
		public Vector3 TargetRotation;
		// ReSharper restore MemberCanBePrivate.Global
	#pragma warning restore 649
		#endregion

		protected override void OnEase (float linearScalar)
		{
			RectArea.localRotation = Quaternion.Euler(StartRotation + ((TargetRotation - StartRotation) * Easing.EaseInOut(linearScalar, EaseType, EaseType)));
		}
	}
}