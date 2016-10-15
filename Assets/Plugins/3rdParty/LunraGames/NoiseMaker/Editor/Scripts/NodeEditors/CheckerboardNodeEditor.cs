using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(CheckerboardNode), Strings.Generators, "Checkerboard")]
	public class CheckerboardNodeEditor : NodeEditor {}
}