using UnityEngine;
using System.Collections;
using UnityEditor;
using Atesh;
using System;

namespace LunraGames
{
	public static class EditorPrefsExtensions 
	{
		public static T GetJson<T>(string key, T defaultValue = default(T))
		{
			var serialized = EditorPrefs.GetString(key, string.Empty);
			if (StringExtensions.IsNullOrWhiteSpace(serialized)) return defaultValue;

			try 
			{
				return JsonUtility.FromJson<T>(serialized);
			}
			catch
			{
				Debug.LogError("Problem parsing "+key+" with value: \n\t"+serialized+"\nReturning default value");
				return defaultValue;
			}
		}

		public static void SetJson(string key, object value)
		{
			if (value == null) EditorPrefs.SetString(key, string.Empty);
			else EditorPrefs.SetString(key, JsonUtility.ToJson(value));
		}
	}
}