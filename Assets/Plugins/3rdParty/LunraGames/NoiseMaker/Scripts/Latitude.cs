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
	}
}