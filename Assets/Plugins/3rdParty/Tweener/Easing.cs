using System;
using UnityEngine;

namespace Tweener
{
	public static class Easing
	{
		// Adapted from source : http://www.robertpenner.com/easing/

		static class MathHelper
		{
			public const float Pi = Mathf.PI;
			public const float HalfPi = Mathf.PI * 0.5f;

			public static float Lerp(float from, float to, float step)
			{
				return (to - from) * step + from;
			}
		}

		public enum EasingType
		{
			Step,
			Linear,
			Sine,
			Quadratic,
			Cubic,
			Quartic,
			Quintic,
			Back
		}

		public static float Ease(float linearStep, float acceleration, EasingType type)
		{
			float easedStep = acceleration > 0f ? EaseIn(linearStep, type) : 
				acceleration < 0f ? EaseOut(linearStep, type) : 
				linearStep;

			return MathHelper.Lerp(linearStep, easedStep, Math.Abs(acceleration));
		}

		public static float EaseIn(float linearStep, EasingType type)
		{
			switch (type)
			{
			case EasingType.Step:       return linearStep < 0.5f ? 0f : 1f;
			case EasingType.Linear:     return linearStep;
			case EasingType.Sine:       return Sine.EaseIn(linearStep);
			case EasingType.Quadratic:  return Power.EaseIn(linearStep, 2f);
			case EasingType.Cubic:      return Power.EaseIn(linearStep, 3f);
			case EasingType.Quartic:    return Power.EaseIn(linearStep, 4f);
			case EasingType.Quintic:    return Power.EaseIn(linearStep, 5f);
			case EasingType.Back: 		return Back.EaseIn(linearStep);
			}
			throw new NotImplementedException();
		}

		public static float EaseOut(float linearStep, EasingType type)
		{
			switch (type)
			{
			case EasingType.Step:       return linearStep < 0.5f ? 0f : 1f;
			case EasingType.Linear:     return linearStep;
			case EasingType.Sine:       return Sine.EaseOut(linearStep);
			case EasingType.Quadratic:  return Power.EaseOut(linearStep, 2f);
			case EasingType.Cubic:      return Power.EaseOut(linearStep, 3f);
			case EasingType.Quartic:    return Power.EaseOut(linearStep, 4f);
			case EasingType.Quintic:    return Power.EaseOut(linearStep, 5f);
			case EasingType.Back: 		return Back.EaseOut(linearStep);
			}
			throw new NotImplementedException();
		}

		public static float EaseInOut(float linearStep, EasingType easeInType, EasingType easeOutType)
		{
			return linearStep < 0.5f ? EaseInOut(linearStep, easeInType) : EaseInOut(linearStep, easeOutType);
		}

		static float EaseInOut(float linearStep, EasingType type)
		{
			switch (type)
			{
			case EasingType.Step:       return linearStep < 0.5f ? 0f : 1f;
			case EasingType.Linear:     return linearStep;
			case EasingType.Sine:       return Sine.EaseInOut(linearStep);
			case EasingType.Quadratic:  return Power.EaseInOut(linearStep, 2f);
			case EasingType.Cubic:      return Power.EaseInOut(linearStep, 3f);
			case EasingType.Quartic:    return Power.EaseInOut(linearStep, 4f);
			case EasingType.Quintic:    return Power.EaseInOut(linearStep, 5f);
			case EasingType.Back:		return Back.EaseInOut(linearStep);
			}
			throw new NotImplementedException();
		}

		static class Sine
		{
			public static float EaseIn(float s)
			{
				return Mathf.Sin(s * MathHelper.HalfPi - MathHelper.HalfPi) + 1f;
			}
			public static float EaseOut(float s)
			{
				return Mathf.Sin(s * MathHelper.HalfPi);
			}
			public static float EaseInOut(float s)
			{
				return (Mathf.Sin(s * MathHelper.Pi - MathHelper.HalfPi) + 1f) * 0.5f;
			}
		}

		static class Power
		{
			public static float EaseIn(float s, float power)
			{
				return Mathf.Pow(s, power);
			}
			public static float EaseOut(float s, float power)
			{
				var sign = Mathf.Sign(power);
				return sign * (Mathf.Pow(s - 1f, power) + sign);
			}
			public static float EaseInOut(float s, float power)
			{
				s *= 2f;
				if (s < 1f) return EaseIn(s, power) * 0.5f;
				var sign = Mathf.Sign(power);
				return sign * 0.5f * (Mathf.Pow(s - 2f, power) + sign * 2f);
			}
		}

		static class Back
		{
			public static float EaseIn(float s)
			{
				var m = 1.70158f;
				return s * s * ((m + 1) * s - m);
			}

			public static float EaseOut(float s)
			{
				var m = 1.70158f;
				return ((s -= 1) * s * ((m + 1) * s + m) + 1);
			}

			public static float EaseInOut(float s)
			{
				var m = 1.70158f;
				if ((s *= 2f) < 1f) return 0.5f * (s * s * (((m *= 1.525f) + 1f) * s - m));
				else return 0.5f * ((s -= 2f) * s * (((m *= 1.525f) + 1f) * s + m) + 2f);
			}
		}
	}
}