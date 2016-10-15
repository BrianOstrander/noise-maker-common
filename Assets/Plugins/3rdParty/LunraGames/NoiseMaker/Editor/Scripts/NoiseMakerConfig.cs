using UnityEngine;
using UnityEditor;
using LunraGames.Singletonnes;
using System;

namespace LunraGames.NoiseMaker
{
	public class NoiseMakerConfig : EditorScriptableSingleton<NoiseMakerConfig>
	{
		public Texture2D[] LoadingPinwheels;

		#region Elevation preview resources
		public GameObject Ico4Vertex;
		public Mesh Ico4VertexMesh;

		public GameObject Ico5Vertex;
		public Mesh Ico5VertexMesh;

		public GameObject Ico4Face;
		public Mesh Ico4FaceMesh;
		#endregion
		
		#region Splash resources
		public Texture2D Splash { get { return EditorGUIUtility.isProSkin ? SplashPro : SplashPeasant; } }
		[SerializeField]
		Texture2D SplashPro;
		[SerializeField]
		Texture2D SplashPeasant;
		
		public Texture2D SplashMini { get { return EditorGUIUtility.isProSkin ? SplashMiniPro : SplashMiniPeasant; } }
		[SerializeField]
		Texture2D SplashMiniPro;
		[SerializeField]
		Texture2D SplashMiniPeasant;
		#endregion
		
		#region Window tab icons
		public Texture2D AuthorTab { get { return EditorGUIUtility.isProSkin ? AuthorTabPro : AuthorTabPeasant; } }
		[SerializeField]
		Texture2D AuthorTabPro;
		[SerializeField]
		Texture2D AuthorTabPeasant;

		public Texture2D PreviewTab { get { return EditorGUIUtility.isProSkin ? PreviewTabPro : PreviewTabPeasant; } }
		[SerializeField]
		Texture2D PreviewTabPro;
		[SerializeField]
		Texture2D PreviewTabPeasant;

		public Texture2D MercatorTab { get { return EditorGUIUtility.isProSkin ? MercatorTabPro : MercatorTabPeasant; } }
		[SerializeField]
		Texture2D MercatorTabPro;
		[SerializeField]
		Texture2D MercatorTabPeasant;
		#endregion

		#region Property option icons
		public Texture2D EditableOption { get { return EditorGUIUtility.isProSkin ? EditableOptionPro : EditableOptionPeasant; } }
		[SerializeField]
		Texture2D EditableOptionPro;
		[SerializeField]
		Texture2D EditableOptionPeasant;
		#endregion

		#region Mercator icons
		public Texture2D DomainIcon { get { return EditorGUIUtility.isProSkin ? DomainIconPro : DomainIconPeasant; } }
		[SerializeField]
		Texture2D DomainIconPro;
		[SerializeField]
		Texture2D DomainIconPeasant;

		public Texture2D BiomeIcon { get { return EditorGUIUtility.isProSkin ? BiomeIconPro : BiomeIconPeasant; } }
		[SerializeField]
		Texture2D BiomeIconPro;
		[SerializeField]
		Texture2D BiomeIconPeasant;

		public Texture2D AltitudeIcon { get { return EditorGUIUtility.isProSkin ? AltitudeIconPro : AltitudeIconPeasant; } }
		[SerializeField]
		Texture2D AltitudeIconPro;
		[SerializeField]
		Texture2D AltitudeIconPeasant;
		#endregion

		#region Domain editors
		public Texture2D DomainLatitudeEmpty { get { return EditorGUIUtility.isProSkin ? DomainLatitudeEmptyPro : DomainLatitudeEmptyPeasant; } }
		[SerializeField]
		Texture2D DomainLatitudeEmptyPro;
		[SerializeField]
		Texture2D DomainLatitudeEmptyPeasant;

		public Texture2D DomainLatitudeFilled { get { return EditorGUIUtility.isProSkin ? DomainLatitudeFilledPro : DomainLatitudeFilledPeasant; } }
		[SerializeField]
		Texture2D DomainLatitudeFilledPro;
		[SerializeField]
		Texture2D DomainLatitudeFilledPeasant;

		public GUIStyle DomainLatitudeWidgetTop { get { return EditorGUIUtility.isProSkin ? DomainLatitudeWidgetTopPro : DomainLatitudeWidgetTopPeasant; } }
		[SerializeField]
		GUIStyle DomainLatitudeWidgetTopPro;
		[SerializeField]
		GUIStyle DomainLatitudeWidgetTopPeasant;

		public GUIStyle DomainLatitudeWidgetBottom { get { return EditorGUIUtility.isProSkin ? DomainLatitudeWidgetBottomPro : DomainLatitudeWidgetBottomPeasant; } }
		[SerializeField]
		GUIStyle DomainLatitudeWidgetBottomPro;
		[SerializeField]
		GUIStyle DomainLatitudeWidgetBottomPeasant;
		
		public Texture2D DomainLatitudeWidgetLine { get { return EditorGUIUtility.isProSkin ? DomainLatitudeWidgetLinePro : DomainLatitudeWidgetLinePeasant; } }
		[SerializeField]
		Texture2D DomainLatitudeWidgetLinePro;
		[SerializeField]
		Texture2D DomainLatitudeWidgetLinePeasant;
		#endregion

		#region Biome editors
		public GUIStyle BiomeAltitudeDeleteWidget { get { return EditorGUIUtility.isProSkin ? BiomeAltitudeDeleteWidgetPro : BiomeAltitudeDeleteWidgetPeasant; } }
		[SerializeField]
		GUIStyle BiomeAltitudeDeleteWidgetPro;
		[SerializeField]
		GUIStyle BiomeAltitudeDeleteWidgetPeasant;

		public GUIStyle BiomeAltitudeEditWidget { get { return EditorGUIUtility.isProSkin ? BiomeAltitudeEditWidgetPro : BiomeAltitudeEditWidgetPeasant; } }
		[SerializeField]
		GUIStyle BiomeAltitudeEditWidgetPro;
		[SerializeField]
		GUIStyle BiomeAltitudeEditWidgetPeasant;

		public GUIStyle BiomeAltitudeWidgetTop { get { return EditorGUIUtility.isProSkin ? BiomeAltitudeWidgetTopPro : BiomeAltitudeWidgetTopPeasant; } }
		[SerializeField]
		GUIStyle BiomeAltitudeWidgetTopPro;
		[SerializeField]
		GUIStyle BiomeAltitudeWidgetTopPeasant;

		public GUIStyle BiomeAltitudeWidgetBottom { get { return EditorGUIUtility.isProSkin ? BiomeAltitudeWidgetBottomPro : BiomeAltitudeWidgetBottomPeasant; } }
		[SerializeField]
		GUIStyle BiomeAltitudeWidgetBottomPro;
		[SerializeField]
		GUIStyle BiomeAltitudeWidgetBottomPeasant;

		public GUIStyle BiomeAltitudeWidgetLine { get { return EditorGUIUtility.isProSkin ? BiomeAltitudeWidgetLinePro : BiomeAltitudeWidgetLinePeasant; } }
		[SerializeField]
		GUIStyle BiomeAltitudeWidgetLinePro;
		[SerializeField]
		GUIStyle BiomeAltitudeWidgetLinePeasant;

		public GUIStyle BiomeAltitudeWidgetMarginTop { get { return EditorGUIUtility.isProSkin ? BiomeAltitudeWidgetMarginTopPro : BiomeAltitudeWidgetMarginTopPeasant; } }
		[SerializeField]
		GUIStyle BiomeAltitudeWidgetMarginTopPro;
		[SerializeField]
		GUIStyle BiomeAltitudeWidgetMarginTopPeasant;

		public GUIStyle BiomeAltitudeWidgetMarginBottom { get { return EditorGUIUtility.isProSkin ? BiomeAltitudeWidgetMarginBottomPro : BiomeAltitudeWidgetMarginBottomPeasant; } }
		[SerializeField]
		GUIStyle BiomeAltitudeWidgetMarginBottomPro;
		[SerializeField]
		GUIStyle BiomeAltitudeWidgetMarginBottomPeasant;
		#endregion
	}
}