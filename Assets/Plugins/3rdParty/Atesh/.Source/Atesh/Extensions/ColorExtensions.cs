// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh
{
    public static class ColorExtensions
    {
        public static Color NewA(this Color This, float A)
        {
            This.a = A;
            return This;
        }
    }
}
