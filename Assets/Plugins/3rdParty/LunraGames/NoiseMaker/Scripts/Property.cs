using System;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace LunraGames.NoiseMaker
{
	[Serializable]
	public class Property
	{
		public string Name;
		public string Id;
		[JsonProperty]
		object _Value;
		// todo: remove this hack, when I have the patience...
		[JsonProperty]
		Vector3 ValueVector3;
		[JsonProperty]
		Type Type;

		[JsonIgnore]
		public object Value
		{
			get 
			{
				if (_Value == null) return null;

				// hack to fix newtonsoft defaulting objects to doubles.
				if (_Value is double) _Value = Convert.ToSingle((double)_Value);
				else if (_Value is long) _Value = Convert.ToInt32((long)_Value);

				if (typeof(Enum).IsAssignableFrom(Type)) _Value = _Value is Enum ? _Value : Enum.Parse(Type, Enum.GetNames(Type)[(int)_Value]);
				else if (Type == typeof(Vector3)) _Value = ValueVector3;

				return _Value;
			}
			set
			{
				_Value = value;
				Type = value == null ? null : value.GetType();
				if (Type == typeof(Vector3)) ValueVector3 = (Vector3)value;
			}
		}
	}
}