using UnityEngine;
using UnityEditor;

namespace LunraGames.NoiseMaker
{
	// todo: make a proper GUISkin file for these, instead of these hacks.
	public class Styles
	{
		public static Color RootColor { get { return Color.cyan; } }

		static GUIStyle _Blank;

		public static GUIStyle Blank
		{
			get
			{
				if (_Blank == null)
				{
					_Blank = new GUIStyle(GUI.skin.box);
					_Blank.alignment = TextAnchor.MiddleLeft;
					_Blank.padding.top -= 2;
					var normal = new Texture2D(_Blank.normal.background.width, _Blank.normal.background.height);
					var active = new Texture2D(_Blank.normal.background.width, _Blank.normal.background.height);

					for (var x = 0; x < active.width; x++)
					{
						for (var y = 0; y < active.height; y++)
						{
							active.SetPixel(x, y, new Color(0.219f, 0.219f, 0.219f));
							normal.SetPixel(x, y, new Color(0.219f, 0.219f, 0.219f));
						}
					}
					normal.Apply();
					active.Apply();
					_Blank.normal.background = normal;
					_Blank.focused.background = normal;
					_Blank.hover.background = normal;
					_Blank.active.background = active;
				}

				return _Blank;
			}
		}

		static GUIStyle _BoxButton;

		public static GUIStyle BoxButton
		{
			get
			{
				if (_BoxButton == null)
				{
					_BoxButton = new GUIStyle(GUI.skin.box);
					_BoxButton.alignment = TextAnchor.MiddleLeft;
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

		static GUIStyle _RenameButton;

		public static GUIStyle RenameButton
		{
			get
			{
				if (_RenameButton == null)
				{
					_RenameButton = new GUIStyle(GUI.skin.box);
					_RenameButton.padding.top -= 2;
					var normal = new Texture2D(_RenameButton.normal.background.width, _RenameButton.normal.background.height);
					var active = new Texture2D(_RenameButton.normal.background.width, _RenameButton.normal.background.height);

					for (var x = 0; x < active.width; x++)
					{
						for (var y = 0; y < active.height; y++)
						{
							var normalColor = x == 0 || y == 0 || x == active.width - 1 || y == active.height - 1 ? Color.HSVToRGB(0.55f, 0.4f, 0.5f) : Color.HSVToRGB(0.51f, 0.3f, 0.5f);
							var activeColor = x == 0 || y == 0 || x == active.width - 1 || y == active.height - 1 ? Color.HSVToRGB(0.55f, 0.3f, 0.6f) : Color.HSVToRGB(0.51f, 0.4f, 0.6f);
							active.SetPixel(x, y, activeColor.NewA(1f));
							normal.SetPixel(x, y, normalColor.NewA(1f));
						}
					}
					normal.Apply();
					active.Apply();
					_RenameButton.normal.background = normal;
					_RenameButton.focused.background = normal;
					_RenameButton.hover.background = normal;
					_RenameButton.active.background = active;
				}

				return _RenameButton;
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

		static GUIStyle _OptionButtonMiddle;

		public static GUIStyle OptionButtonMiddle
		{
			get
			{
				if (_OptionButtonMiddle == null)
				{
					_OptionButtonMiddle = new GUIStyle(EditorStyles.miniButtonMid);
					_OptionButtonMiddle.alignment = TextAnchor.MiddleLeft;
					_OptionButtonMiddle.fontSize = 16;
				}

				return _OptionButtonMiddle;
			}
		}

		static GUIStyle _OptionButtonRight;

		public static GUIStyle OptionButtonRight
		{
			get
			{
				if (_OptionButtonRight == null)
				{
					_OptionButtonRight = new GUIStyle(EditorStyles.miniButtonRight);
					_OptionButtonRight.alignment = TextAnchor.MiddleLeft;
					_OptionButtonRight.fontSize = 16;
				}

				return _OptionButtonRight;
			}
		}

		static GUIStyle _ToolbarButtonLeft;

		public static GUIStyle ToolbarButtonLeft
		{
			get
			{
				if (_ToolbarButtonLeft == null)
				{
					_ToolbarButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
					_ToolbarButtonLeft.alignment = TextAnchor.MiddleCenter;
					_ToolbarButtonLeft.fontSize = 18;
				}

				return _ToolbarButtonLeft;
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

		static GUIStyle _NoPreviewLabel;

		public static GUIStyle NoPreviewLabel
		{
			get 
			{
				if (_NoPreviewLabel == null)
				{
					_NoPreviewLabel = new GUIStyle(EditorStyles.label);
					_NoPreviewLabel.alignment = TextAnchor.MiddleCenter;
					_NoPreviewLabel.fontSize = 48;
					_NoPreviewLabel.fixedHeight = 128f;
				}
				return _NoPreviewLabel;
			}
		}

		static GUIStyle _NoPreviewSmallLabel;

		public static GUIStyle NoPreviewSmallLabel
		{
			get 
			{
				if (_NoPreviewSmallLabel == null)
				{
					_NoPreviewSmallLabel = new GUIStyle(EditorStyles.label);
					_NoPreviewSmallLabel.alignment = TextAnchor.MiddleCenter;
					_NoPreviewSmallLabel.fontSize = 34;
					_NoPreviewSmallLabel.fixedHeight = 128f;
				}
				return _NoPreviewSmallLabel;
			}
		}

		static GUIStyle _PreviewToolbarLeft;

		public static GUIStyle PreviewToolbarLeft
		{
			get
			{
				if (_PreviewToolbarLeft == null)
				{
					_PreviewToolbarLeft = new GUIStyle(EditorStyles.miniButtonLeft);
					_PreviewToolbarLeft.alignment = TextAnchor.LowerLeft;
					_PreviewToolbarLeft.fontSize = 18;
					_PreviewToolbarLeft.fixedHeight = 23f;
				}

				return _PreviewToolbarLeft;
			}
		}

		static GUIStyle _PreviewToolbarMiddle;

		public static GUIStyle PreviewToolbarMiddle
		{
			get
			{
				if (_PreviewToolbarMiddle == null)
				{
					_PreviewToolbarMiddle = new GUIStyle(EditorStyles.miniButtonMid);
					_PreviewToolbarMiddle.alignment = TextAnchor.MiddleCenter;
					_PreviewToolbarMiddle.fontSize = 18;
					_PreviewToolbarMiddle.fixedHeight = 23f;
				}

				return _PreviewToolbarMiddle;
			}
		}

		static GUIStyle _PreviewToolbarSelected;

		public static GUIStyle PreviewToolbarSelected
		{
			get
			{
				if (_PreviewToolbarSelected == null)
				{
					_PreviewToolbarSelected = new GUIStyle(EditorStyles.miniButtonMid);
					_PreviewToolbarSelected.alignment = TextAnchor.MiddleCenter;
					_PreviewToolbarSelected.fontSize = 18;
					_PreviewToolbarSelected.fixedHeight = 23f;

					_PreviewToolbarSelected.normal = _PreviewToolbarSelected.active;
				}

				return _PreviewToolbarSelected;
			}
		}

		static GUIStyle _PreviewBackground;

		public static GUIStyle PreviewBackground
		{
			get 
			{
				if (_PreviewBackground == null)
				{
					_PreviewBackground = new GUIStyle(GUI.skin.box);

					_PreviewBackground.margin.left = 0;
					_PreviewBackground.margin.top = 0;
					_PreviewBackground.margin.bottom = 0;
					_PreviewBackground.padding.left = 0;
					_PreviewBackground.padding.right = 0;
					_PreviewBackground.padding.top = 0;

					_PreviewBackground.border.top = 16;
					_PreviewBackground.border.bottom = 2;

					var background = new Texture2D(64, 64);

					for (var x = 0; x < background.width; x++)
					{
						for (var y = 0; y < background.height; y++)
						{
							var color = (background.height - 16) < y ? Color.clear : Color.HSVToRGB(0f, 0f, 0.25f);
							background.SetPixel(x, y, color);
						}
					}
					background.Apply();
					_PreviewBackground.normal.background = background;
				}
				return _PreviewBackground;
			}
		}

		static GUIStyle _CreateFirstLatitudeButton;

		public static GUIStyle CreateFirstLatitudeButton
		{
			get
			{
				if (_CreateFirstLatitudeButton == null)
				{
					_CreateFirstLatitudeButton = new GUIStyle(GUI.skin.button);
					_CreateFirstLatitudeButton.alignment = TextAnchor.MiddleCenter;
					_CreateFirstLatitudeButton.fontSize = 32;
					_CreateFirstLatitudeButton.fixedHeight = 98f;
				}

				return _CreateFirstLatitudeButton;
			}
		}

		public static GUIStyle StretchedImageButton(Texture2D image)
		{
			var button = new GUIStyle(GUI.skin.button);
			button.normal.background = image;

			return button;
		}
	}
}