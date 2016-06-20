using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Modifiers;
using System.Collections.Generic;
using Atesh;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class DisplaceNode : Node<IModule>
	{
		/// <summary>
		/// The source used if SourceIds[0] is null.
		/// </summary>
		[NodeLinker(0, hide: true), JsonIgnore]
		public IModule Source;
		/// <summary>
		/// The source used if SourceIds[1] is null.
		/// </summary>
		[NodeLinker(1, hide: true), JsonIgnore]
		public IModule XDisplacement;
		/// <summary>
		/// The source used if SourceIds[2] is null.
		/// </summary>
		[NodeLinker(2, hide: true), JsonIgnore]
		public IModule YDisplacement;
		/// <summary>
		/// The source used if SourceIds[3] is null.
		/// </summary>
		[NodeLinker(3, hide: true), JsonIgnore]
		public IModule ZDisplacement;

		public override IModule GetValue (List<INode> nodes)
		{
			var values = NullableValues(nodes);

			var source = GetLocalIfValueNull<IModule>(Source, 0, values);
			var xDisplacement = GetLocalIfValueNull<IModule>(XDisplacement, 1, values);
			var yDisplacement = GetLocalIfValueNull<IModule>(YDisplacement, 2, values);
			var zDisplacement = GetLocalIfValueNull<IModule>(ZDisplacement, 3, values);

			if (source == null || xDisplacement == null || yDisplacement == null || zDisplacement == null) return null;

			var displace = Value == null ? new DisplaceInput(source, xDisplacement, yDisplacement, zDisplacement) : Value as DisplaceInput;

			displace.SourceModule = source;
			displace.XDisplaceModule = xDisplacement;
			displace.YDisplaceModule = yDisplacement;
			displace.ZDisplaceModule = zDisplacement;

			Value = displace;

			return Value;
		}
	}
}