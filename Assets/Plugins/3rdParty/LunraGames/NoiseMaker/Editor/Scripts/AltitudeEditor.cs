using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public abstract class AltitudeEditor
	{
		public abstract Altitude Draw(Altitude altitude, ref bool changed);
	}
}