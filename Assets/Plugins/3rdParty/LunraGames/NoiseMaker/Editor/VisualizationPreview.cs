using UnityEngine;
using System;

namespace LunraGames.NoiseMaker
{
	public class VisualizationPreview 
	{
		//public delegate Color CalculateColor(float height, float minHeight, float maxHeight, float hueMin = 0f, float hueMax = 0f, float saturationMin = 0f, float saturationMax = 0f, float valueMin = 0f, float valueMax = 0f);

		public string Name;
		public Texture2D Preview;
		public Action Activate;
		//public CalculateColor Calculate;
		public float LowestCutoff;
		public float HighestCutoff;
	}
}