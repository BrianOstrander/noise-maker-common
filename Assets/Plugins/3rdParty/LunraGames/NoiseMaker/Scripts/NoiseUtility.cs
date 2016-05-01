using System;
using UnityEngine;
using System.Collections.Generic;
using Atesh;
using LibNoise;
using System.Linq;
using Newtonsoft.Json;

namespace LunraGames.NoiseMaker
{
	public class NoiseUtility
	{
		public static int Seed { get { return UnityEngine.Random.Range(int.MinValue, int.MaxValue); } }
	}
}