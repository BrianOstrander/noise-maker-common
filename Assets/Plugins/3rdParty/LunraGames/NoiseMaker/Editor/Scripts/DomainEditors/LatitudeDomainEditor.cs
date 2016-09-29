using UnityEngine;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[DomainDrawer(typeof(LatitudeDomain), "Latitude", "Define a minimum and maximum latitude constraint for a Biome")]
	public class LatitudeDomainEditor : DomainEditor 
	{
		const string TopTabControlName = "LGM_TopTabControlName";
		const string BottomTabControlName = "LGM_BottomTabControlName";

		public override Domain Draw (Domain domain, object module, out DomainPreview preview)
		{
			var latitude = domain as LatitudeDomain;

			preview = GetPreview(latitude, module);

			GUILayout.Label(GUIContent.none);

			var lastRect = GUILayoutUtility.GetLastRect();

			var topStart = (90 - latitude.MaxLatitude) / 180f;
			var bottomStart = (90 - latitude.MinLatitude) / 180f;

			var emptyTexture = NoiseMakerConfig.Instance.DomainLatitudeEmpty;
			var filledTexture = NoiseMakerConfig.Instance.DomainLatitudeFilled;
			var lineTexture = NoiseMakerConfig.Instance.DomainLatitudeWidgetLine;
			var topTabTexture = NoiseMakerConfig.Instance.DomainLatitudeWidgetTop.normal.background;
			var bottomTabTexture = NoiseMakerConfig.Instance.DomainLatitudeWidgetBottom.normal.background;

			var topArea = new Rect(lastRect.width - emptyTexture.width, lastRect.yMax, emptyTexture.width, emptyTexture.height * topStart);
			var middleArea = new Rect(lastRect.width - emptyTexture.width, topArea.yMax, topArea.width, emptyTexture.height * (bottomStart - topStart));
			var bottomArea = new Rect(lastRect.width - emptyTexture.width, middleArea.yMax, topArea.width, emptyTexture.height * (1f - bottomStart));

			var topHeightScaled = (topArea.height / emptyTexture.height);
			var middleHeightScaled = (middleArea.height / filledTexture.height);
			var bottomHeightScaled = (bottomArea.height / emptyTexture.height);

			var lineWidth = topArea.width * 1.2f;
			var topLine = new Rect(lastRect.width - lineWidth, Mathf.Min(topArea.yMax, bottomArea.yMax - lineTexture.height), lineWidth, lineTexture.height);
			var bottomLine = new Rect(topLine.x, Mathf.Max(middleArea.yMax - topLine.height, topArea.y), lineWidth, lineTexture.height);

			var topTab = new Rect(topLine.x - topTabTexture.width - 46f, Mathf.Max(topLine.yMax - topTabTexture.height, topArea.yMin), 50f, topTabTexture.height);
			var bottomTab = new Rect(bottomLine.x - bottomTabTexture.width - 46f, Mathf.Min(bottomLine.y, bottomArea.yMax - bottomTabTexture.height), 50f, bottomTabTexture.height);

			if (Mathf.Approximately(topTab.y, topArea.y)) bottomTab.y = bottomTab.y < topTab.yMax ? topTab.yMax : bottomTab.y;
			else topTab.y = bottomTab.y < topTab.yMax ? bottomTab.y - topTab.height : topTab.y;

			var topTabText = new Rect(topLine.x + NoiseMakerConfig.Instance.DomainLatitudeWidgetTop.padding.left - 46f, topTab.y + NoiseMakerConfig.Instance.DomainLatitudeWidgetTop.padding.top, 50f, topTab.height - NoiseMakerConfig.Instance.DomainLatitudeWidgetTop.padding.bottom);
			var bottomTabText = new Rect(bottomLine.x + NoiseMakerConfig.Instance.DomainLatitudeWidgetBottom.padding.left - 46f, bottomTab.y + NoiseMakerConfig.Instance.DomainLatitudeWidgetBottom.padding.top, 50f, bottomTab.height - NoiseMakerConfig.Instance.DomainLatitudeWidgetBottom.padding.bottom);

			var pixelLatScalar = 180f / filledTexture.height;
			var lineLat = pixelLatScalar * topLine.height * 2f;

			GUI.DrawTexture(topLine, lineTexture);
			GUI.DrawTexture(bottomLine, lineTexture);

			if (latitude.MaxLatitude < latitude.MinLatitude)
			{
				var wasMax = latitude.MaxLatitude;
				latitude.MaxLatitude = latitude.MinLatitude;
				latitude.MinLatitude = wasMax;
				preview.Stale = true;
				GUIUtility.keyboardControl = -1;
				MercatorMakerWindow.RepaintNow();
			}

			latitude.MaxLatitude = Deltas.DetectDelta(latitude.MaxLatitude, EditorGUI.DelayedFloatField(topTabText, latitude.MaxLatitude, Styles.MercatorLatitudeText), ref preview.Stale);
			GUI.SetNextControlName(TopTabControlName);
			if (GUI.RepeatButton(topTab, GUIContent.none, NoiseMakerConfig.Instance.DomainLatitudeWidgetTop))
			{
				var delta = topTab.center - Event.current.mousePosition;
				var newLat = Mathf.Clamp(latitude.MaxLatitude + (180f * (delta.y / filledTexture.height)), -90f, 90f);
				latitude.MaxLatitude = newLat;
				if (latitude.MaxLatitude <= latitude.MinLatitude + lineLat) latitude.MinLatitude = Mathf.Clamp(latitude.MaxLatitude - lineLat, -90f, 90f);
				preview.Stale = true;
				GUIUtility.keyboardControl = -1;
				MercatorMakerWindow.RepaintNow();
			}

			latitude.MinLatitude = Deltas.DetectDelta(latitude.MinLatitude, EditorGUI.DelayedFloatField(bottomTabText, latitude.MinLatitude, Styles.MercatorLatitudeText), ref preview.Stale);
			GUI.SetNextControlName(BottomTabControlName);
			if (GUI.RepeatButton(bottomTab, GUIContent.none, NoiseMakerConfig.Instance.DomainLatitudeWidgetBottom))
			{
				var delta = bottomTab.center - Event.current.mousePosition;
				var newLat = Mathf.Clamp(latitude.MinLatitude + (180f * (delta.y / filledTexture.height)), -90f, 90f);
				latitude.MinLatitude = newLat;
				if (latitude.MaxLatitude - lineLat <= latitude.MinLatitude) latitude.MaxLatitude = Mathf.Clamp(latitude.MinLatitude + lineLat, -90f, 90f);
				preview.Stale = true;
				GUIUtility.keyboardControl = -1;
				MercatorMakerWindow.RepaintNow();
			}

			GUI.DrawTextureWithTexCoords(topArea, emptyTexture, new Rect(0f, 1f - topHeightScaled, 1f, topHeightScaled));
			GUI.DrawTextureWithTexCoords(middleArea, filledTexture, new Rect(0f, 1f - (middleHeightScaled + topHeightScaled), 1f, middleHeightScaled));
			GUI.DrawTextureWithTexCoords(bottomArea, emptyTexture, new Rect(0f, 0f, 1f, bottomHeightScaled));

			return domain;
		}
	}
}