using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(BlendNode), Strings.Selectors, "Blend", "Specify two sources and a weight.")]
	public class BlendNodeEditor : NodeEditor {}
}