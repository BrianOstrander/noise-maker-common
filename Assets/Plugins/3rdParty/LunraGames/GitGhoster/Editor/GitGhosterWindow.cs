using UnityEditor;
using UnityEngine;

namespace LunraGames.GitGhoster
{
	public class GitGhosterWindow : EditorWindow 
	{
		bool IsActive;
		bool CheckPeriod;
		bool LogOnNoCleanup;
		bool LogOnCleanup;

		[MenuItem ("Window/Git Ghoster")]
		static void Init () 
		{
			var window = EditorWindow.GetWindow(typeof (GitGhosterWindow), true, "Git Ghoster") as GitGhosterWindow;
			window.IsActive = EditorPrefs.GetBool(PrefKeyNames.IsActive, false);
			window.CheckPeriod = EditorPrefs.GetBool(PrefKeyNames.CheckPeriod, false);
			window.LogOnNoCleanup = EditorPrefs.GetBool(PrefKeyNames.LogOnNoCleanup, false);
			window.LogOnCleanup = EditorPrefs.GetBool(PrefKeyNames.LogOnCleanup, true);

			window.Show();
		}

		void OnGUI () 
		{
			GUILayout.Label("Deletes empty directories.", EditorStyles.wordWrappedLabel);

			var newIsActive = GUILayout.Toggle(IsActive, "Run every time the git repository changes");
			if (newIsActive != IsActive)
			{
				EditorPrefs.SetBool(PrefKeyNames.IsActive, newIsActive);
				IsActive = newIsActive;
			}

			var newCheckPeriod = GUILayout.Toggle(CheckPeriod, "Check directories beginning with a \".\"");
			if (newCheckPeriod != CheckPeriod)
			{
				EditorPrefs.SetBool(PrefKeyNames.CheckPeriod, newCheckPeriod);
				CheckPeriod = newCheckPeriod;
			}

			var newLogOnCleanup = GUILayout.Toggle(LogOnCleanup, "Log the results of a cleanup");
			if (newLogOnCleanup != LogOnCleanup)
			{
				EditorPrefs.SetBool(PrefKeyNames.LogOnCleanup, newLogOnCleanup);
				LogOnCleanup = newLogOnCleanup;
			}

			var newLogOnNoCleanup = GUILayout.Toggle(LogOnNoCleanup, "Log the results of a cleanup even if no directories were removed");
			if (newLogOnNoCleanup != LogOnNoCleanup)
			{
				EditorPrefs.SetBool(PrefKeyNames.LogOnNoCleanup, newLogOnNoCleanup);
				LogOnNoCleanup = newLogOnNoCleanup;
			}
		}

	}
}