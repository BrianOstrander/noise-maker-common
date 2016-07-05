using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[AltitudeDrawer(typeof(ColorAltitude), Strings.Default, "Color", "Define a color to appear at this Altitude")]
	public class ColorAltitudeEditor : AltitudeEditor
	{
		public override Altitude Draw(Altitude altitude, ref bool changed)
		{
			var color = altitude as ColorAltitude;
			var wasColor = color.Color;
			color.Color = EditorGUILayout.ColorField("Color", color.Color);
			changed = changed || wasColor != color.Color;
			return color;
		}
	}
}