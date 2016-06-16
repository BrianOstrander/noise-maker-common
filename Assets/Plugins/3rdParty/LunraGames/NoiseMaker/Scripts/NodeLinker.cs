using System;
using System.Reflection;

namespace LunraGames.NoiseMaker
{
	[AttributeUsage(AttributeTargets.Field)]
	public class NodeLinker : Attribute
	{
		public int Index;
		public string Name;
		public Type Type;
		public FieldInfo Field;
		public bool Hide;

		public NodeLinker(int index, bool hide = false, string name = null)
		{
			Index = index;
			Name = name;
			Hide = hide;
		}
	}
}