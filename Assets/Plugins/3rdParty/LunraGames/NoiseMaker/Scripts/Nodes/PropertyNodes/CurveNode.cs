using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public class CurveNode : PropertyNode<AnimationCurve> 
	{
		protected override AnimationCurve DefaultValue { get { return new AnimationCurve(); } }
	}
}