using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(SpheresNode), Strings.Generators, "Spheres")]
	public class SpheresNodeEditor : NodeEditor {}
}