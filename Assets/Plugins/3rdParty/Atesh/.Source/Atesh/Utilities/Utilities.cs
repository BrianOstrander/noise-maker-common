// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Atesh
{
    public static class Utilities
    {
        public static void Swap<T>(ref T A, ref T B)
        {
            var Temp = A;
            A = B;
            B = Temp;
        }

        public static IEnumerable<Transform> SceneRoots(bool OnlyActiveScene = false)
        {
            if (OnlyActiveScene)
            {
                foreach (var Root in SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    yield return Root.transform;
                }
            }
            else
            {
                for (var I = 0; I < SceneManager.sceneCount; I++)
                {
                    var Scene = SceneManager.GetSceneAt(I);
                    foreach (var Root in Scene.GetRootGameObjects())
                    {
                        yield return Root.transform;
                    }
                }
            }
        }

        public static IEnumerable<Transform> AllSceneObjects(bool OnlyActiveScene = false)
        {
            var Roots = SceneRoots(OnlyActiveScene).ToList();

            foreach (var Root in Roots)
            {
                yield return Root;
            }

            foreach (var Descendant in Roots.SelectMany(Root => Root.Descendants()))
            {
                yield return Descendant;
            }
        }
    }
}
