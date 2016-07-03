using UnityEngine;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[DomainDrawer(typeof(LatitudeDomain), "Latitude", "Define a minimum and maximum latitude constraint for a Biome")]
	public class LatitudeDomainEditor : DomainEditor 
	{
		public override Domain Draw (Mercator mercator, Domain domain, object module, out Texture2D preview)
		{
			var latitude = domain as LatitudeDomain;

			var previewCache = GetPreview(mercator, latitude, module);
			preview = previewCache.Preview;

			latitude.MaxLatitude = Deltas.DetectDelta<float>(latitude.MaxLatitude, EditorGUILayout.FloatField("Maximum Latitude", latitude.MaxLatitude), ref previewCache.Stale);
			latitude.MinLatitude = Deltas.DetectDelta<float>(latitude.MinLatitude, EditorGUILayout.FloatField("Minimum Latitude", latitude.MinLatitude), ref previewCache.Stale);

			var lastRect = GUILayoutUtility.GetLastRect();

			var topStart = (90 - latitude.MaxLatitude) / 180f;
			var bottomStart = (90 - latitude.MinLatitude) / 180f;

			var emptyTexture = NoiseMakerConfig.Instance.DomainLatitudeEmpty;
			var filledTexture = NoiseMakerConfig.Instance.DomainLatitudeFilled;

			var topArea = new Rect(0f, lastRect.yMax, (float)emptyTexture.width, (float)emptyTexture.height * topStart);
			var middleArea = new Rect(0f, topArea.yMax, topArea.width, (float)emptyTexture.height * (bottomStart - topStart));
			var bottomArea = new Rect(0f, middleArea.yMax, topArea.width, (float)emptyTexture.height * (1f - bottomStart));

			var topHeightScaled = (topArea.height / (float)emptyTexture.height);
			var middleHeightScaled = (middleArea.height / (float)filledTexture.height);
			var bottomHeightScaled = (bottomArea.height / (float)emptyTexture.height);

			GUI.DrawTextureWithTexCoords(topArea, emptyTexture, new Rect(0f, 1f - topHeightScaled, 1f, topHeightScaled));
			GUI.DrawTextureWithTexCoords(middleArea, filledTexture, new Rect(0f, -(middleHeightScaled + topHeightScaled), 1f, middleHeightScaled));
			GUI.DrawTextureWithTexCoords(bottomArea, emptyTexture, new Rect(0f, 0f, 1f, bottomHeightScaled));

			return domain;
		}
	}
}