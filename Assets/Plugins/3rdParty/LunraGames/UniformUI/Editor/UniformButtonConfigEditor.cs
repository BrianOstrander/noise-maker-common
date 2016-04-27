using UnityEngine.UI;
using UnityEditor;

namespace LunraGames.UniformUI
{
	[CustomEditor(typeof(UniformButtonConfig), true)]
	[CanEditMultipleObjects]
	public class UniformButtonConfigEditor : Editor 
	{
		SerializedProperty TransitionProperty;
		SerializedProperty ColorsProperty;

		void OnEnable()
		{
			TransitionProperty = serializedObject.FindProperty("Transition");
			ColorsProperty = serializedObject.FindProperty("Colors");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(TransitionProperty);

			var transition = GetTransition(TransitionProperty);
			if (transition == Selectable.Transition.ColorTint) EditorGUILayout.PropertyField(ColorsProperty);
			else if (transition != Selectable.Transition.None) EditorGUILayout.HelpBox("This transition type is not currently implemented.", MessageType.Error);

			serializedObject.ApplyModifiedProperties();
		}

		static Selectable.Transition GetTransition(SerializedProperty transition)
		{
			return (Selectable.Transition)transition.enumValueIndex;
		}
	}
}