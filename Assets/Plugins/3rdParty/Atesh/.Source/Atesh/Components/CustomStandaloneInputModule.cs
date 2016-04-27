// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Atesh
{
    public class CustomStandaloneInputModule : StandaloneInputModule
    {
        public Dictionary<int, PointerEventData> PointerData => m_PointerData;
        public PointerEventData GetPointerDataLeft() => m_PointerData[kMouseLeftId];
        public PointerEventData GetPointerDataRight() => m_PointerData[kMouseRightId];
        public PointerEventData GetPointerDataMiddle() => m_PointerData[kMouseMiddleId];
    }
}