using UnityEngine;
using System;

namespace LunraGames.NoiseMaker
{
	public class Connection<T>
	{
		public string Name;
		public Action<T> OnChange;
	}
}