using UnityEngine;
using System;
using Tweener;

namespace LunraGames.Juice
{
	public abstract class Juicer : MonoBehaviour 
	{
		#region Serialized Fields
		[HideInInspector]
		public bool Constant;
		[HideInInspector]
		public bool StartsEased;
		[HideInInspector]
		public bool Toggles;
		[HideInInspector]
		public float DelayDuration;
		[HideInInspector]
		public float Duration;
		[HideInInspector]
		public bool DisableResets;
		[HideInInspector]
		public float InitiallyElapsed;
		[HideInInspector]
		public Easing.EasingType EaseToType;
		[HideInInspector]
		public Easing.EasingType EaseFromType;
		[HideInInspector]
		public bool PingPongs;
		#endregion

		public float TotalDuration { get { return DelayDuration + Duration; } }
		public Easing.EasingType EaseType { get { return Toggles && TogglingFrom ? EaseFromType : EaseToType; } }

		bool HasStarted;
		bool TogglingFrom;

		[NonSerialized]
		public float? Elapsed;

		public bool IsEasing { get { return Elapsed.HasValue; } }

		void Start()
		{
			OnStartOrEnable();
			HasStarted = true;
		}

		void OnEnable()
		{
			if (HasStarted) OnStartOrEnable();
		}

		void OnStartOrEnable()
		{
			if (HasStarted) 
			{
				if (DisableResets) Elapsed = null;
				else return;
			}

			OnStart();
			OnReset();
			if (StartsEased)
			{
				if (Toggles) TogglingFrom = true;
				OnEase(1f);
			}
			if (Constant) Ease(Elapsed.HasValue ? Elapsed.Value : InitiallyElapsed);
		}

		#region Events
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedMember.Global

		/// <summary>
		/// Starts the shrinking tween, can be called by button OnClick events.
		/// </summary>
		public void Ease(float elapsed = 0f)
		{
			Elapsed = elapsed;
			OnReset();
		}

		void Update()
		{
			if (Elapsed.HasValue && !Mathf.Approximately(Elapsed.Value, TotalDuration))
			{
				// If it's constant, we want to rollover any overflow.
				if (Constant) Elapsed = (Elapsed + Time.deltaTime) % TotalDuration;
				else Elapsed = Mathf.Min(Elapsed.Value + Time.deltaTime, TotalDuration);

				if (Elapsed < DelayDuration) return;

				var progress = Elapsed.Value - DelayDuration;

				// Get the scalar, then flip flop it depending how far we are, so it sizes in the same spot we started.
				var scalar = progress / Duration;
				if (Toggles) scalar = TogglingFrom ? 1f - scalar : scalar;
				else if (PingPongs) scalar = scalar < 0.5f ? scalar / 0.5f : 2f - (scalar / 0.5f);

				OnEase(scalar);
			}
			else 
			{
				if (Elapsed.HasValue && Toggles) TogglingFrom = !TogglingFrom;

				if (Constant) Ease();
				else Elapsed = null;
			}
		}
		// ReSharper restore UnusedMember.Global
		// ReSharper restore UnusedMember.Local
		#endregion

		protected virtual void OnStart() { return; }

		protected virtual void OnReset() { return; }

		protected abstract void OnEase(float linearScalar);
	}
}