using UnityEngine;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[DomainDrawer(typeof(LatitudeDomain), "Latitude")]
	public class LatitudeDomainEditor : DomainEditor 
	{
		public override Domain Draw (Mercator mercator, Domain domain, object module, out Texture2D preview)
		{
			var latitude = domain as LatitudeDomain;

			var previewCache = GetPreview(latitude, module);
			preview = previewCache.Preview;

			latitude.MinLatitude = Deltas.DetectDelta<float>(latitude.MinLatitude, EditorGUILayout.FloatField("Minimum Latitude", latitude.MinLatitude), ref previewCache.Stale);
			latitude.MaxLatitude = Deltas.DetectDelta<float>(latitude.MaxLatitude, EditorGUILayout.FloatField("Maximum Latitude", latitude.MaxLatitude), ref previewCache.Stale);

			return domain;
		}
	}
}