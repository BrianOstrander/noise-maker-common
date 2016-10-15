using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(AbsoluteNode), Strings.Modifiers, "Absolute", Strings.SpecifyAnInput)]
	public class AbsoluteNodeEditor : NodeEditor {}
}