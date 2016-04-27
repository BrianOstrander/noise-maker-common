using UnityEngine;
using System.Collections.Generic;
using LibNoise;
using System;
using UnityEditor;

namespace LunraGames.NoiseMaker
{
	public class PerlinNode : Node
	{
		Perlin module = new Perlin();
		Texture2D preview = new Texture2D(256, 256);

		public PerlinNode()
		{
			Name = "Perlin";
			var frequency = new Action<float>(val => module.Frequency = val);
			var lacunarity = new Action<float>(val => module.Lacunarity = val);
			// todo: noise quality
			//var noiseQuality = new Action<float>(val => module.NoiseQuality = val);
			var octaveCount = new Action<int>(val => module.OctaveCount = val);
			var persistence = new Action<float>(val => module.Persistence = val);
			var seed = new Action<int>(val => module.Seed = val);

			Fields = new List<Field>
			{
				new Field { Name = "Frequency", FieldType = typeof(float), OnChange = val => frequency((float)val) },
				new Field { Name = "Lacunarity", FieldType = typeof(float), OnChange = val => lacunarity((float)val) },
				new Field { Name = "Octaves", FieldType = typeof(int), OnChange = val => octaveCount((int)val) },
				new Field { Name = "Persistence", FieldType = typeof(float), OnChange = val => persistence((float)val) },
				new Field { Name = "Seed", FieldType = typeof(int), OnChange = val => seed((int)val) }
			};
		}

		protected override void OnDraw()
		{
			if (GUILayout.Button("Redraw"))
			{
				for (var x = 0; x < preview.width; x ++)
				{
					for (var y = 0; y < preview.height; y++) 
					{
						var val = (float)module.GetValue((float)x, (float)y, 0.0);
						preview.SetPixel(x, y, new Color(val, val, val, 1f));
						//Debug.Log(val);
					}
				}
				preview.Apply();
			}
			GUILayout.Box(preview);
			DrawFields();
		}
	}
}