using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public class Visualizers
	{
		public static Color Grayscale(float value, VisualizationPreview previewer)
		{
			var normal = NormalizedRange(value, previewer.LowestCutoff, previewer.CutoffRange);
			return Color.HSVToRGB(0f, 0f, previewer.ValueMin + (normal * previewer.ValueRange));
		}

		public static Color Spectrum(float value, VisualizationPreview previewer)
		{
			var normal = NormalizedRange(value, previewer.LowestCutoff, previewer.CutoffRange);
			return Color.HSVToRGB(previewer.HueMin + (normal * previewer.HueRange), 1f, 1f);
		}

		static float NormalizedRange(float value, float min, float range)
		{
			return (value - min) / range;
		}
	}
}