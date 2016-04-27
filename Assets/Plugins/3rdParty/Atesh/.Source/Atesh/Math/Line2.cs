// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh
{
    public struct Line2
    {
        public Vector2 Point1;
        public Vector2 Point2;

        public float Height => Point1.y - Point2.y;
        public float Width => Point1.x - Point2.x;
        public float Slope => Height / Width;
        public Rect Bounds => new Rect(Mathf.Min(Point1.x, Point2.x), Mathf.Min(Point1.y, Point2.y), Mathf.Abs(Width), Mathf.Abs(Height));

        public float CalculateY(float X)
        {
            var InterceptY = Point1.y - Slope * Point1.x;
            var Result = Slope * X + InterceptY;

            return float.IsInfinity(Result) ? float.NaN : Result;
        }
    }
}