using UnityEngine;
using UnityEditor;

namespace LunraGames.Juice
{
	[CustomEditor(typeof(Juicer), true)]
	[CanEditMultipleObjects]
	public class JuicerEditor : Editor
	{
		SerializedProperty ConstantProperty;
		SerializedProperty InitiallyElapsedProperty;
		SerializedProperty DisableResetsProperty;

		SerializedProperty StartsEasedProperty;
		SerializedProperty TogglesProperty;
		SerializedProperty DelayDurationProperty;
		SerializedProperty DurationProperty;
		SerializedProperty EaseToTypeProperty;
		SerializedProperty EaseFromTypeProperty;
		SerializedProperty PingPongsProperty;

		void OnEnable()
		{
			ConstantProperty = serializedObject.FindProperty("Constant");
			StartsEasedProperty = serializedObject.FindProperty("StartsEased");
			TogglesProperty = serializedObject.FindProperty("Toggles");
			DelayDurationProperty = serializedObject.FindProperty("DelayDuration");
			DurationProperty = serializedObject.FindProperty("Duration");
			DisableResetsProperty = serializedObject.FindProperty("DisableResets");
			InitiallyElapsedProperty = serializedObject.FindProperty("InitiallyElapsed");
			EaseToTypeProperty = serializedObject.FindProperty("EaseToType");
			EaseFromTypeProperty = serializedObject.FindProperty("EaseFromType");
			PingPongsProperty = serializedObject.FindProperty("PingPongs");
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			serializedObject.Update();
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(ConstantProperty);
			if (ConstantProperty.boolValue)
			{
				EditorGUILayout.PropertyField(InitiallyElapsedProperty);
				EditorGUILayout.Space();
			}
			else EditorGUILayout.HelpBox("\"Initially Eased\" is only used for constant eases.", MessageType.Info);

			EditorGUILayout.PropertyField(DisableResetsProperty);
			EditorGUILayout.PropertyField(StartsEasedProperty);
			EditorGUILayout.PropertyField(TogglesProperty);
			EditorGUILayout.PropertyField(DelayDurationProperty);
			EditorGUILayout.PropertyField(DurationProperty);
			EditorGUILayout.LabelField("Total time", (DelayDurationProperty.floatValue + DurationProperty.floatValue)+" seconds");

			if (TogglesProperty.boolValue)
			{
				EditorGUILayout.PropertyField(EaseToTypeProperty);
				EditorGUILayout.PropertyField(EaseFromTypeProperty);
				EditorGUILayout.HelpBox("\"Ping Pongs\" does not work for toggling eases.", MessageType.Info);
				PingPongsProperty.boolValue = false;
			}
			else
			{
				EditorGUILayout.PropertyField(EaseToTypeProperty, new GUIContent("Ease"));
				EaseFromTypeProperty.enumValueIndex = EaseToTypeProperty.enumValueIndex;
				EditorGUILayout.PropertyField(PingPongsProperty);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}