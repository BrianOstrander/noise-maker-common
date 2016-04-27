// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using UnityEngine;

namespace Atesh
{
    public static class RectTransformExtensions
    {
        public static void Maximize(this RectTransform This)
        {
            This.anchorMin = Vector2.zero;
            This.anchorMax = Vector2.one;
            This.offsetMin = Vector2.zero;
            This.offsetMax = Vector2.zero;
        }

        public static void AnchorsToCorners(this RectTransform This)
        {
            var Parent = This.parent ? This.parent.GetComponent<RectTransform>() : null;
            if (Parent == null) throw new InvalidOperationException(Strings.ParentIsNotRectTransform(This.name));

            This.anchorMin = new Vector2(This.anchorMin.x + This.offsetMin.x / Parent.rect.width, This.anchorMin.y + This.offsetMin.y / Parent.rect.height);
            This.anchorMax = new Vector2(This.anchorMax.x + This.offsetMax.x / Parent.rect.width, This.anchorMax.y + This.offsetMax.y / Parent.rect.height);
            This.offsetMin = This.offsetMax = new Vector2(0, 0);
        }

        public static void CornersToAnchors(this RectTransform This) => This.offsetMin = This.offsetMax = new Vector2(0, 0);
    }
}