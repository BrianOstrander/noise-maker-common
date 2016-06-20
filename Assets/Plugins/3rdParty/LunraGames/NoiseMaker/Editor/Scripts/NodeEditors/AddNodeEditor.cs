using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(AddNode), Strings.Combiners, "Add")]
	public class AddNodeEditor : NodeEditor {}
}