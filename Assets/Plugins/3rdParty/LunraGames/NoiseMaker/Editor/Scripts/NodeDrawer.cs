using System;

namespace LunraGames.NoiseMaker
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class NodeDrawer : Attribute
	{
		public Type Target;
		public string Category;
		public string Name;

		public NodeDrawer(Type target, string category, string name)
		{
			Target = target;
			Category = category;
			Name = name;
		}
	}
}