using UnityEngine;
using System.Collections;
using Tweener;
using System;

namespace LunraGames.Juice
{
	public class Revealer : Juicer
	{

		#region Inspector
	#pragma warning disable 649
		// ReSharper disable MemberCanBePrivate.Global
		[Tooltip("The easing for the revealed")]
		public Easing.EasingType IndividualEaseType;
		// ReSharper restore MemberCanBePrivate.Global
	#pragma warning restore 649
		#endregion

		protected override void OnEase (float linearScalar)
		{
			var scalar = Easing.EaseIn(linearScalar, EaseType);

			var childCount = (float)transform.childCount;
			var individualRange = Duration / childCount;
			var currChild = Mathf.FloorToInt(scalar * childCount);
			var currChildScalar = ((scalar * childCount) - (float)currChild) / individualRange;
			currChildScalar = Easing.EaseIn(currChildScalar, IndividualEaseType);

			for (var i = 0; i < childCount; i++)
			{
				var childGroup = transform.GetChild(i).GetComponent<CanvasGroup>();
				if (i < currChild) childGroup.alpha = 1f;
				else if (i == currChild) childGroup.alpha = currChildScalar;
				else childGroup.alpha = 0f;
			}
		}
	}
}