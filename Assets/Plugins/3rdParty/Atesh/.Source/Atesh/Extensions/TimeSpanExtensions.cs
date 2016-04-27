// TimeSpan format routine originally courtesy of RICHARD DUTTON:
// http://dutton.me.uk/2013/09/26/custom-format-string-for-a-net-3-5-timespan-object/
//
// Improved by Onur "Xtro" Er and included in Atesh Framework / December 2014.

using System;
using System.Text;

namespace Atesh
{
    public static class TimeSpanExtensions
    {
        ///  <summary>  
        ///  Returns the TimeSpan object as a string using the provided format string.  
        ///  Currently supports:  
        ///  d  - Days
        ///  h  - Hours  
        ///  m  - Minutes  
        ///  s  - Seconds  
        ///  dd - Zero padded days
        ///  hh - Zero padded hours  
        ///  mm - Zero padded minutes  
        ///  ss - Zero padded seconds  
        ///  </summary>  
        /// <param name="This">The TimeSpan object</param>  
        /// <param name="Format">The format string</param>
        /// <param name="FixedUnit">The time units below the given unit(including itself) will be displayed even if the time value is zero.</param>
        /// <param name="TrimLeadingNonNumerics">Set this to false for leading non numeric characters not to be trimmed. The default value is true.</param>
        /// <returns>Formatted string representation of the TimeSpan</returns>  
        public static string ToString(this TimeSpan This, string Format, TimeUnit? FixedUnit = null, bool TrimLeadingNonNumerics = true)
        {
            var StringBuilder = new StringBuilder();

            for (var I = 0; I < Format.Length; I++)
            {
                switch (Format[I])
                {
                case 'd':
                    if (FixedUnit == null || This.Days > 0 || FixedUnit >= TimeUnit.Day)
                    {
                        if (I < Format.Length - 1 && Format[I + 1] == 'd')
                        {
                            if (This.Days < 10) StringBuilder.AppendFormat("0{0}", This.Hours);
                            else StringBuilder.Append(This.Days);
                            I++;
                        }
                        else StringBuilder.Append(This.Days);
                    }
                    break;
                case 'h':
                    if (FixedUnit == null || This.Days > 0 || This.Hours > 0 || FixedUnit >= TimeUnit.Hour)
                    {
                        if (I < Format.Length - 1 && Format[I + 1] == 'h')
                        {
                            if (This.Hours < 10) StringBuilder.AppendFormat("0{0}", This.Hours);
                            else StringBuilder.Append(This.Hours);
                            I++;
                        }
                        else StringBuilder.Append(This.Hours);
                    }
                    break;
                case 'm':
                    if (FixedUnit == null || This.Days > 0 || This.Hours > 0 || This.Minutes > 0 || FixedUnit >= TimeUnit.Minute)
                    {
                        if (I < Format.Length - 1 && Format[I + 1] == 'm')
                        {
                            if (This.Minutes < 10) StringBuilder.AppendFormat("0{0}", This.Minutes);
                            else StringBuilder.Append(This.Minutes);
                            I++;
                        }
                        else StringBuilder.Append(This.Minutes);
                    }
                    break;
                case 's':
                    if (I < Format.Length - 1 && Format[I + 1] == 's')
                    {
                        if (This.Seconds < 10) StringBuilder.AppendFormat("0{0}", This.Seconds);
                        else StringBuilder.Append(This.Seconds);
                        I++;
                    }
                    else StringBuilder.Append(This.Seconds);
                    break;
                default: // Pass through any non recognised characters  
                    StringBuilder.Append(Format[I]);
                    break;
                }
            }

            if (TrimLeadingNonNumerics)
            {
                var TrimLength = 0;
                for (var I = 0; I < StringBuilder.Length; I++)
                {
                    if (char.IsNumber(StringBuilder[I])) continue;

                    TrimLength = I + 1;
                    break;
                }
                StringBuilder.Remove(0, TrimLength);
            }

            return StringBuilder.ToString();
        }
    }
}