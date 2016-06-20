using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(InvertNode), Strings.Modifiers, "Invert")]
	public class InvertNodeEditer : NodeEditor {}
}