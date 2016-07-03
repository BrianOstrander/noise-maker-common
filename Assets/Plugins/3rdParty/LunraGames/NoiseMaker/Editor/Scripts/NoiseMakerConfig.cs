using UnityEngine;
using UnityEditor;

namespace LunraGames.NoiseMaker
{
	public class NoiseMakerConfig : ScriptableObject 
	{
		static NoiseMakerConfig _Instance;

		public static NoiseMakerConfig Instance { get { return _Instance ?? ( _Instance = FindInstance()); } }

		static NoiseMakerConfig FindInstance()
		{
			var instances = AssetDatabase.FindAssets("LunraGamesNoiseMakerSettings");
			if (instances.Length != 1) 
			{
				Debug.LogError(instances.Length == 0 ? "No instance of Noise Maker settings exist" : "More than one instance of Noise Maker settings exists");
				return null;
			}
			return AssetDatabase.LoadAssetAtPath<NoiseMakerConfig>(AssetDatabase.GUIDToAssetPath(instances[0]));

		}

		public Texture2D[] LoadingPinwheels;

		#region Elevation preview resources
		public GameObject Ico4Vertex;
		public Mesh Ico4VertexMesh;

		public GameObject Ico5Vertex;
		public Mesh Ico5VertexMesh;

		public GameObject Ico4Face;
		public Mesh Ico4FaceMesh;
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

		public Texture2D DomainLatitudeWidgetTop { get { return EditorGUIUtility.isProSkin ? DomainLatitudeWidgetTopPro : DomainLatitudeWidgetTopPeasant; } }
		[SerializeField]
		Texture2D DomainLatitudeWidgetTopPro;
		[SerializeField]
		Texture2D DomainLatitudeWidgetTopPeasant;

		public Texture2D DomainLatitudeWidgetBottom { get { return EditorGUIUtility.isProSkin ? DomainLatitudeWidgetBottomPro : DomainLatitudeWidgetBottomPeasant; } }
		[SerializeField]
		Texture2D DomainLatitudeWidgetBottomPro;
		[SerializeField]
		Texture2D DomainLatitudeWidgetBottomPeasant;
		
		public Texture2D DomainLatitudeWidgetLine { get { return EditorGUIUtility.isProSkin ? DomainLatitudeWidgetLinePro : DomainLatitudeWidgetLinePeasant; } }
		[SerializeField]
		Texture2D DomainLatitudeWidgetLinePro;
		[SerializeField]
		Texture2D DomainLatitudeWidgetLinePeasant;
		#endregion
	}
}