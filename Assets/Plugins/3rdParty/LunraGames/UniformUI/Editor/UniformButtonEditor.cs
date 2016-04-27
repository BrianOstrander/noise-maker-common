using UnityEngine.UI;
using UnityEditor.UI;
using UnityEditor;

namespace LunraGames.UniformUI
{
	[CustomEditor(typeof(UniformButton), true)]
	[CanEditMultipleObjects]
	public class UniformButtonEditor : SelectableEditor
	{
		SerializedProperty TargetGraphicProperty;

		SerializedProperty TransitionProperty;
		SerializedProperty ColorsProperty;

		SerializedProperty OnClickProperty;
		SerializedProperty ConfigProperty;

		protected override void OnEnable()
		{
			base.OnEnable();
			TargetGraphicProperty = serializedObject.FindProperty("m_TargetGraphic");

			TransitionProperty = serializedObject.FindProperty("m_Transition");
			ColorsProperty = serializedObject.FindProperty("m_Colors");

			OnClickProperty = serializedObject.FindProperty("m_OnClick");
			ConfigProperty = serializedObject.FindProperty("Config");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(ConfigProperty);
			EditorGUILayout.PropertyField(TargetGraphicProperty);

			if (ConfigProperty.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox("You must specify a uniform button config, or local values will be used.", MessageType.Warning);
				EditorGUILayout.PropertyField(TransitionProperty);

				var transition = GetTransition(TransitionProperty);
				if (transition == Selectable.Transition.ColorTint) EditorGUILayout.PropertyField(ColorsProperty);
				else if (transition != Selectable.Transition.None) EditorGUILayout.HelpBox("This transition type is not currently implemented.", MessageType.Error);
			}
			EditorGUILayout.PropertyField(OnClickProperty);	
			serializedObject.ApplyModifiedProperties();
		}

		static Selectable.Transition GetTransition(SerializedProperty transition)
		{
			return (Selectable.Transition)transition.enumValueIndex;
		}
	}
}