#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace LunraGames
{
	public static class EditorUtility
	{
		public static bool GetWindowVisible(Type type)
		{
			var windows = Resources.FindObjectsOfTypeAll(type);
			return windows != null && 0 < windows.Length;
		}

		public static bool GetWindowVisible<T>()
		{
			return GetWindowVisible(typeof(T));
		}

		public static EditorWindow GetGameWindow()
		{
			var type = Type.GetType("UnityEditor.GameView,UnityEditor");
			return GetWindowVisible(type) ? EditorWindow.GetWindow(type) : null;
		}
	}
}
#endif