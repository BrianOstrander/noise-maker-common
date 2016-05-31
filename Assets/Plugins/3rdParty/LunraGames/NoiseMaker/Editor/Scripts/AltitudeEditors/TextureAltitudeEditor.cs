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
		public override Altitude Draw(Altitude altitude)
		{
			var texture = altitude as TextureAltitude;

			texture.Texture = EditorGUILayout.ObjectField(texture.Texture, typeof(Texture2D), false) as Texture2D;

			return texture;
		}
	}
}