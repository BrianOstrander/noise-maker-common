namespace LunraGames.NoiseMaker
{
	public class NoiseUtility
	{
		public static int Seed { get { return UnityEngine.Random.Range(int.MinValue, int.MaxValue); } }
	}
}