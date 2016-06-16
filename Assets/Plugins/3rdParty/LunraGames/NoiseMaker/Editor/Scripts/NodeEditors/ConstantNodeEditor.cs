using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(ConstantNode), Strings.Generators, "Constant")]
	public class ConstantNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var constant = node as ConstantNode;

			var preview = GetPreview<IModule>(graph, node);

			GUILayout.Box(preview.Preview, GUILayout.MaxWidth(PreviewWidth), GUILayout.ExpandWidth(true));

			GUILayout.FlexibleSpace();

			constant.Constant = Deltas.DetectDelta<float>(constant.Constant, EditorGUILayout.FloatField("Value", constant.Constant), ref preview.Stale);

			return constant;
		}
	}
}