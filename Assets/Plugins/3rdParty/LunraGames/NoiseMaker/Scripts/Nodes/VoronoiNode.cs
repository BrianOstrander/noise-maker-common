using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using System;

namespace LunraGames.NoiseMaker
{
	public class VoronoiNode : Node
	{
		public float Displacement = 1f;
		public bool DistanceEnabled;
		public float Frequency = 0.02f;
		public int Seed = NoiseUtility.Seed;

		public override IModule GetModule (List<Node> nodes)
		{
			var voronoi = Module == null ? new Voronoi() : Module as Voronoi;

			voronoi.Displacement = Displacement;
			voronoi.DistanceEnabled = DistanceEnabled;
			voronoi.Frequency = Frequency;
			voronoi.Seed = Seed;

			Module = voronoi;
			return Module;
		}
	}
}