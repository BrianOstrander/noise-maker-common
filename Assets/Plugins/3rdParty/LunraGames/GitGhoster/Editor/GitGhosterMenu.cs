using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace LunraGamesEditor.GitGhoster
{
	public class GitGhosterMenu
	{
		[MenuItem("Assets/Remove Empty Directories")]
		static void RemoveEmptyDirectories()
		{
			GitGhoster.Cleanup();
		}
	}
}