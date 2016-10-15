using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class RotatePointNode : Node<IModule>
	{
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source;
		[NodeLinker(1)]
		public Vector3 Rotation = Vector3.zero;

		public override IModule GetValue (Graph graph)
		{
			var values = NullableValues(graph);
			var source = GetLocalIfValueNull<IModule>(Source, 0, values);

			if (source == null) return null;

			var rotation = GetLocalIfValueNull<Vector3>(Rotation, 1, values);

			var rotatePoint = Value == null ? new RotateInput(source, rotation.x, rotation.y, rotation.z) : Value as RotateInput;

			rotatePoint.SourceModule = source;

			rotatePoint.SetAngles(rotation.x, rotation.y, rotation.z);

			Value = rotatePoint;
			return Value;
		}
	}
}