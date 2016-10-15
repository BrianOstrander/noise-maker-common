using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(ConstantNode), Strings.Generators, "Constant")]
	public class ConstantNodeEditor : NodeEditor {}
}