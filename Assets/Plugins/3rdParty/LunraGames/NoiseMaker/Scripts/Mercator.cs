﻿using System;
using System.Collections.Generic;
using LibNoise;
using Atesh;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;
using LibNoise.Models;

namespace LunraGames.NoiseMaker
{
	public class Mercator
	{
		public List<Latitude> Latitudes = new List<Latitude>();

		public void Remove(Latitude entry)
		{
			if (entry == null) throw new ArgumentNullException("entry");
			int? index = null;
			for (var i = 0; i < Latitudes.Count; i++)
			{
				if (Latitudes[i].Id == entry.Id) index = i;
				if (index.HasValue) break;
			}
			if (index.HasValue) Latitudes.RemoveAt(index.Value);
		}

		// todo: support more than one latitude.
		public Color GetColor(float latitude, float longitude, float altitude)
		{
			if (Latitudes == null || Latitudes.FirstOrDefault() == null) return Color.white;

			var lat = Latitudes.FirstOrDefault();
			return lat.GetColor(latitude, longitude, altitude);
		}

		public void GetColors(int height, int width, Sphere sphere, ref Color[] colors)
		{
			if (colors == null) throw new ArgumentNullException("colors");
			if (height * width != colors.Length) throw new ArgumentOutOfRangeException("colors");

			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var lat = SphereUtils.GetLatitude(y, height);
					var lon = SphereUtils.GetLongitude(x, width);
					var value = (float)sphere.GetValue((double)lat, (double)lon);
					var index = (width * y) + x;
					colors[index] = GetColor(lat, lon, value);
				}
			}
		}
	}
}