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

		public Color GetColor(float latitude, float longitude, float altitude, Mercator mercator)
		{
			//var allAltitudes = GetAltitudes(mercator);
			// todo: change this to actually get the bioms altitudes, not every one that falls somewhere
			//var altitudes = allAltitudes.Where(a => a.MinAltitude <= altitude && altitude <= a.MaxAltitude).ToList();
			var altitudes = mercator.Altitudes.Where (a => AltitudeIds.Contains (a.Id)).ToList();

			if (altitudes.Count == 0) return Color.magenta;
			if (altitudes.Count == 1) return altitudes[0].GetColor(latitude, longitude);

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
	}
}