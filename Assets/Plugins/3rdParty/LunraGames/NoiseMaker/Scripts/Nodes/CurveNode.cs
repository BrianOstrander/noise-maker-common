using UnityEngine;
using LibNoise;
using System.Collections.Generic;
using LibNoise.Modifiers;
using Atesh;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class CurveNode : Node<IModule> 
	{
		/// <summary>
		/// The source used if SourceIds[0] is null.
		/// </summary>
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source;
		/// <summary>
		/// The points that define the curved output, where X is input/time, Y is output/value.
		/// </summary>
		public List<Vector2> Points;

		public override IModule GetValue (List<INode> nodes)
		{
			var source = GetLocalIfValueNull<IModule>(Source, 0, nodes);

			if (source == null) return null;

			var curve = Value == null ? new CurveOutput(source) : Value as CurveOutput;

			curve.SourceModule = source;

			if (Points == null)
			{
				// newtonsoft seems to have a weird tendancy to *add* to an existing list, instead of replacing it, so we define it here.
				Points = new List<Vector2> 
				{
					new Vector2(-2f, -2f),
					new Vector2(1f, 1f),
					new Vector2(-1f, -1f),
					new Vector2(2f, 2f)
				};
			}

			curve.ControlPoints = curve.ControlPoints ?? new List<CurveControlPoint>();

			for (var i = 0; i < Points.Count; i++)
			{
				var point = Points[i];

				if (i < curve.ControlPoints.Count) curve.ControlPoints[i] = new CurveControlPoint { Input = point.x, Output = point.y };
				else curve.ControlPoints.Add(new CurveControlPoint { Input = point.x, Output = point.y });
			}

			Value = curve;

			return Value;
		}
	}
}