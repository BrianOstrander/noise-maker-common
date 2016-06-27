using System;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	[Serializable]
	public abstract class Domain
	{
		public string Id;
		/// <summary>
		/// The id's of associated altitudes.
		/// </summary>
		public List<string> SourceIds = new List<string>();

		public abstract float GetWeight(float latitude, float longitude, float altitude);
	}
}
