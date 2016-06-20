using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(ExponentNode), Strings.Modifiers, "Exponent")]
	public class ExponentNodeEditor : NodeEditor {}
}