// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Reflection;

namespace Atesh
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class NotNullAttribute : ParameterValidationAttribute
    {
        public override void ValidateParameter(ParameterInfo Parameter, object Value)
        {
            if (Value == null) throw new ArgumentNullException(Parameter.Name);
        }
    }
}