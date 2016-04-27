// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh
{
    public static class Math
    {
        public static float GetInterpolationOfClosestPointOnLine(Vector3 A, Vector3 B, Vector3 P)
        {
            var Vector = B - A;

            return Vector3.Dot(P - A, Vector) / (Vector.magnitude * Vector.magnitude);
        }

        public static float GetInterpolationOfClosestPointOnLineSegment(Vector3 A, Vector3 B, Vector3 P)
        {
            var Vector = B - A;

            return Mathf.Clamp(Vector3.Dot(P - A, Vector) / (Vector.magnitude * Vector.magnitude), 0, 1);
        }
    }
}