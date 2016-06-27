using System;

namespace LunraGames.NoiseMaker
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class DomainDrawer : Attribute
	{
		public Type Target;
		public string Name;

		public DomainDrawer(Type target, string name)
		{
			Target = target;
			Name = name;
		}
	}
}