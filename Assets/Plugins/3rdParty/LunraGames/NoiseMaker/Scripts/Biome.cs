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

			altitudes.OrderBy(a => Mathf.Abs(((altitude - a.MinAltitude) / (a.MaxAltitude - a.MinAltitude)) - 0.5f));

			var count = 0f;
			var scalars = new List<float>();
			var colors = new List<Color>();

			foreach (var curr in altitudes) 
			{
				var scalar = (altitude - curr.MinAltitude) / (curr.MaxAltitude - curr.MinAltitude); 
				scalars.Add(scalar);
				colors.Add(curr.GetColor(latitude, longitude));
				count++;
			}

			var currScalar = scalars.First();
			var currColor = colors.First();
			var index = 1;

			while (index < count)
			{
				var nextScalar = scalars[index];
				var nextColor = colors[index];

				var blend = (currScalar + nextScalar) / 2f;
				currColor = Color.Lerp(currColor, nextColor, blend);

				currScalar = nextScalar;
				index++;
			}

			return currColor;
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