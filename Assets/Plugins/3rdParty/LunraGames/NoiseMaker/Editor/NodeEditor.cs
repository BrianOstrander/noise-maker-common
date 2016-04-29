using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LunraGames.NoiseMaker
{
	public abstract class NodeEditor
	{
		public const int PreviewSize = 198;

		protected List<NodePreview> Previews = new List<NodePreview>();

		protected NodePreview GetPreview(Graph graph, Node node)
		{
			var preview = Previews.FirstOrDefault(p => p.Id == node.Id);

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
			}
			return preview;
		}

		public abstract Node Draw(Graph graph, Node node);
	}
}