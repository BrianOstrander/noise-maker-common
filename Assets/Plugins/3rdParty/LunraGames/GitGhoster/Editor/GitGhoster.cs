using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace LunraGames.GitGhoster
{
	public class GitGhoster 
	{
		[InitializeOnLoadMethod]
		static void Initialize()
		{
			CoreAssetPostprocessor.OnPostprocessAllAssetsEvents += Refresh;
		}

		static void Refresh()
		{
			if (EditorPrefs.GetBool(PrefKeyNames.IsActive, false)) Cleanup(true);
		}

		public static void Cleanup(bool onlyIfGitChanged = false)
		{
			if (onlyIfGitChanged) 
			{
				var currCommit = Git.GetCurrentSha();
				var lastCommit = EditorPrefs.GetString(PrefKeyNames.LastGitCommit, currCommit);
				if (currCommit == null || currCommit == lastCommit)
				{
					if (EditorPrefs.GetBool(PrefKeyNames.LogOnNoCleanup, false)) Debug.Log("No changes in git detected, no empty directories will be removed");
					return;
				}
				EditorPrefs.SetString(PrefKeyNames.LastGitCommit, currCommit);
			}

			var removed = new List<string>();
			GitGhoster.RemoveEmptyDirectories(ref removed, Application.dataPath, EditorPrefs.GetBool(PrefKeyNames.CheckPeriod, false));
			AssetDatabase.Refresh();

			var removedLog = "No empty directories found";

			if (removed.Count == 0 && EditorPrefs.GetBool(PrefKeyNames.LogOnNoCleanup, false)) Debug.Log(removedLog);
			else if (EditorPrefs.GetBool(PrefKeyNames.LogOnCleanup, true))
			{
				if (0 < removed.Count)
				{
					removedLog = "Removed the following empty directories:";
					foreach (var dir in removed) removedLog += "\n * "+dir;
				}
				Debug.Log(removedLog);	
			}
		}

		static void RemoveEmptyDirectories(ref List<string> removed, string path, bool checkPeriod = false)
		{
			var root = new DirectoryInfo(path);
			if (!checkPeriod && root.Name.StartsWith(".")) return;
			var dirs = root.GetDirectories();
			foreach (var dir in dirs) RemoveEmptyDirectories(ref removed, dir.FullName, checkPeriod);
			var files = root.GetFiles();
			foreach (var file in files) if (file.Name != ".DS_Store") return;

			root.Refresh();
			if (0 < root.GetDirectories().Length) return;

			removed.Add(root.FullName);

			var metaFullName = root.Parent.FullName+"/"+root.Name+".meta";
			Directory.Delete(root.FullName, true);
			if (File.Exists(metaFullName)) File.Delete(metaFullName);
		}
	}
}