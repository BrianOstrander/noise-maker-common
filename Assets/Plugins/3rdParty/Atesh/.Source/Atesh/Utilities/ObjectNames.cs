// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System.Collections.Generic;

namespace Atesh
{
    public static class ObjectNames
    {
        public static string NicifyVariableName(string Name)
        {
            var Offset = 0;

            // Remove optional m_, _ or k followed by uppercase letter in front of the name
            if (Name.Length > 2 && Name.StartsWith("m_")) Offset = 2;
            else if (Name.Length > 1 && (Name[0] == '_' || (char.IsUpper(Name[1]) && Name[0] == 'k'))) Offset = 1;

            var Result = new List<char>();
            for (var I = Offset; I < Name.Length; I++)
            {
                var Char = Name[I];
                Result.Add(Char);

                if (I < Name.Length - 1 &&
                    (
                    (char.IsLower(Char) && char.IsUpper(Name[I + 1]))
                    ||
                    (char.IsDigit(Char) && !char.IsDigit(Name[I + 1]))
                    ||
                    (!char.IsDigit(Char) && char.IsDigit(Name[I + 1]))
                    )
                   ) Result.Add(' ');
            }

            return new string(Result.ToArray());
        }
    }
}