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
				PropertyValue = (T)value; 
				Value = PropertyValue;
			}
		}

		public override T GetValue (Graph graph)
		{
			Value = PropertyValue;
			return Value;
		}

		[JsonIgnore]
		protected virtual T DefaultValue { get { return default(T); } }

		public PropertyNode() 
		{
			// This may give you a warning, but it should be safe... since it's just getting a default value.
			PropertyValue = DefaultValue;
		}
	}
}