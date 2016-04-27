// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Reflection;

namespace Atesh
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class NotLessThanAttribute : RangeValidationAttribute
    {
        public NotLessThanAttribute(int Value)
        {
            TestValue1 = Value;
        }

        public NotLessThanAttribute(long Value)
        {
            TestValue1 = Value;
        }

        public NotLessThanAttribute(float Value)
        {
            TestValue1 = Value;
        }

        public NotLessThanAttribute(double Value)
        {
            TestValue1 = Value;
        }

        public override void ValidateParameter(ParameterInfo Parameter, object Value)
        {
            var Throw = false;

            if (Value is int)
            {
                if ((int)Value < (int)TestValue1) Throw = true;
            }
            else if (Value is long)
            {
                if ((long)Value < (long)TestValue1) Throw = true;
            }
            else if (Value is float)
            {
                if ((float)Value < (float)TestValue1) Throw = true;
            }
            else if (Value is double)
            {
                if ((double)Value < (double)TestValue1) Throw = true;
            }
            else throw new NotImplementedException();

            if (Throw) throw new ArgumentOutOfRangeException(Parameter.Name, Value, Strings.ValueCantBeLessThan(TestValue1.ToString()));
        }
    }
}