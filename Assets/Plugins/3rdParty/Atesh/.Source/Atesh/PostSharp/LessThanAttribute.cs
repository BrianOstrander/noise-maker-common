// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Reflection;

namespace Atesh
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class LessThanAttribute : RangeValidationAttribute
    {
        public LessThanAttribute(int Value)
        {
            TestValue1 = Value;
        }

        public LessThanAttribute(long Value)
        {
            TestValue1 = Value;
        }

        public LessThanAttribute(float Value)
        {
            TestValue1 = Value;
        }

        public LessThanAttribute(double Value)
        {
            TestValue1 = Value;
        }

        public override void ValidateParameter(ParameterInfo Parameter, object Value)
        {
            var Throw = false;

            if (Value is int)
            {
                if ((int)Value >= (int)TestValue1) Throw = true;
            }
            else if (Value is long)
            {
                if ((long)Value >= (long)TestValue1) Throw = true;
            }
            else if (Value is float)
            {
                if ((float)Value >= (float)TestValue1) Throw = true;
            }
            else if (Value is double)
            {
                if ((double)Value >= (double)TestValue1) Throw = true;
            }
            else throw new NotImplementedException();

            if (Throw) throw new ArgumentOutOfRangeException(Parameter.Name, Value, Strings.ValueMustBeLessThan(TestValue1.ToString()));
        }
    }
}