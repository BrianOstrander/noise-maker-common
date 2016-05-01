using UnityEngine;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class NodePreview
	{
		public string Id;
		public bool Stale;
		public Texture2D Preview;
		public long LastUpdated;
		public List<string> LastSourceIds = new List<string>();
	}
}