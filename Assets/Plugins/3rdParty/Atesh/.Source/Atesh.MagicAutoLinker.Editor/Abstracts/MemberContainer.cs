// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System.Collections.Generic;
using System.Reflection;

namespace Atesh.MagicAutoLinker.Editor
{
    abstract class MemberContainer
    {
        protected readonly List<MemberNode> Members = new List<MemberNode>();

        internal MemberNode NewMember(MemberInfo Member)
        {
            var Result = new MemberNode { Name = Member?.Name };
            Members.Add(Result);
            return Result;
        }
    }
}