using UnityEngine;
using System.Collections.Generic;
using System;
using Atesh;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace LunraGames.NoiseMaker
{
	public class Field
	{
		public string Name;
		public Type FieldType;
		public Action<object> OnChange;
		object LastValue;
#if UNITY_EDITOR
		public void Draw()
		{
			if (FieldType == typeof(float)) 
			{
				var val = EditorGUILayout.FloatField(StringExtensions.IsNullOrWhiteSpace(Name) ? "Float" : Name, LastValue == null ? 0f : (float)LastValue);
				if (val != (LastValue == null ? 0f : (float)LastValue)) OnChange(val);
				LastValue = val;
			}
			else if (FieldType == typeof(int))
			{
				var val = EditorGUILayout.IntField(StringExtensions.IsNullOrWhiteSpace(Name) ? "Int" : Name, LastValue == null ? 0 : (int)LastValue);
				if (val != (LastValue == null ? 0f : (int)LastValue)) OnChange(val);
				LastValue = val;
			}
			else
			{
				EditorGUILayout.LabelField(StringExtensions.IsNullOrWhiteSpace(Name) ? "Unknown" : Name, "Not supported type");
			}
		}
#endif
	}
}