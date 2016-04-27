using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace LunraGames.GitGhoster
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