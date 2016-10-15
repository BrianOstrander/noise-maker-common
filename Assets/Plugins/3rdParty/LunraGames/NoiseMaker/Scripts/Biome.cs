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

		Color GetColor(Func<Altitude, Color> colorRetriever, float altitude, Mercator mercator)
		{
			var altitudes = mercator.Altitudes.Where (a => AltitudeIds.Contains (a.Id)).ToList();

			if (altitudes.Count == 0) return Color.magenta;
			if (altitudes.Count == 1) return colorRetriever(altitudes[0]);

			altitudes.OrderBy(a => Mathf.Abs(((altitude - a.MinAltitude) / (a.MaxAltitude - a.MinAltitude)) - 0.5f));

			var count = 0f;
			var scalars = new List<float>();
			var colors = new List<Color>();

			foreach (var curr in altitudes) 
			{
				var scalar = (altitude - curr.MinAltitude) / (curr.MaxAltitude - curr.MinAltitude); 
				scalars.Add(scalar);
				colors.Add(colorRetriever(curr));
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

		public Color GetSphereColor(float latitude, float longitude, float altitude, Mercator mercator)
		{
			return GetColor(alt => alt.GetSphereColor(latitude, longitude), altitude, mercator);
		}

		public Color GetPlaneColor(float x, float y, float altitude, Mercator mercator)
		{
			return GetColor(alt => alt.GetPlaneColor(x, y), altitude, mercator);
		}
	}
}