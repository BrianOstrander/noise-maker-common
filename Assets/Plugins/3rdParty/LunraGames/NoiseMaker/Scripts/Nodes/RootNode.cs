using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class RootNode : Node<IModule>
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

			var root = Value == null ?  new TranslateInput(source, 0f, 0f ,0f) : Value as TranslateInput;

			root.SourceModule = source;

			Value = root;

			return Value;
		}
	}
}