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
			var instances = AssetDatabase.FindAssets("LgNmSettings");
			if (instances.Length != 1) 
			{
				Debug.LogError(instances.Length == 0 ? "No instance of Noise Maker settings exist" : "More than one instance of Noise Maker settings exists");
				return null;
			}
			return AssetDatabase.LoadAssetAtPath<NoiseMakerConfig>(AssetDatabase.GUIDToAssetPath(instances[0]));

		}

		public GUISkin EditorSkin;
		public GameObject Ico4Preview;
	}
}