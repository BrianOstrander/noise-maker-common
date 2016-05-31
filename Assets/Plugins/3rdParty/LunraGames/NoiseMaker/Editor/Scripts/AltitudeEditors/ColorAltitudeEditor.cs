using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[AltitudeDrawer(typeof(ColorAltitude), Strings.Default, "Color")]
	public class ColorAltitudeEditor : AltitudeEditor
	{
		public override Altitude Draw(Altitude altitude)
		{
			var color = altitude as ColorAltitude;

			color.Color = EditorGUILayout.ColorField("Color", color.Color);

			return color;
		}
	}
}