using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using LibNoise;
using LibNoise.Models;

namespace LunraGames.NoiseMaker
{
	public static class BiomeEditor
	{
		public static int PreviewWidth = 198;
		public static int PreviewHeight = 64;

		public static VisualizationPreview Previewer = NodeEditor.Visualizations[0];

		static List<BiomePreview> Previews = new List<BiomePreview>();

		public static BiomePreview GetPreview(Mercator mercator, Domain domain, Biome biome, object module)
		{
			var preview = Previews.FirstOrDefault(p => p.Id == biome.Id && p.DomainId == domain.Id);

			if (preview == null)
			{
				preview = new BiomePreview { Id = biome.Id, DomainId = domain.Id, Stale = true };
				preview.Preview = new Texture2D(PreviewWidth, PreviewHeight);
				Previews.Add(preview);
			}
			else
			{
				preview.Stale = preview.Stale || biome.AltitudeIds.Count != preview.LastSourceIds.Count || preview.LastVisualizer != Previewer;
				for (var i = 0; i < biome.AltitudeIds.Count; i++)
				{
					var id = biome.AltitudeIds[i];
					preview.Stale = preview.Stale || id != preview.LastSourceIds[i];
				}
				preview.Stale = preview.Stale || preview.LastUpdated < DomainEditor.LastUpdated(domain.Id);
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

								var normalValue = biome.GetColor(latitude, longitude, value, mercator);
								var highlightedValue = Previewer.Calculate(value, Previewer);

								pixels[SphereUtils.PixelCoordinateToIndex(x, y, width, height)] = Mathf.Approximately(0f, domain.GetWeight(latitude, longitude, value)) ? highlightedValue : normalValue;
							}
						}
					},
					() => TextureFarmer.Queue
					(
						preview.Preview, 
						pixels, 
						() =>
						{
							MercatorMakerWindow.PreviewUpdating = false;
							MercatorMakerWindow.QueueRepaint();
						},
						() =>
						{
							MercatorMakerWindow.PreviewUpdating = true;
							MercatorMakerWindow.QueueRepaint();
						}
					),
					Debug.LogException
				);

				preview.Stale = false;
				preview.LastUpdated = DateTime.Now.Ticks;
				preview.LastSourceIds = new List<string>(biome.AltitudeIds);
				preview.LastVisualizer = Previewer;
				preview.LastModule = module;
			}

			return preview;
		}
	}
}