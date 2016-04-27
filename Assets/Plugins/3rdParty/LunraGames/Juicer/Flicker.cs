using UnityEngine;
using Tweener;
using System;
using Atesh;
using Atesh.MagicAutoLinker;
using Random = UnityEngine.Random;

namespace LunraGames.Juice
{
	/// <summary>
	/// A rotating, shaking effect for UI elements.
	/// </summary>
	// ReSharper disable UnusedMember.Global
	public class Flicker : Juicer 
	// ReSharper restore UnusedMember.Global
	{
		#region Inspector
		#pragma warning disable 649
		// ReSharper disable MemberCanBePrivate.Global
		[Tooltip("Min starting angle")]
		public float AngleStartMin;
		[Tooltip("Max starting angle")]
		public float AngleStartMax;
		[Tooltip("Min angle to rotate to")]
		public float AngleMin;
		[Tooltip("Max angle to rotate to")]
		public float AngleMax;
		// ReSharper restore MemberCanBePrivate.Global
		#pragma warning restore 649
		#endregion

		float AngleStart;
		float AngleDelta;

		protected override void OnStart ()
		{
			var rotation = transform.localRotation.eulerAngles;
			transform.localRotation = Quaternion.Euler(new Vector3(rotation.x, rotation.y, rotation.z + Random.Range(AngleStartMin, AngleStartMax)));
		}

		protected override void OnReset ()
		{
			AngleStart = transform.localRotation.eulerAngles.z;
			AngleDelta = Random.Range(AngleMin, AngleMax);
		}

		protected override void OnEase (float linearScalar)
		{
			var scalar = Easing.EaseIn(linearScalar, EaseType);
			var rotation = transform.localRotation.eulerAngles;
			rotation = new Vector3(rotation.x, rotation.y, AngleStart + (scalar * AngleDelta));
			transform.localRotation = Quaternion.Euler(rotation);
		}
	}
}