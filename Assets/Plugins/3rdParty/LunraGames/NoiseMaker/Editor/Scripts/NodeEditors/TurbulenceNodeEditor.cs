using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(TurbulenceNode), Strings.Transformers, "Turbulence")]
	public class TurbulenceNodeEditor : NodeEditor {}
}