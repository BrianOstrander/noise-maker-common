// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;

namespace Atesh.MagicAutoLinker
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AutoLinkSelfAttribute : Attribute
    {
        // ReSharper disable ConvertToConstant.Global
        // ReSharper disable FieldCanBeMadeReadOnly.Global
        public string Description;
        // ReSharper restore FieldCanBeMadeReadOnly.Global
        // ReSharper restore ConvertToConstant.Global
    }
}