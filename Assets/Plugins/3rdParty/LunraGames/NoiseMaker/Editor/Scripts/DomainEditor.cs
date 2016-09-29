using System.Collections.Generic;
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

		static List<DomainPreview> Previews = new List<DomainPreview>();

		protected DomainPreview GetPreview(Domain domain, object module)
		{
			var preview = Previews.FirstOrDefault(p => p.Id == domain.Id);

			if (preview == null)
			{
				preview = new DomainPreview { Id = domain.Id, Stale = true };
				preview.Preview = new Texture2D(PreviewWidth, PreviewHeight);
				Previews.Add(preview);
			}
			else preview.Stale = preview.Stale || domain.BiomeId != preview.LastSourceId || preview.LastVisualizer != Previewer || preview.LastModule != module;


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
								var isSphere = module is Sphere;
								float value;
								float weight;
								
								if (isSphere) 
								{
									var latitude = SphereUtils.GetLatitude(y, height);
									var longitude = SphereUtils.GetLongitude(x, width);
									value = (float)(module as Sphere).GetValue(latitude, longitude);
									weight = domain.GetSphereWeight(latitude, longitude, value);
								}
								else
								{
									value = (float)(module as IModule).GetValue(x, y, 0.0);
									weight = domain.GetPlaneWeight(x, y, value);
								}

								var normalValue = Previewer.Calculate(value, Previewer);
								var highlightedValue = Color.green.NewV(normalValue);

								pixels[SphereUtils.PixelCoordinateToIndex(x, y, width, height)] = Mathf.Approximately(0f, weight) ? normalValue : highlightedValue;
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
					)
				);

				preview.Stale = false;
				preview.LastUpdated = DateTime.Now.Ticks;
				preview.LastSourceId = domain.BiomeId;
				preview.LastVisualizer = Previewer;
				preview.LastModule = module;
			}

			return preview;
		}

		public abstract Domain Draw(Domain domain, object module, out DomainPreview preview);	

		public static long LastUpdated(string id)
		{
			var preview = Previews.FirstOrDefault(p => p.Id == id);
			return preview == null ? long.MinValue : preview.LastUpdated;
		}
	}
}