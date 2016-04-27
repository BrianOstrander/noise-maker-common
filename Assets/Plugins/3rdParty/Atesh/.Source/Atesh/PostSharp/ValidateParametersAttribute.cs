// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Linq;
using System.Reflection;
using PostSharp;
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace Atesh
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public class ValidateParametersAttribute : OnMethodBoundaryAspect
    {
        ParameterInfo[] Parameters;
        ParameterValidationAttribute[][] ParameterAttributes;
        
        public override void CompileTimeInitialize(MethodBase Method, AspectInfo AspectInfo)
        {  
            base.CompileTimeInitialize(Method, AspectInfo);

            Parameters = Method.GetParameters();
            ParameterAttributes = new ParameterValidationAttribute[Parameters.Length][];
            
            var AttributeFound = false;
            for (var I = 0; I < Parameters.Length; I++)
            {
                var Parameter = Parameters[I];
                var ValidationAttributes = Parameter.GetCustomAttributes(false).OfType<ParameterValidationAttribute>().ToArray();
                if (ValidationAttributes.Length > 0) AttributeFound = true;

                foreach (var ValidationAttribute in ValidationAttributes)
                {
                    ValidationAttribute.ValidateUsage(Parameter);
                }

                ParameterAttributes[I] = ValidationAttributes;
            }

            if (!AttributeFound)
            {
                Message.Write(MessageLocation.Of(Method), SeverityType.Fatal, "AE001", Strings.MethodDoesntHaveValidatedParameter($"{Method.DeclaringType.Name}.{Method.Name}"));
                throw new InvalidAnnotationException();
            }
        }
         
        public override void OnEntry(MethodExecutionArgs Args)
        {
            for (var I = 0; I < Args.Arguments.Count; I++)
            {
                if (ParameterAttributes[I].Length == 0) continue;

                foreach (var ValidationAttribute in ParameterAttributes[I])
                {
                    ValidationAttribute.ValidateParameter(Parameters[I], Args.Arguments[I]);
                }
            }

            base.OnEntry(Args);
        }
    }
}