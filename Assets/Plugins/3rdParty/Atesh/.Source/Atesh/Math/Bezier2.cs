// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh
{
    public class Bezier2
    {
        public Vector2 Point1;
        public Vector2 Point2;
        public Vector2 Point3;
        public Vector2 Point4;

        float Ax;
        float Ay;

        float Bx;
        float By;

        float Cx;
        float Cy;

        Vector2 LastPoint1;
        Vector2 LastPoint2;
        Vector2 LastPoint3;
        Vector2 LastPoint4;

        readonly CurveMap2 CurveMap;

        public float Length => CurveMap.Length;

        [ValidateParameters]
        public Bezier2([NotLessThan(1)] int Resolution)
        {
            CurveMap = new CurveMap2(Resolution);
        }

        public Vector2 Interpolate(float I)
        {
            Update();

            return GetInterpolation(I);
        }

        public Vector2 Interpolate(float I, out Vector2 Tangent)
        {
            Update();

            Tangent = GetTangent(I);
            return GetInterpolation(I);
        }

        public Vector2 GetTangent(float I)
        {
            Update();

            var I2 = I * I;
            var Tx = 3 * Ax * I2 + 2 * Bx * I + Cx;
            var Ty = 3 * Ay * I2 + 2 * By * I + Cy;

            return new Vector2(Tx, Ty).normalized;
        }

        // Returns the new interpolation key of a point as it moves on the curve by a given distance to its original interpolation key
        [ValidateParameters]
        public float Translate([Range(0f, 1f)] float I, float Distance, out float RemainingDistance)
        {
            Update();

            Distance += CurveMap.DistanceOfInterpolation(I);

            if (Distance < 0)
            {
                RemainingDistance = -Distance;
                return float.NegativeInfinity;
            }
            if (Distance > CurveMap.Length)
            {
                RemainingDistance = Distance - CurveMap.Length;
                return float.PositiveInfinity;
            }

            RemainingDistance = 0;
            return CurveMap.Deinterpolate(Distance);
        }

        Vector2 GetInterpolation(float I)
        {
            var I2 = I * I;
            var I3 = I * I * I;
            var X = Ax * I3 + Bx * I2 + Cx * I + Point1.x;
            var Y = Ay * I3 + By * I2 + Cy * I + Point1.y;

            return new Vector2(X, Y);
        }

        void Update()
        {
            if (Point1 == LastPoint1 && Point2 == LastPoint2 && Point3 == LastPoint3 && Point4 == LastPoint4) return;

            LastPoint1 = Point1;
            LastPoint2 = Point2;
            LastPoint3 = Point3;
            LastPoint4 = Point4;

            // Set constants

            Cx = 3f * (Point2.x - Point1.x);
            Bx = 3f * (Point3.x - Point2.x) - Cx;
            Ax = Point4.x - Point1.x - Cx - Bx;

            Cy = 3f * (Point2.y - Point1.y);
            By = 3f * (Point3.y - Point2.y) - Cy;
            Ay = Point4.y - Point1.y - Cy - By;

            CurveMap.Set(this);
        }
    }

    class CurveMap2
    {
        readonly float[] ArcLengths;
        readonly float Ratio;

        internal CurveMap2(int Resolution)
        {
            ArcLengths = new float[Resolution];
            Ratio = 1f / Resolution;
        }

        internal void Set(Bezier2 Curve)
        {
            var LastPoint = Curve.Interpolate(0);
            var ArcLength = 0f;
            for (var I = 0; I < ArcLengths.Length; I++)
            {
                var Point = Curve.Interpolate((I + 1) * Ratio);
                ArcLength += (LastPoint - Point).magnitude;
                ArcLengths[I] = ArcLength;
                LastPoint = Point;
            }
        }

        internal float Length => ArcLengths[ArcLengths.Length - 1];

        // Returns the distance of an interpolated point on the curve
        internal float DistanceOfInterpolation(float Interpolation)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Interpolation == 1) return Length;

            var ArcIndex = Mathf.FloorToInt(Interpolation / Ratio);
            var InterpolationBeforeArc = ArcIndex * Ratio;
            var InternalInterpolation = (Interpolation - InterpolationBeforeArc) / ((ArcIndex + 1) * Ratio - InterpolationBeforeArc);

            var LengthBeforeArc = ArcIndex > 0 ? ArcLengths[ArcIndex - 1] : 0;
            return Mathf.Lerp(LengthBeforeArc, ArcLengths[ArcIndex], InternalInterpolation);
        }

        // Returns the interpolation key of a point which has the given distance from the first curve point.
        internal float Deinterpolate(float Distance)
        {
            var Index = 0;
            var Low = 0;
            var High = ArcLengths.Length - 1;

            // Binary search to find largest value <= target
            while (Low <= High)
            {
                Index = (Low + High) / 2;
                var Found = ArcLengths[Index];
                var Onebeforefound = Index > 0 ? ArcLengths[Index - 1] : 0;

                if (Distance <= Found && Distance > Onebeforefound) break;
                if (Distance > Found) Low = Index + 1;
                else High = Index;
            }

            // Linear interpolation for index
            var Min = Index > 0 ? ArcLengths[Index - 1] : 0;
            var Max = ArcLengths[Index];
            var I = (Index + (Distance - Min) / (Max - Min)) * Ratio;

            return I;
        }
    }
}