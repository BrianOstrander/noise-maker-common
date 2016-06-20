using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(TranslatePointNode), Strings.Transformers, "Translate Point")]
	public class TranslatePointNodeEditor : NodeEditor {}
}