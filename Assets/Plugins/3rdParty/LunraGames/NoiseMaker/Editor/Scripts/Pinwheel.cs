using UnityEngine;
using System;
using System.Collections.Generic;
using Tweener;

namespace LunraGames.NoiseMaker
{
	public static class Pinwheel 
	{
		public static void Draw(Vector2 position)
		{
			if (NoiseMakerConfig.Instance == null || NoiseMakerConfig.Instance.LoadingPinwheels == null) return;

			var secondScalar = (float)((DateTime.Now.Second * 1000) + DateTime.Now.Millisecond) / 2000f;
			var alphas = new List<float>();

			for (var i = 0f; i < NoiseMakerConfig.Instance.LoadingPinwheels.Length; i++) 
			{
				var alpha = (secondScalar + (i * 0.77f)) % 1f;
				var easing = alpha < 0.5f ? Easing.EasingType.Sine : Easing.EasingType.Quartic;
				alpha = alpha < 0.5f ? alpha / 0.5f : 1f - ((alpha - 0.5f) / 0.5f);
				alpha = Easing.EaseIn(alpha, easing);
				alphas.Add(alpha);
			}

			var wasColor = GUI.color;
			for (var i = 0; i < alphas.Count; i++)
			{
				var texture = NoiseMakerConfig.Instance.LoadingPinwheels[i];
				GUI.color = new Color(1f, 1f, 1f, alphas[i]);
				GUI.DrawTexture(new Rect(position.x - (texture.width * 0.5f), position.y - (texture.height * 0.5f), texture.width, texture.height), texture);
			}
			GUI.color = wasColor;
		}
	}
}