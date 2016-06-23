using UnityEngine;
using System;

namespace LunraGames.NoiseMaker
{
	public class VisualizationPreview 
	{
		public string Name;
		public NodeEditor.CalculateColor Calculate;
		public float LowestCutoff;
		public float HighestCutoff;
		public float HueMin;
		public float HueMax;
		public float SaturationMin;
		public float SaturationMax;
		public float ValueMin;
		public float ValueMax;

		public float CutoffRange { get { return HighestCutoff - LowestCutoff; } }
		public float HueRange { get { return HueMax - HueMin; } }
		public float SaturationRange { get { return SaturationMax - SaturationMin; } }
		public float ValueRange { get { return ValueMax - ValueMin; } }

		Texture2D _Preview;

		public Texture2D Preview
		{
			get
			{
				if (_Preview == null)
				{
					_Preview = new Texture2D((int)(NoiseMakerWindow.Layouts.VisualizationOptionsWidth * 0.9f), 24);
					for (var x = 0; x < _Preview.width; x++)
					{
						for (var y = 0; y < _Preview.height; y++)
						{
							var value = LowestCutoff + (((float)x / (float)_Preview.width) * CutoffRange);
							var color = Calculate(value, this);
							_Preview.SetPixel(x, y, color);
						}
					}
					_Preview.Apply();
				}

				return _Preview;
			}
		}
	}
}