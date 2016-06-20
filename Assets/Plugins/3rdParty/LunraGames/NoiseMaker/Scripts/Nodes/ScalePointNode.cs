using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class ScalePointNode : Node<IModule>
	{
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source;
		[NodeLinker(1)]
		public Vector3 Scale = Vector3.one;

		public override IModule GetValue (List<INode> nodes)
		{
			var values = NullableValues(nodes);
			var source = GetLocalIfValueNull<IModule>(Source, 0, values);

			if (source == null) return null;

			var scale = GetLocalIfValueNull<Vector3>(Scale, 1, values);

			var scalePoint = Value == null ? new ScaleInput(source, scale.x, scale.y, scale.z) : Value as ScaleInput;

			scalePoint.SourceModule = source;

			scalePoint.X = scale.x;
			scalePoint.Y = scale.y;
			scalePoint.Z = scale.z;

			Value = scalePoint;
			return Value;
		}
	}
}