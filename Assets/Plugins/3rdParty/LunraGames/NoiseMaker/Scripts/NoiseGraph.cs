using UnityEngine;
using System;
using System.Collections;

namespace LunraGames.NoiseMaker
{
	public class NoiseGraph : ScriptableObject 
	{
		public string GraphJson;

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