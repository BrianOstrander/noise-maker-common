using System;

namespace LunraGames.NoiseMaker
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class AltitudeDrawer : Attribute
	{
		public Type Target;
		public string Category;
		public string Name;

		public AltitudeDrawer(Type target, string category, string name)
		{
			Target = target;
			Category = category;
			Name = name;
		}
	}
}