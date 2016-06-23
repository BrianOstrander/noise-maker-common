namespace LunraGames.NoiseMaker
{
	public class SeedNode : Node<int> 
	{
		int? Seed;

		public override int GetValue (Graph graph)
		{
			if (!Seed.HasValue) 
			{
				Seed = 0;
				//Seed = graph.Random.Next();\
				//var rand = new System.Random()
			}

			return Seed.Value;
		}
	}
}