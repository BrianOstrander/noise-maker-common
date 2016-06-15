using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using System;

namespace LunraGames.NoiseMaker
{
	public class VoronoiNode : Node<IModule>
	{
		public float Displacement = 1f;
		public bool DistanceEnabled;
		public float Frequency = 0.02f;
		public int Seed = NoiseUtility.Seed;

		public override IModule GetValue (List<INode> nodes)
		{
			var voronoi = Value == null ? new Voronoi() : Value as Voronoi;

			voronoi.Displacement = Displacement;
			voronoi.DistanceEnabled = DistanceEnabled;
			voronoi.Frequency = Frequency;
			voronoi.Seed = Seed;

			Value = voronoi;
			return Value;
		}
	}
}