using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(RidgedMultifractalNode), Strings.Generators, "Ridged Multifractal")]
	public class RidgedMultifractalNodeEditor : NodeEditor {}
}