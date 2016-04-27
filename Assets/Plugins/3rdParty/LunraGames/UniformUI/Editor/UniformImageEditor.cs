using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;

namespace LunraGames.UniformUI
{
	[CustomEditor(typeof(UniformImage), true)]
	[CanEditMultipleObjects]
	public class UniformImageEditor : ImageEditor
	{
		SerializedProperty ConfigProperty;
		SerializedProperty PreserveAspect;
		SerializedProperty Type;
		SerializedProperty Sprite;
		AnimBool ShowType;

		protected override void OnEnable() 
		{
			base.OnEnable();
			ConfigProperty = serializedObject.FindProperty("Config");
			PreserveAspect = serializedObject.FindProperty("m_PreserveAspect");
			Type = serializedObject.FindProperty("m_Type");
			Sprite = serializedObject.FindProperty("m_Sprite");

			ShowType = new AnimBool(Sprite.objectReferenceValue != null);
			ShowType.valueChanged.AddListener(Repaint);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			if (ConfigProperty.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox("You must specify a uniform button config, or local values will be used.", MessageType.Warning);
				EditorGUILayout.PropertyField(ConfigProperty);
				serializedObject.ApplyModifiedProperties();
				base.OnInspectorGUI();
			}
			else 
			{
				EditorGUILayout.PropertyField(ConfigProperty);
				SpriteGUI();

				ShowType.target = Sprite.objectReferenceValue != null;
				if (EditorGUILayout.BeginFadeGroup(ShowType.faded)) TypeGUI();
				EditorGUILayout.EndFadeGroup();

				SetShowNativeSize(false);
				if (EditorGUILayout.BeginFadeGroup(m_ShowNativeSize.faded))
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(PreserveAspect);
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.EndFadeGroup();
				NativeSizeButtonGUI();

				serializedObject.ApplyModifiedProperties();
			}
		}

		void SetShowNativeSize(bool instant)
		{
			Image.Type type = (Image.Type)Type.enumValueIndex;
			bool showNativeSize = (type == Image.Type.Simple || type == Image.Type.Filled);
			base.SetShowNativeSize(showNativeSize, instant);
		}
	}
}