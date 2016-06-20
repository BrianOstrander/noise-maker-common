using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class NoiseGraph : ScriptableObject 
	{
		public string GraphJson;
		public List<Property> Properties = new List<Property>();

		/// <summary>
		/// Retrieves a new copy of the serialized graph.
		/// </summary>
		/// <value>The graph.</value>
		public Graph GraphInstantiation 
		{
			get 
			{
				return Serialization.DeserializeJson<Graph>(GraphJson);
			}
			set
			{
				GraphJson = Serialization.SerializeJson(value);
			}
		}
	}
}