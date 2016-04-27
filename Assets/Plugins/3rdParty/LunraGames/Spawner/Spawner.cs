using UnityEngine;
using Atesh.WeNeedCreatedMessage;

namespace LunraGames.Spawner
{
	class Spawner : CreatedMessageReceiver
	{
		public enum DoneOptions
		{
			DoNothing,
			DestroyComponent,
			DestroyGameObject
		}
	    #region Inspector
	#pragma warning disable 649
		public DoneOptions OnDone;
		public bool SpawnAsSiblings;
	    public SpawnerConfig Config;
	#pragma warning restore 649
	    #endregion

	    protected override void Created(bool onSceneLoad)
	    {
	        foreach (var prefab in Config.Prefabs) Spawn(prefab);
	        if (!SpawnAsSiblings && OnDone == DoneOptions.DestroyGameObject) Debug.LogError("Can't destroy a Spawner's GameObject unless SpawnAsSiblings is true");
	        else if (OnDone == DoneOptions.DestroyComponent) Destroy(this);
	        else if (OnDone == DoneOptions.DestroyGameObject) Destroy(gameObject);
	    }

		void Spawn(GameObject prefab)
		{
			var newPrefab = Instantiate(prefab);
			newPrefab.SetActive(false);
			newPrefab.name = prefab.name;
			if (SpawnAsSiblings)
			{
				if (transform.parent != null) newPrefab.transform.SetParent(transform.parent, false);
		 	} 
			else newPrefab.transform.SetParent(transform, false);
			
		}
	}
}