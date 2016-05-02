using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public abstract class NodeEditor
	{
		public const int PreviewSize = 198;
		const float IoStartOffset = 16;
		const float IoDivider = 8;
		const float IoWidth = 32;
		const float IoHeight = 16;

		static Vector2 HueRange = Vector2.zero;
		static Vector2 SaturationRange = Vector2.zero;
		static Vector2 ValueRange = new Vector2(-2f, 4f);

		static VisualizationPreview Visualization(string name, float hueMin = 0f, float hueMax = 0f, float saturationMin = 0f, float saturationMax = 0f, float valueMin = 0f, float valueMax = 0f)
		{
			var visualization = new VisualizationPreview();
			visualization.Name = name;
			visualization.Activate = () => NodeEditor.RangeVisualization(hueMin, hueMax, saturationMin, saturationMax, valueMin, valueMax);

			visualization.LowestCutoff = hueMin;
			visualization.HighestCutoff = hueMin;
			foreach (var curr in new float[] { hueMax, saturationMin, saturationMax, valueMin, valueMax})
			{
				visualization.LowestCutoff = Mathf.Min(visualization.LowestCutoff, curr);
				visualization.HighestCutoff = Mathf.Max(visualization.HighestCutoff, curr);
			}

			visualization.Preview = VisualizationPreview(visualization.LowestCutoff, visualization.HighestCutoff, hueMin, hueMax, saturationMin, saturationMax, valueMin, valueMax);
			return visualization;
		}

		static List<VisualizationPreview> _Visualizations;

		public static List<VisualizationPreview> Visualizations
		{
			get
			{
				if (_Visualizations == null)
				{
					_Visualizations = new List<VisualizationPreview>();
					_Visualizations.Add(Visualization("Grayscale", valueMin: -2f, valueMax: 2f));
					_Visualizations.Add(Visualization("Spectrum", -2f, 2f, 1f, 1f, 1f, 1f));
				}

				return _Visualizations;
			}
		}

		static void RangeVisualization(float hueMin = 0f, float hueMax = 0f, float saturationMin = 0f, float saturationMax = 0f, float valueMin = 0f, float valueMax = 0f)
		{
			HueRange = new Vector2(hueMin, hueMax - hueMin);
			SaturationRange = new Vector2(saturationMin, saturationMax - saturationMin);
			ValueRange = new Vector2(valueMin, valueMax - valueMin);
		}

		static Texture2D VisualizationPreview(float lowest, float highest, float hueMin = 0f, float hueMax = 0f, float saturationMin = 0f, float saturationMax = 0f, float valueMin = 0f, float valueMax = 0f)
		{
			var hueRange = new Vector2(hueMin, hueMax - hueMin);
			var saturationRange = new Vector2(saturationMin, saturationMax - saturationMin);
			var valueRange = new Vector2(valueMin, valueMax - valueMin);

			var preview = new Texture2D(PreviewSize, 24);
			for (var x = 0; x < preview.width; x++)
			{
				for (var y = 0; y < preview.height; y++)
				{
					var value = lowest + (((highest - lowest) * ((float)x / (float)preview.width)));
					var color = Color.HSVToRGB(NormalizedRange(value, hueRange), NormalizedRange(value, saturationRange), NormalizedRange(value, valueRange));
					preview.SetPixel(x, y, color);
				}
			}
			preview.Apply();
			return preview;
		}

		static float NormalizedRange(float value, Vector2 range)
		{
			if (Mathf.Approximately(range.y, 0f)) return range.x;
			return (value - range.x) / range.y;
		}

		protected static List<NodePreview> Previews = new List<NodePreview>();

		protected NodePreview GetPreview(Graph graph, Node node)
		{
			var preview = Previews.FirstOrDefault(p => p.Id == node.Id);

			if (preview != null)
			{
				preview.Stale = preview.Stale || node.SourceIds.Count != preview.LastSourceIds.Count;
				for (var i = 0; i < node.SourceIds.Count; i++)
				{
					var id = node.SourceIds[i];
					preview.Stale = preview.Stale || id != preview.LastSourceIds[i];
					if (StringExtensions.IsNullOrWhiteSpace(id)) continue;
					var sourcePreview = Previews.FirstOrDefault(p => p.Id == id);
					if (sourcePreview == null) continue;
					preview.Stale = preview.Stale || preview.LastUpdated < sourcePreview.LastUpdated;
				}
			}

			if (preview == null || preview.Stale)
			{
				if (preview == null)
				{
					preview = new NodePreview { Id = node.Id };
					preview.Preview = new Texture2D(PreviewSize, PreviewSize);
					Previews.Add(preview);
				}

				var module = node.GetModule(graph.Nodes);

				for (var x = 0; x < preview.Preview.width; x++)
				{
					for (var y = 0; y < preview.Preview.height; y++)
					{
						var value = (float)module.GetValue((double)x, (double)y, 0.0);
						var color = Color.HSVToRGB(NormalizedRange(value, HueRange), NormalizedRange(value, SaturationRange), NormalizedRange(value, ValueRange));
						preview.Preview.SetPixel(x, y, color);
					}
				}
				preview.Preview.Apply();
				preview.Stale = false;
				preview.LastUpdated = DateTime.Now.Ticks;
				preview.LastSourceIds = new List<string>(node.SourceIds);
			}
			return preview;
		}

		public List<Rect> DrawInputs(Rect position, params NodeIo[] inputs)
		{
			var currRect = new Rect(position.x - IoWidth + 1, position.y + IoStartOffset, IoWidth, IoHeight);
			var rects = new List<Rect>();
			foreach (var input in inputs)
			{
				rects.Add(new Rect(currRect));
				if (GUI.Button(currRect, input.Active ? new GUIContent("x") : GUIContent.none, Styles.BoxButton)) input.OnClick();
				currRect.y += IoDivider + IoHeight;
			}
			return rects;
		}

		public Rect DrawOutput(Rect position, NodeIo output)
		{
			var currRect = new Rect(position.x + position.width - 2, position.y + IoStartOffset, IoWidth, IoHeight);
			if (GUI.Button(currRect, GUIContent.none, output.Connecting ? Styles.BoxButtonHovered : Styles.BoxButton)) output.OnClick();
			return currRect;
		}

		public abstract Node Draw(Graph graph, Node node);
	}
}