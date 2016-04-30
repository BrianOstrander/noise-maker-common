using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class Styles
	{
		static GUIStyle _BoxButton;

		public static GUIStyle BoxButton
		{
			get
			{
				if (_BoxButton == null)
				{
					_BoxButton = new GUIStyle(GUI.skin.box);
					var active = new Texture2D(_BoxButton.normal.background.width, _BoxButton.normal.background.height);

					for (var x = 0; x < active.width; x++)
					{
						for (var y = 0; y < active.height; y++)
						{
							var color = x == 0 || y == 0 || x == active.width - 1 || y == active.height - 1 ? Color.HSVToRGB(0f, 0f, 0.4f) : Color.HSVToRGB(0f, 0f, 0.5f);
							active.SetPixel(x, y, color);
						}
					}
					active.Apply();
					_BoxButton.active.background = active;
				}

				return _BoxButton;
			}
		}

		static GUIStyle _BoxButtonHovered;

		public static GUIStyle BoxButtonHovered
		{
			get
			{
				if (_BoxButtonHovered == null)
				{
					_BoxButtonHovered = new GUIStyle(GUI.skin.box);
					var hovered = new Texture2D(_BoxButtonHovered.normal.background.width, _BoxButtonHovered.normal.background.height);

					for (var x = 0; x < hovered.width; x++)
					{
						for (var y = 0; y < hovered.height; y++)
						{
							var color = x == 0 || y == 0 || x == hovered.width - 1 || y == hovered.height - 1 ? Color.HSVToRGB(0f, 0f, 0.4f) : Color.HSVToRGB(0f, 0f, 0.5f);
							hovered.SetPixel(x, y, color);
						}
					}
					hovered.Apply();
					_BoxButtonHovered.normal.background = hovered;
				}

				return _BoxButtonHovered;
			}
		}

	}
}