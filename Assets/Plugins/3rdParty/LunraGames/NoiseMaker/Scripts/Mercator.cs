using System;
using System.Collections.Generic;
using LibNoise;
using Atesh;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

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
	}
}