using System;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	[Serializable]
	public class Property 
	{
		public string Name;
		public string Id;
		[JsonProperty]
		object _Value;
		[JsonIgnore]
		public object Value
		{
			get 
			{
				if (_Value == null) return null;

				if (_Value is double) _Value = Convert.ToSingle((double)_Value);

				return _Value;
			}
			set
			{
				_Value = value;
			}
		}
	}
}