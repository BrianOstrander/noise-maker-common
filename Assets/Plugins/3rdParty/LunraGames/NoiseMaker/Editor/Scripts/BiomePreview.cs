using UnityEngine;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	// todo: this is poorly named, it's more of a cache than a preview...
	public class BiomePreview
	{
		public string Id;
		public string DomainId;
		public bool Stale;
		public Texture2D Preview;
		public long LastUpdated;
		public VisualizationPreview LastVisualizer;
		public object LastModule;
		public List<string> LastSourceIds = new List<string>();
	}
}