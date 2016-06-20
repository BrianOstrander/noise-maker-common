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
	}
}