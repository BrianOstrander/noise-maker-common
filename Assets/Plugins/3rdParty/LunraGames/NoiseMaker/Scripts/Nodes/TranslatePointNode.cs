using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Modifiers;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class TranslatePointNode : Node<IModule>
	{
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source;
		[NodeLinker(1)]
		public Vector3 Position = Vector3.zero;

		public override IModule GetValue (List<INode> nodes)
		{
			var values = NullableValues(nodes);
			var source = GetLocalIfValueNull<IModule>(Source, 0, values);

			if (source == null) return null;

			var position = GetLocalIfValueNull<Vector3>(Position, 1, values);

			var translatePoint = Value == null ? new TranslateInput(source, position.x, position.y, position.z) : Value as TranslateInput;

			translatePoint.SourceModule = source;

			translatePoint.X = position.x;
			translatePoint.Y = position.y;
			translatePoint.Z = position.z;

			Value = translatePoint;
			return Value;
		}
	}
}