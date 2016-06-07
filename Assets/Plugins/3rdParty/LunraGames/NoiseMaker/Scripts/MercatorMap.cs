using UnityEngine;
using System;
using System.Collections;

namespace LunraGames.NoiseMaker
{
	public class MercatorMap : ScriptableObject 
	{
		public string MercatorJson;

		/// <summary>
		/// Retrieves a new copy of the serialized mercator.
		/// </summary>
		/// <value>The graph.</value>
		public Mercator MercatorInstantiation 
		{
			get 
			{
				return Serialization.DeserializeJson<Mercator>(MercatorJson);
			}
			set
			{
				MercatorJson = Serialization.SerializeJson(value);
			}
		}
	}
}