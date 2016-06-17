using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class AbsoluteNode : Node<IModule>
	{
		/// <summary>
		/// The source used if SourceIds[0] is null.
		/// </summary>
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source;

		public override IModule GetValue (List<INode> nodes)
		{
			var values = NullableValues(nodes);

			var source = GetLocalIfValueNull<IModule>(Source, 0, values);

			if (source == null) return null;

			var absolute = Value == null ?  new AbsoluteOutput(source) : Value as AbsoluteOutput;

			absolute.SourceModule = source;

			Value = absolute;

			return Value;
		}
	}
}