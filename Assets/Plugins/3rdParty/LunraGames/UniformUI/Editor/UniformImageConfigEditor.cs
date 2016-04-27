using UnityEditor;

namespace LunraGames.UniformUI
{
	[CustomEditor(typeof(UniformImageConfig), true)]
	[CanEditMultipleObjects]
	public class UniformImageConfigEditor : Editor 
	{
		SerializedProperty ColorProperty;

		void OnEnable()
		{
			ColorProperty = serializedObject.FindProperty("Color");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(ColorProperty);
			serializedObject.ApplyModifiedProperties();
		}
	}
}