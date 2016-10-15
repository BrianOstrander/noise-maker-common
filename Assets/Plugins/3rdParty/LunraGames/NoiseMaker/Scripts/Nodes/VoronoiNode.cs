using System.Collections.Generic;
using LibNoise;
using LunraGames.NumberDemon;

namespace LunraGames.NoiseMaker
{
	public class VoronoiNode : Node<IModule>
	{
		[NodeLinker(0)]
		public float Displacement = 1f;
		[NodeLinker(1)]
		public bool DistanceEnabled;
		[NodeLinker(2)]
		public float Frequency = 0.02f;
		[NodeLinker(3)]
		public int Seed = DemonUtility.NextInteger;

		public override IModule GetValue (Graph graph)
		{
			var values = NullableValues(graph);

			var voronoi = Value == null ? new Voronoi() : Value as Voronoi;

			voronoi.Displacement = GetLocalIfValueNull<float>(Displacement, 0, values);
			voronoi.DistanceEnabled = GetLocalIfValueNull<bool>(DistanceEnabled, 1, values);
			voronoi.Frequency = GetLocalIfValueNull<float>(Frequency, 2, values);
			voronoi.Seed = GetLocalIfValueNull<int>(Seed, 3, values);

			Value = voronoi;
			return Value;
		}
	}
}