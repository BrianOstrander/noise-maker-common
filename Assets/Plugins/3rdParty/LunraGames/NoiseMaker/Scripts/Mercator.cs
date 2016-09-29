using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LibNoise.Models;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	public class Mercator
	{
		public List<Domain> Domains = new List<Domain>();
		public List<Biome> Biomes = new List<Biome>();
		public List<Altitude> Altitudes = new List<Altitude>();

		public void Remove(Domain entry)
		{
			if (entry == null) throw new ArgumentNullException("entry");
			int? index = null;
			for (var i = 0; i < Domains.Count; i++)
			{
				if (Domains[i].Id == entry.Id) index = i;
				if (index.HasValue) break;
			}
			if (index.HasValue) Domains.RemoveAt(index.Value);
		}

		public void Remove(Biome entry)
		{
			if (entry == null) throw new ArgumentNullException("entry");
			int? index = null;
			for (var i = 0; i < Biomes.Count; i++)
			{
				if (Biomes[i].Id == entry.Id) index = i;
				if (index.HasValue) break;
			}

			if (index.HasValue) 
			{
				foreach (var domain in Domains) domain.BiomeId = domain.BiomeId == entry.Id ? null : domain.BiomeId;
				Biomes.RemoveAt(index.Value);
			}
		}

		public void Remove(Altitude entry)
		{
			if (entry == null) throw new ArgumentNullException("entry");
			int? index = null;
			for (var i = 0; i < Altitudes.Count; i++)
			{
				if (Altitudes[i].Id == entry.Id) index = i;
				if (index.HasValue) break;
			}

			if (index.HasValue) 
			{
				foreach (var biome in Biomes) biome.AltitudeIds.RemoveAll(a => a == entry.Id);
				Altitudes.RemoveAt(index.Value);
			}
		}

		Color GetColor(Func<Domain, float> weightRetriever, Func<Domain, Color> colorRetriever)
		{
			if (Domains == null || Domains.Count == 0 || Biomes == null || Biomes.Count == 0 || Altitudes == null || Altitudes.Count == 0) return Color.magenta;
			if (Domains.Count == 1) return colorRetriever(Domains.First());
			var orderedDomains = Domains.OrderByDescending(weightRetriever).ToArray();
			var first = orderedDomains[0];
			var firstWeight = weightRetriever(first);
			var firstColor = colorRetriever(first);

			if (Mathf.Approximately(firstWeight, 0f)) return firstColor;

			var second = orderedDomains[1];
			var secondWeight = weightRetriever(second);

			if (Mathf.Approximately(secondWeight, 0f)) return firstColor;

			var secondColor = colorRetriever(second);

			var scalar = 0.5f - (((firstWeight - secondWeight) / firstWeight) * 0.5f);

			return Color.Lerp(firstColor, secondColor, scalar);
		}

		public Color GetSphereColor(float latitude, float longitude, float altitude)
		{
			return GetColor(domain => domain.GetSphereWeight(latitude, longitude, altitude), domain => domain.GetSphereColor(latitude, longitude, altitude, this));
		}

		public void GetSphereColors(int width, int height, Sphere sphere, ref Color[] colors)
		{
			if (colors == null) throw new ArgumentNullException("colors");
			if (height * width != colors.Length) throw new ArgumentOutOfRangeException("colors");
			if (sphere == null) throw new ArgumentNullException("sphere");

			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var lat = SphereUtils.GetLatitude(y, height);
					var lon = SphereUtils.GetLongitude(x, width);
					var value = (float)sphere.GetValue((double)lat, (double)lon);
					colors[SphereUtils.PixelCoordinateToIndex(x, y, width, height)] = GetSphereColor(lat, lon, value);
				}
			}
		}

		public Color GetPlaneColor(float x, float y, float altitude)
		{
			return GetColor(domain => domain.GetPlaneWeight(x, y, altitude), domain => domain.GetPlaneColor(x, y, altitude, this));
		}

		public void GetPlaneColors(int width, int height, IModule module, ref Color[] colors)
		{
			if (colors == null) throw new ArgumentNullException("colors");
			if (height * width != colors.Length) throw new ArgumentOutOfRangeException("colors");

			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var value = (float)module.GetValue(x, y, 0f);
					colors[(y * width) + x] = GetPlaneColor(x, y, value);
				}
			}
		}
	}
}