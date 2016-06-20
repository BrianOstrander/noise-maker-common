using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public abstract class PropertyNode<T> : Node<T>, IPropertyNode
	{
		public bool IsEditable { get; set; }
		public T PropertyValue { get; set; }

		[JsonIgnore]
		public object RawPropertyValue 
		{ 
			get { return PropertyValue; } 
			set 
			{ 
				Debug.Log(typeof(T).FullName+" _ "+value.GetType().FullName);
				PropertyValue = (T)value; 
				Value = PropertyValue;
			}
		}

		public override T GetValue (List<INode> nodes)
		{
			Value = PropertyValue;
			return Value;
		}
	}
}