using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(ClampNode), Strings.Modifiers, "Clamp", "Specify an input or ensure boundries are valid.")]
	public class ClampNodeEditor : NodeEditor {}
}