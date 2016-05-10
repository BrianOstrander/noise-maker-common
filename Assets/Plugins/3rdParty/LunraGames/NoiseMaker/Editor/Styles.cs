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
					_BoxButton.padding.top -= 2;
					var normal = new Texture2D(_BoxButton.normal.background.width, _BoxButton.normal.background.height);
					var active = new Texture2D(_BoxButton.normal.background.width, _BoxButton.normal.background.height);

					for (var x = 0; x < active.width; x++)
					{
						for (var y = 0; y < active.height; y++)
						{
							var normalColor = x == 0 || y == 0 || x == normal.width - 1 || y == normal.height - 1 ? Color.HSVToRGB(0f, 0f, 0.35f) : Color.HSVToRGB(0f, 0f, 0.25f);
							var activeColor = x == 0 || y == 0 || x == active.width - 1 || y == active.height - 1 ? Color.HSVToRGB(0f, 0f, 0.4f) : Color.HSVToRGB(0f, 0f, 0.5f);
							active.SetPixel(x, y, activeColor.NewA(1f));
							normal.SetPixel(x, y, normalColor.NewA(1f));
						}
					}
					normal.Apply();
					active.Apply();
					_BoxButton.normal.background = normal;
					_BoxButton.focused.background = normal;
					_BoxButton.hover.background = normal;
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

		static GUIStyle _CloseButton;

		public static GUIStyle CloseButton
		{
			get
			{
				if (_CloseButton == null)
				{
					_CloseButton = new GUIStyle(GUI.skin.box);
					_CloseButton.padding.top -= 2;
					var normal = new Texture2D(_CloseButton.normal.background.width, _CloseButton.normal.background.height);
					var active = new Texture2D(_CloseButton.normal.background.width, _CloseButton.normal.background.height);

					for (var x = 0; x < active.width; x++)
					{
						for (var y = 0; y < active.height; y++)
						{
							var normalColor = x == 0 || y == 0 || x == active.width - 1 || y == active.height - 1 ? Color.HSVToRGB(0f, 0.4f, 0.5f) : Color.HSVToRGB(0f, 0.3f, 0.5f);
							var activeColor = x == 0 || y == 0 || x == active.width - 1 || y == active.height - 1 ? Color.HSVToRGB(0f, 0.3f, 0.6f) : Color.HSVToRGB(0f, 0.4f, 0.6f);
							active.SetPixel(x, y, activeColor.NewA(1f));
							normal.SetPixel(x, y, normalColor.NewA(1f));
						}
					}
					normal.Apply();
					active.Apply();
					_CloseButton.normal.background = normal;
					_CloseButton.focused.background = normal;
					_CloseButton.hover.background = normal;
					_CloseButton.active.background = active;
				}

				return _CloseButton;
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

		static GUIStyle _ToolbarButtonMiddle;

		public static GUIStyle ToolbarButtonMiddle
		{
			get
			{
				if (_ToolbarButtonMiddle == null)
				{
					_ToolbarButtonMiddle = new GUIStyle(EditorStyles.miniButtonMid);
					_ToolbarButtonMiddle.alignment = TextAnchor.MiddleCenter;
					_ToolbarButtonMiddle.fontSize = 18;
					_ToolbarButtonMiddle.fixedHeight = 24f;
				}

				return _ToolbarButtonMiddle;
			}
		}

		static GUIStyle _ToolbarButtonRight;

		public static GUIStyle ToolbarButtonRight
		{
			get
			{
				if (_ToolbarButtonRight == null)
				{
					_ToolbarButtonRight = new GUIStyle(EditorStyles.miniButtonRight);
					_ToolbarButtonRight.alignment = TextAnchor.MiddleCenter;
					_ToolbarButtonRight.fontSize = 18;
					_ToolbarButtonRight.fixedHeight = 24f;
				}

				return _ToolbarButtonRight;
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