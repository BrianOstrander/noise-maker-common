using UnityEngine;

namespace LunraGames.Spawner
	{
	class SpawnerConfig : ScriptableObject
	{
	    #region Inspector
	#pragma warning disable 649
	    public GameObject[] Prefabs;
	#pragma warning restore 649
	    #endregion
	}
}