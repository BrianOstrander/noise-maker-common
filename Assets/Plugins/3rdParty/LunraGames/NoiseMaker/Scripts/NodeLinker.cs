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
		public object Min;
		public object Max;

		public NodeLinker(int index, object min = null, object max = null, bool hide = false, string name = null)
		{
			Index = index;
			Min = min;
			Max = max;
			Name = name;
			Hide = hide;
		}
	}
}