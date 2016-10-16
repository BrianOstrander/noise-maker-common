using System;
using LibNoise;
using UnityEngine;

namespace LunraGames.NoiseMaker.Modifiers
{
    public class CurveSimpleOutput : LibNoise.Math, IModule
    {
        public IModule SourceModule { get; set; }
        public AnimationCurve Curve = new AnimationCurve();

        public CurveSimpleOutput(IModule sourceModule, AnimationCurve curve)
        {
            if (sourceModule == null) throw new ArgumentNullException("A source module must be provided.");
            if (curve == null) throw new ArgumentNullException("An animation curve must be provided.");

            SourceModule = sourceModule;
            Curve = curve;
        }

        public double GetValue(double x, double y, double z)
        {
            if (SourceModule == null) throw new NullReferenceException("A source module must be provided.");
            if (Curve == null) throw new Exception("An animation curve must be specified.");

            // Get the output value from the source module.
            double sourceModuleValue = SourceModule.GetValue(x, y, z);

            return Curve.Evaluate((float)sourceModuleValue);
        }
    }
}
