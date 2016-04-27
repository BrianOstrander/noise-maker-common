// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;
using UnityEditor;

namespace Atesh.MagicAutoLinker.Editor
{
    class Menu
    {
        #region Messages
        // ReSharper disable UnusedMember.Local
        [MenuItem("Window/Magic Auto Linker/Preview Window")]
        static void PreviewWindow() => AutoLinkerPreviewWindow.Show();

        [MenuItem("Window/Magic Auto Linker/Documentation")]
        static void Documentation() => Application.OpenURL("https://docs.google.com/document/d/1hCXerp9d1OQlYknq5roHTgNa9YUYg3jIoLuurTUhELA");

        // ReSharper restore UnusedMember.Local
        #endregion
    }
}