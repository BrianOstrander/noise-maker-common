using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LunraGames.NoiseMaker
{
	[Serializable]
	public class Biome
	{
		public string Id;
		public string Name;
		/// <summary>
		/// The id's of associated Altitudes.
		/// </summary>
		public List<string> AltitudeIds = new List<string>();

		List<Altitude> _Altitudes;
		List<Altitude> GetAltitudes(Mercator mercator)
		{
			if (_Altitudes == null) _Altitudes = mercator.Altitudes.FindAll(a => AltitudeIds.Contains(a.Id));
			return _Altitudes;
		}


		public Color GetColor(float latitude, float longitude, float altitude, Mercator mercator)
		{
			var allAltitudes = GetAltitudes(mercator);
			var altitudes = allAltitudes.Where(a => a.MinAltitude <= altitude && altitude <= a.MaxAltitude).ToList();

			if (altitudes.Count == 0)
			{
				var lowest = GetLowest(mercator);

				if (lowest == null) return Color.magenta;
				else if (altitude < lowest.MinAltitude) return lowest.GetColor(latitude, longitude);
				else return GetHighest(mercator).GetColor(latitude, longitude);
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

		Altitude GetHighest(Mercator mercator)
		{
			var allAltitudes = GetAltitudes(mercator);

			Altitude highest = null;
			foreach (var altitude in allAltitudes)
			{	
				var unmodifiedAltitude = altitude;
				if (highest == null || highest.MaxAltitude <= unmodifiedAltitude.MaxAltitude) highest = unmodifiedAltitude;
			}
			return highest;
		}

		Altitude GetLowest(Mercator mercator)
		{
			var allAltitudes = GetAltitudes(mercator);

			Altitude lowest = null;
			foreach (var altitude in allAltitudes)
			{
				var unmodifiedAltitude = altitude;
				if (lowest == null || unmodifiedAltitude.MinAltitude <= lowest.MinAltitude) lowest = unmodifiedAltitude;
			}
			return lowest;
		}
	}
}