using UnityEngine;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	// todo: this is poorly named, it's more of a cache than a preview...
	public class DomainPreview
	{
		public string Id;
		public bool Stale;
		public Texture2D Preview;
		public long LastUpdated;
		public VisualizationPreview LastVisualizer;
		public object LastModule;
		public string LastSourceId;
		public string Warning;
	}
}