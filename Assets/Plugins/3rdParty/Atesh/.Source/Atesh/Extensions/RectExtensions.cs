using UnityEngine;

namespace Atesh
{
    public static class RectExtensions
    {
        public static bool IntersectsLine(this Rect This, Line2 Line)
        {
            var LineBounds = Line.Bounds;
            Uninverse(ref This);
            Uninverse(ref LineBounds);

            if (!This.Overlaps(LineBounds)) return false;

            var LeftY = Line.CalculateY(This.xMin);

            // If the line is vertical
            if (float.IsNaN(LeftY)) return Line.Point1.x > This.xMin && Line.Point1.x < This.xMax;

            var RightY = Line.CalculateY(This.xMax);

            return !((LeftY >= This.yMax && RightY >= This.yMax) || (LeftY <= This.yMin && RightY <= This.yMin));
        }

        public static void Uninverse(ref Rect Value)
        {
            if (Value.xMin > Value.xMax)
            {
                var Temp = Value.xMin;
                Value.xMin = Value.xMax;
                Value.xMax = Temp;
            }
            if (Value.yMin > Value.yMax)
            {
                var Temp = Value.yMin;
                Value.yMin = Value.yMax;
                Value.yMax = Temp;
            }
        }

        public static Rect Uninversed(ref Rect Value)
        {
            var Result = Value;
            Uninverse(ref Result);
            return Result;
        }

        public static Rect Uninversed(this Rect This)
        {
            var Result = This;
            Uninverse(ref Result);
            return Result;
        }
    }
}