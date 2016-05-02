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

		public delegate Color CalculateColor(float value, VisualizationPreview previewer);

		public static VisualizationPreview Previewer;

		static List<VisualizationPreview> _Visualizations;

		public static List<VisualizationPreview> Visualizations
		{
			get
			{
				if (_Visualizations == null)
				{
					_Visualizations = new List<VisualizationPreview>();
					_Visualizations.Add(new VisualizationPreview 
					{
						Name = "Grayscale",
						Calculate = Visualizers.Grayscale,
						LowestCutoff = -2f,
						HighestCutoff = 2f,
						ValueMin = 0f,
						ValueMax = 1f
					});
					_Visualizations.Add(new VisualizationPreview 
					{
						Name = "Spectrum",
						Calculate = Visualizers.Spectrum,
						LowestCutoff = -2f,
						HighestCutoff = 2f,
						HueMin = 0f,
						HueMax = 1f
					});
					_Visualizations.Add(new VisualizationPreview 
					{
						Name = "Cool",
						Calculate = Visualizers.Spectrum,
						LowestCutoff = -2f,
						HighestCutoff = 2f,
						HueMin = 0.3f,
						HueMax = 0.7f
					});
				}

				return _Visualizations;
			}
		}

		protected static List<NodePreview> Previews = new List<NodePreview>();

		protected NodePreview GetPreview(Graph graph, Node node)
		{
			var preview = Previews.FirstOrDefault(p => p.Id == node.Id);

			if (preview != null)
			{
				preview.Stale = preview.Stale || node.SourceIds.Count != preview.LastSourceIds.Count || preview.LastVisualizer != Previewer;
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
						var color = Previewer.Calculate(value, Previewer);
						preview.Preview.SetPixel(x, y, color);
					}
				}
				preview.Preview.Apply();
				preview.Stale = false;
				preview.LastUpdated = DateTime.Now.Ticks;
				preview.LastSourceIds = new List<string>(node.SourceIds);
				preview.LastVisualizer = Previewer;
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