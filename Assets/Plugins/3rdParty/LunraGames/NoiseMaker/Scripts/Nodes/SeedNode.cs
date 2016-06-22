using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class SeedNode : Node<int> 
	{
		int? Seed;

		public override int GetValue (List<INode> nodes)
		{
			return 0;
			//if (!Seed.HasValue) Seed
		}
	}
}