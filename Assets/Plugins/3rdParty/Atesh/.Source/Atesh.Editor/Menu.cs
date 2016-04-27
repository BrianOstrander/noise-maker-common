// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;
using UnityEditor;

namespace Atesh.Editor
{
    class Menu
    {
        #region Messages
        // ReSharper disable UnusedMember.Local
        [MenuItem("GameObject/Anchors to Corners %.")]
        static void AnchorsToCorners() => (Selection.activeTransform as RectTransform).AnchorsToCorners();

        [MenuItem("GameObject/Corners to Anchors %,")]
        static void CornersToAnchors() => (Selection.activeTransform as RectTransform).CornersToAnchors();
        // ReSharper restore UnusedMember.Local
        #endregion
    }
}