using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEditor;
using LibNoise;
using Atesh;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(RootNode), Strings.Hidden, "Root", Strings.SpecifyAnInput)]
	public class RootNodeEditor : NodeEditor {}
}