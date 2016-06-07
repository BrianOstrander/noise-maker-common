using System;
using UnityEngine;
using System.Collections.Generic;
using Atesh;
using LibNoise;
using System.Linq;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	[Serializable]
	public class Latitude
	{
		public string Id;
		public float MinLatitude;
		public float MaxLatitude;

		public List<Altitude> Altitudes = new List<Altitude>();

		public void Remove(Altitude entry)
		{
			if (entry == null) throw new ArgumentNullException("entry");
			int? index = null;
			for (var i = 0; i < Altitudes.Count; i++)
			{
				if (Altitudes[i].Id == entry.Id) index = i;
				if (index.HasValue) break;
			}
			if (index.HasValue) Altitudes.RemoveAt(index.Value);
		}

		public Color GetColor(float latitude, float longitude, float altitude)
		{
			if (Altitudes == null || Altitudes.FirstOrDefault() == null) return Color.white;

			var altitudes = Altitudes.Where(a => a.MinAltitude <= altitude && altitude <= a.MaxAltitude).ToList();

			if (altitudes.Count == 0)
			{
				var lowest = GetLowest();

				if (altitude < lowest.MinAltitude) return lowest.GetColor(latitude, longitude);
				else return GetHighest().GetColor(latitude, longitude);
			}
			else if (altitudes.Count == 1) return altitudes[0].GetColor(latitude, longitude);

			var firstAlt = altitudes[0];
			var lastAlt = altitudes[1];

			var delta = firstAlt.MaxAltitude - lastAlt.MinAltitude;

			if (Mathf.Approximately(delta, 0f)) return lastAlt.GetColor(latitude, longitude);

			var deltaProgress = altitude - lastAlt.MinAltitude;
			var scalar = deltaProgress / delta;

			return Color.Lerp(firstAlt.GetColor(latitude, longitude), lastAlt.GetColor(latitude, longitude), scalar);
		}

		Altitude GetHighest()
		{
			if (Altitudes == null) return null;

			Altitude highest = null;
			foreach (var altitude in Altitudes)
			{	
				var unmodifiedAltitude = altitude;
				if (highest == null || highest.MaxAltitude <= unmodifiedAltitude.MaxAltitude) highest = unmodifiedAltitude;
			}
			return highest;
		}

		Altitude GetLowest()
		{
			if (Altitudes == null) return null;

			Altitude lowest = null;
			foreach (var altitude in Altitudes)
			{
				var unmodifiedAltitude = altitude;
				if (lowest == null || unmodifiedAltitude.MinAltitude <= lowest.MinAltitude) lowest = unmodifiedAltitude;
			}
			return lowest;
		}
	}
}