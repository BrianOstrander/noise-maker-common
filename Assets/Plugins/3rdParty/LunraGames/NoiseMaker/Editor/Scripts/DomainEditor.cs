﻿using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using LibNoise;
using LibNoise.Models;

namespace LunraGames.NoiseMaker
{
	public abstract class DomainEditor
	{
		public static int PreviewWidth = 198;
		public static int PreviewHeight = 64;

		public static VisualizationPreview Previewer = NodeEditor.Visualizations[0];

		protected static List<DomainPreview> Previews = new List<DomainPreview>();

		protected DomainPreview GetPreview(Domain domain, object module)
		{
			var preview = Previews.FirstOrDefault(p => p.Id == domain.Id);

			if (preview != null)
			{
				preview.Stale = preview.Stale || domain.SourceIds.Count != preview.LastSourceIds.Count || preview.LastVisualizer != Previewer;
				for (var i = 0; i < domain.SourceIds.Count; i++)
				{
					var id = domain.SourceIds[i];
					preview.Stale = preview.Stale || id != preview.LastSourceIds[i];
					if (StringExtensions.IsNullOrWhiteSpace(id)) continue;
					var sourcePreview = Previews.FirstOrDefault(p => p.Id == id);
					if (sourcePreview == null) continue;
					preview.Stale = preview.Stale || preview.LastUpdated < sourcePreview.LastUpdated;
				}
			}

			if (preview == null)
			{
				preview = new DomainPreview { Id = domain.Id, Stale = true };
				preview.Preview = new Texture2D(PreviewWidth, PreviewHeight);
				Previews.Add(preview);
			}

			if (preview.Stale)
			{
				var width = preview.Preview.width;
				var height = preview.Preview.height;
				var pixels = new Color[width * height];

				Thrifty.Queue(
					() =>
					{
						for (var x = 0; x < width; x++)
						{
							for (var y = 0; y < height; y++)
							{
								var latitude = SphereUtils.GetLatitude(y, height);
								var longitude = SphereUtils.GetLongitude(x, width);

								float value;

								if (module is Sphere) value = (float)(module as Sphere).GetValue(latitude, longitude);
								else value = (float)(module as IModule).GetValue((double)x, (double)y, 0.0);

								var normalValue = Previewer.Calculate(value, Previewer);
								var highlightedValue = Color.green.NewV(normalValue);

								pixels[(width * y) + x] = Mathf.Approximately(0f, domain.GetWeight(latitude, longitude, value)) ? normalValue : highlightedValue;
							}
						}
					},
					() => TextureFarmer.Queue (preview.Preview, pixels, MercatorMakerWindow.QueueRepaint, MercatorMakerWindow.QueueRepaint)
				);

				preview.Stale = false;
				preview.LastUpdated = DateTime.Now.Ticks;
				preview.LastSourceIds = new List<string>(domain.SourceIds);
				preview.LastVisualizer = Previewer;
			}

			return preview;
		}

		public abstract Domain Draw(Mercator mercator, Domain domain, object module, out Texture2D preview);	
	}
}