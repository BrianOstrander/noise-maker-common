using UnityEngine;
using LibNoise;
using System.Collections.Generic;
using LibNoise.Modifiers;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class CurveNode : Node<IModule> 
	{
		/// <summary>
		/// The points that define the curved output, where X is input/time, Y is output/value.
		/// </summary>
		public List<Vector2> Points;

		public override IModule GetValue (List<INode> nodes)
		{
			if (SourceIds == null || SourceIds.Count != 1)
			{
				if (SourceIds == null) SourceIds = new List<string>();
				SourceIds.Add(null);
			}
			if (StringExtensions.IsNullOrWhiteSpace(SourceIds[0])) return null;
			var sources = Sources(nodes);
			if (sources.Count != 1) return null;

			var curve = Value == null ? new CurveOutput(sources[0] as IModule) : Value as CurveOutput;

			curve.SourceModule = sources[0] as IModule;

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