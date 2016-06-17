using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(PerlinNode), Strings.Generators, "Perlin")]
	public class PerlinNodeEditor : NodeEditor {}
}