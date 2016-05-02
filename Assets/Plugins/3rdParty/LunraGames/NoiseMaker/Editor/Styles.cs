using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Atesh;

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
							active.SetPixel(x, y, color.NewA(1f));
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
							hovered.SetPixel(x, y, color.NewA(1f));
						}
					}
					hovered.Apply();
					_BoxButtonHovered.normal.background = hovered;
				}

				return _BoxButtonHovered;
			}
		}

		static GUIStyle _Foldout;

		public static GUIStyle Foldout
		{
			get
			{
				if (_Foldout == null)
				{
					_Foldout = new GUIStyle(EditorStyles.foldout);
					_Foldout.fontSize = 18;
					_Foldout.fixedHeight = 24f;
					_Foldout.alignment = TextAnchor.MiddleLeft;
					_Foldout.imagePosition = ImagePosition.TextOnly;
					_Foldout.padding.bottom += 8;
				}

				return _Foldout;
			}
		}

		static GUIStyle _OptionBox;

		public static GUIStyle OptionBox
		{
			get
			{
				if (_OptionBox == null)
				{
					_OptionBox = new GUIStyle(GUI.skin.box);
					var background = new Texture2D(_OptionBox.normal.background.width, _OptionBox.normal.background.height);

					for (var x = 0; x < background.width; x++)
					{
						for (var y = 0; y < background.height; y++)
						{
							var color = x == 0 || x == background.width - 1 ? Color.HSVToRGB(0f, 0f, 0.1f) : Color.HSVToRGB(0f, 0f, 0.25f);
							background.SetPixel(x, y, color.NewA(0.75f));
						}
					}
					background.Apply();
					_OptionBox.normal.background = background;
				}

				return _OptionBox;
			}
		}

		static GUIStyle _OptionButton;

		public static GUIStyle OptionButton
		{
			get
			{
				if (_OptionButton == null)
				{
					_OptionButton = new GUIStyle(EditorStyles.miniButtonRight);
					_OptionButton.alignment = TextAnchor.MiddleLeft;
					_OptionButton.fontSize = 16;
					_OptionButton.fixedHeight = 24f;
				}

				return _OptionButton;
			}
		}

		static GUIStyle _ResetButton;

		public static GUIStyle ResetButton
		{
			get
			{
				if (_ResetButton == null)
				{
					_ResetButton = new GUIStyle(EditorStyles.miniButtonRight);
					_ResetButton.alignment = TextAnchor.MiddleCenter;
					_ResetButton.fontSize = 18;
					_ResetButton.fixedHeight = 24f;
				}

				return _ResetButton;
			}
		}

		static GUIStyle _VisualizationToggle;

		public static GUIStyle VisualizationToggle
		{
			get
			{
				if (_VisualizationToggle == null)
				{
					_VisualizationToggle = new GUIStyle(EditorStyles.miniButtonMid);
					_VisualizationToggle.alignment = TextAnchor.MiddleCenter;
					_VisualizationToggle.fontSize = 18;
					_VisualizationToggle.fixedHeight = 24f;
				}

				return _VisualizationToggle;
			}
		}

		static GUIStyle _VisualizationRangeLabel;

		public static GUIStyle VisualizationRangeLabel
		{
			get
			{
				if (_VisualizationRangeLabel == null)
				{
					_VisualizationRangeLabel = new GUIStyle(EditorStyles.label);
					_VisualizationRangeLabel.padding.bottom -= 4;
					_VisualizationRangeLabel.alignment = TextAnchor.LowerCenter;
					_VisualizationRangeLabel.fontSize = 16;
					_VisualizationRangeLabel.fixedHeight = 24f;
				}

				return _VisualizationRangeLabel;
			}
		}
	}
}