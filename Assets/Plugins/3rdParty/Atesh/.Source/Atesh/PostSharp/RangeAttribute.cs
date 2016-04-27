// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Reflection;

namespace Atesh
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RangeAttribute : RangeValidationAttribute
    {
        public RangeAttribute(int Min, int Max)
        {
            TestValue1 = Min;
            TestValue2 = Max;
        }

        public RangeAttribute(long Min, long Max)
        {
            TestValue1 = Min;
            TestValue2 = Max;
        }

        public RangeAttribute(float Min, float Max)
        {
            TestValue1 = Min;
            TestValue2 = Max;
        }

        public RangeAttribute(double Min, double Max)
        {
            TestValue1 = Min;
            TestValue2 = Max;
        }

        public override void ValidateParameter(ParameterInfo Parameter, object Value)
        {
            var Throw = false;

            if (Value is int)
            {
                if ((int)Value < (int)TestValue1 || (int)Value > (int)TestValue2) Throw = true;
            }
            else if (Value is long)
            {
                if ((long)Value <= (long)TestValue1 || (long)Value > (long)TestValue2) Throw = true;
            }
            else if (Value is float)
            {
                if ((float)Value <= (float)TestValue1 || (float)Value > (float)TestValue2) Throw = true;
            }
            else if (Value is double)
            {
                if ((double)Value <= (double)TestValue1 || (double)Value > (double)TestValue2) Throw = true;
            }
            else throw new NotImplementedException();

            if (Throw) throw new ArgumentOutOfRangeException(Parameter.Name, Value, Strings.ValueMustBeBetween(TestValue1.ToString(), TestValue2.ToString()));
        }
    }
}