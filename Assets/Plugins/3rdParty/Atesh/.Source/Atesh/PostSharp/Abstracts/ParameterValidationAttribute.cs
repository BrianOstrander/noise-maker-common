// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Reflection;

namespace Atesh
{
    [Serializable]
    public abstract class ParameterValidationAttribute : Attribute
    {
        public abstract void ValidateParameter(ParameterInfo Parameter, object Value);
        // ReSharper disable once UnusedParameter.Global
        public virtual void ValidateUsage(ParameterInfo Parameter) { }
    }
}