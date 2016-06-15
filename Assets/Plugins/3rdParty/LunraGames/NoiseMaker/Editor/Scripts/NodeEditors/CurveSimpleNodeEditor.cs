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

			if (curveSimple.GetValue(graph.Nodes) != null)
			{
				var preview = GetModulePreview(graph, node as Node<IModule>);
				GUILayout.Box(preview.Preview);

				GUILayout.FlexibleSpace();
				var unmodifiedCurve = new AnimationCurve();
				foreach (var key in curveSimple.Curve.keys)
				{
					unmodifiedCurve.AddKey(new Keyframe(key.time, key.value, key.inTangent, key.outTangent));
				}
				//var currCurve = EditorGUILayout.CurveField("Curve", curveSimple.Curve);
				curveSimple.Curve = EditorGUILayout.CurveField("Curve", unmodifiedCurve);
				preview.Stale = preview.Stale || CurvesEqual(unmodifiedCurve, curveSimple.Curve);
			}
			else EditorGUILayout.HelpBox(Strings.SpecifyAnInput, MessageType.Warning);

			return curveSimple;
		}

		static bool CurvesEqual(AnimationCurve curve1, AnimationCurve curve2)
		{
			var changed = (curve1 == null && curve2 != null) || (curve1 != null && curve2 == null);

			if (changed) return changed;

			if (curve1.keys.Length == curve2.keys.Length)
			{
				var pairedKeys = new List<Keyframe>(curve1.keys);
				foreach (var key in curve2.keys)
				{
					int? index = null;
					for (var i = 0; i < pairedKeys.Count; i++)
					{
						var k = pairedKeys[i];
						if (Mathf.Approximately(key.inTangent, k.inTangent) && Mathf.Approximately(key.outTangent, k.outTangent) && Mathf.Approximately(key.time, k.time) && Mathf.Approximately(key.value, k.value)) 
						{
							index = i;
							//Debug.Log(key.inTangent+" "+k.inTangent+", "+key.outTangent+" "+k.outTangent+", "+key.time+" "+k.time+", "+key.value+" "+k.value);
						}
						if (index.HasValue) break;
					}
					if (index.HasValue) 
					{
						pairedKeys.RemoveAt(index.Value);
					}
					else
					{
						changed = true;
						break;
					}
				}
			}
			else changed = true;

			return changed;
		}
	}
}