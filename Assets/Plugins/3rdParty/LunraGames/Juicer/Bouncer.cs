using UnityEngine;
using Tweener;
using Atesh.MagicAutoLinker;

namespace LunraGames.Juice
{
	public class Bouncer : Juicer
	{
		#region AutoLinks
	#pragma warning disable 649
		[AutoLinkSelf, HideInInspector]
		public RectTransform RectArea;
	#pragma warning restore 649
		#endregion

		#region Inspector
	#pragma warning disable 649
		// ReSharper disable MemberCanBePrivate.Global
		[Tooltip("The offset to tween to and from")]
		public Vector3 TargetOffset;
		// ReSharper restore MemberCanBePrivate.Global
	#pragma warning restore 649
		#endregion

		Vector3 StartPosition;

		protected override void OnStart ()
		{
			StartPosition = new Vector3(RectArea.localPosition.x, RectArea.localPosition.y, RectArea.localPosition.z);
		}

		protected override void OnEase (float linearScalar)
		{
			RectArea.localPosition = StartPosition + (TargetOffset * Easing.EaseInOut(linearScalar, EaseType, EaseType));
		}

		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, 8f);
			Gizmos.DrawWireSphere(transform.position + TargetOffset, 4f);
			Gizmos.DrawLine(transform.position, transform.position + TargetOffset);
		}
	}
}