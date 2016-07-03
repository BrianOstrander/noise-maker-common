using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LibNoise.Models;

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

		// todo: support more than one latitude.
		public Color GetSphereColor(float latitude, float longitude, float altitude)
		{
			if (Biomes == null || Biomes.FirstOrDefault() == null) return Color.white;

			var lat = Biomes.FirstOrDefault();
			return lat.GetColor(latitude, longitude, altitude, this);
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
					var index = (width * y) + x;
					colors[index] = GetSphereColor(lat, lon, value);
				}
			}
		}
	}
}