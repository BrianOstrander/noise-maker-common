using System;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LunraGames
{
	public class Git
	{
		public static string GetCurrentSha()
		{
			var root = (new DirectoryInfo(Application.dataPath)).Parent;
			var head = Path.Combine(root.FullName, ".git/logs/HEAD");

			if (!File.Exists(head)) return null;

			var lastLine = File.ReadAllLines(head).LastOrDefault();
			if (lastLine == null) return null;
			var columns = lastLine.Split(' ');
			// The latest commit is on the second column of the last row of the git HEAD file
			if (columns == null || columns.Length < 2) return null;

			return columns[1];
		}
	}
}