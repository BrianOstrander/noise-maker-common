// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Reflection;
using PostSharp;
using PostSharp.Extensibility;

namespace Atesh
{
    [Serializable]
    public abstract class RangeValidationAttribute : ParameterValidationAttribute
    {
        protected object TestValue1;
        protected object TestValue2;

        public override void ValidateUsage(ParameterInfo Parameter)
        {
            if (Parameter.ParameterType != TestValue1.GetType())
            {
                Message.Write(MessageLocation.Of(Parameter), SeverityType.Fatal, "AE002", Strings.TypeMismatchBetweenParameterAndValidation(Parameter.Name, GetType().Name));
                throw new InvalidAnnotationException();
            }
        }

        public abstract override void ValidateParameter(ParameterInfo Parameter, object Value);
    }
}