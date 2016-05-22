﻿using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using System;

namespace LunraGames.NoiseMaker
{
	public class NodeIo
	{
		public string Name;
		public string Tooltip;
		public bool Connecting;
		public bool Active;
		public Action OnClick;
	}
}