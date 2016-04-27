using UnityEngine;
using UnityEngine.UI;
using UnityEditor.UI;
using UnityEditor;

namespace LunraGames.Toggler
{
	[CustomEditor(typeof(TogglerButton), true)]
	[CanEditMultipleObjects]
	public class TogglerButtonEditor : SelectableEditor
	{
		SerializedProperty InteractableProperty;
		SerializedProperty TargetGraphicProperty;
		SerializedProperty OnValueChangedProperty;
		SerializedProperty OnClickProperty;
		SerializedProperty ToggleOnColorsProperty;
		SerializedProperty ToggleOffColorsProperty;
		SerializedProperty TransitionOnProperty;
		SerializedProperty TransitionOffProperty;
		SerializedProperty GroupProperty;
		SerializedProperty IsOnProperty;

		protected override void OnEnable()
		{
			base.OnEnable();

			InteractableProperty = serializedObject.FindProperty("m_Interactable");
			TargetGraphicProperty = serializedObject.FindProperty("m_TargetGraphic");
			ToggleOnColorsProperty = serializedObject.FindProperty("ToggleOnColors");
			ToggleOffColorsProperty = serializedObject.FindProperty("ToggleOffColors");
			TransitionOnProperty = serializedObject.FindProperty("ToggleOnTransition");
			TransitionOffProperty = serializedObject.FindProperty("ToggleOffTransition");
			GroupProperty = serializedObject.FindProperty("m_Group");
			IsOnProperty = serializedObject.FindProperty("m_IsOn");
			OnValueChangedProperty = serializedObject.FindProperty("OnValueChanged");
			OnClickProperty = serializedObject.FindProperty("OnClick");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(InteractableProperty);
			EditorGUILayout.PropertyField(TargetGraphicProperty);
			EditorGUILayout.PropertyField(IsOnProperty);
			EditorGUILayout.PropertyField(TransitionOnProperty);
			EditorGUILayout.PropertyField(ToggleOnColorsProperty);
			EditorGUILayout.PropertyField(TransitionOffProperty);
			EditorGUILayout.PropertyField(ToggleOffColorsProperty);
			EditorGUILayout.PropertyField(GroupProperty);

			EditorGUILayout.Space();

			// Draw the event notification options
			EditorGUILayout.PropertyField(OnValueChangedProperty);
			EditorGUILayout.PropertyField(OnClickProperty);

			serializedObject.ApplyModifiedProperties();
		}
	}
}