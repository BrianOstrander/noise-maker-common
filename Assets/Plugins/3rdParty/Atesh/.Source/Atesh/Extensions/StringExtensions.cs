using System;
using System.Linq;

namespace Atesh
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(string Value) => Value == null || Value.All(char.IsWhiteSpace);

        public static string Replace(this string This, char[] OldChars, string NewValue) => string.Join(NewValue, This.Split(OldChars, StringSplitOptions.RemoveEmptyEntries));
    }
}
