using System;

namespace LunraGames.NoiseMaker
{
	[Serializable]
	public abstract class Domain
	{
		public abstract float GetWeight(float latitude, float longitude, float altitude);
	}
}
