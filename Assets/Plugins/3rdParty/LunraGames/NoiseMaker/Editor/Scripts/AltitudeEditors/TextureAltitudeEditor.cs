using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[AltitudeDrawer(typeof(TextureAltitude), Strings.Default, "Texture")]
	public class TextureAltitudeEditor : AltitudeEditor
	{
		public override Altitude Draw(Altitude altitude, ref bool changed)
		{
			var texture = altitude as TextureAltitude;
			var wasTexture = texture.Texture;
			texture.Texture = EditorGUILayout.ObjectField(texture.Texture, typeof(Texture2D), false) as Texture2D;
			changed = changed || wasTexture != texture.Texture;
			return texture;
		}
	}
}