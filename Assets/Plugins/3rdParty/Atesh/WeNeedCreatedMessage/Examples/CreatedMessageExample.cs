// ReSharper disable UnusedMember.Local
#pragma warning disable 649

using UnityEngine;

namespace Atesh.WeNeedCreatedMessage.Examples
{
    sealed class CreatedMessageExample : MonoBehaviour
    {
        public GameObject Prefab;

        GameObject LastInstantiatedObject;

        void OnGUI()
        {
            if (GUILayout.Button("Instantiate Prefab as Active")) Instantiate(Prefab).transform.parent = transform.parent;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Instantiate Prefab as Inactive"))
            {
                Prefab.SetActive(false);
                LastInstantiatedObject = Instantiate(Prefab);
                LastInstantiatedObject.transform.parent = transform.parent;
                Prefab.SetActive(true);
            }
            if (GUILayout.Button("Activate last instantiated object"))
            {
                if (LastInstantiatedObject) LastInstantiatedObject.SetActive(true);
            }
            GUILayout.EndHorizontal();
        }
    }
}