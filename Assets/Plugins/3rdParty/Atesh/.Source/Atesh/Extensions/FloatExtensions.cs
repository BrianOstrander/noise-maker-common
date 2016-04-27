// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;

namespace Atesh
{
    public static class FloatExtensions
    {
        public static string ToTimeString(this float This, TimeUnit? FixedUnit = null, string Format = "d:hh:mm:ss", bool TrimLeadingNonNumerics = true) => TimeSpan.FromSeconds(This).ToString(Format, FixedUnit, TrimLeadingNonNumerics);
        public static string ToShortTimeString(this float This, TimeUnit? FixedUnit = null) => TimeSpan.FromSeconds(This).ToString("mm:ss", FixedUnit);
    }
}