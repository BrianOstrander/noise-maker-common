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

		protected static List<NodePreview> Previews = new List<NodePreview>();

		protected NodePreview GetPreview(Graph graph, Node node)
		{
			var preview = Previews.FirstOrDefault(p => p.Id == node.Id);

			if (preview != null)
			{
				foreach (var id in node.SourceIds)
				{
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
						var val = (float)module.GetValue((double)x, (double)y, 0.0);
						preview.Preview.SetPixel(x, y, new Color(val, val, val));
					}
				}
				preview.Preview.Apply();
				preview.Stale = false;
				preview.LastUpdated = DateTime.Now.Ticks;
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