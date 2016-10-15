using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using LibNoise;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(CurveSimpleNode), Strings.Modifiers, "Curve Simple")]
	public class CurveSimpleNodeEditor : NodeEditor
	{
		public override INode Draw(Graph graph, INode node)
		{
			var curveSimple = node as CurveSimpleNode;

			if (curveSimple.GetValue(graph) != null)
			{
				var preview = GetPreview<IModule>(graph, node);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();
				var unmodifiedCurve = new AnimationCurve();
				foreach (var key in curveSimple.Curve.keys)
				{
					unmodifiedCurve.AddKey(new Keyframe(key.time, key.value, key.inTangent, key.outTangent));
				}
				curveSimple.Curve = EditorGUILayout.CurveField("Curve", unmodifiedCurve);
				preview.Stale = preview.Stale || AnimationCurveExtensions.CurvesEqual(unmodifiedCurve, curveSimple.Curve);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return curveSimple;
		}
	}
}