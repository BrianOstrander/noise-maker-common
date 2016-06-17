using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(BillowNode), Strings.Generators, "Billow")]
	public class BillowNodeEditor : NodeEditor {}
}