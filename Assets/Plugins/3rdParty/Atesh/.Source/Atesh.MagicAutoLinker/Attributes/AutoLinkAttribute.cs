// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;

namespace Atesh.MagicAutoLinker
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AutoLinkAttribute : Attribute
    {
        // ReSharper disable ConvertToConstant.Global
        // ReSharper disable FieldCanBeMadeReadOnly.Global
        public string GameObjectName;
        public string CollectionElementName;
        public int NumberOfDigitsForCollectionElement = 1;
        public int IndexOffsetForCollectionElement = 1;
        public string Description;
        // ReSharper restore FieldCanBeMadeReadOnly.Global
        // ReSharper restore ConvertToConstant.Global
    }
}