using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class NoiseGraph : ScriptableObject 
	{
		public string GraphJson;
		public string PropertiesJson;

		/// <summary>
		/// Retrieves a new copy of the serialized graph.
		/// </summary>
		/// <value>The graph.</value>
		public Graph GraphInstantiation 
		{
			get 
			{
				return Serialization.DeserializeJson<Graph>(GraphJson, verbose: true);
			}
			set
			{
				GraphJson = Serialization.SerializeJson(value, true);
			}
		}

		/// <summary>
		/// Retrieves a new copy of the serialized properties.
		/// </summary>
		/// <value>The properties.</value>
		public List<Property> PropertiesInstantiation
		{
			get 
			{
				return Serialization.DeserializeJson<List<Property>>(PropertiesJson, verbose: true);
			}
			set
			{
				PropertiesJson = Serialization.SerializeJson(value, true);
			}
		}
	}
}