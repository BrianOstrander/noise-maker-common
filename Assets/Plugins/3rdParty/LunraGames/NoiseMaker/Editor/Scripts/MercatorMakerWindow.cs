using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LibNoise.Models;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class MercatorMakerWindow : EditorWindow
	{
		public class Layouts
		{
			public const float PreviewHeight = 256f;
		}

		enum States
		{
			Splash,
			Idle
		}

		[SerializeField]
		States State = States.Splash;
		[SerializeField]
		string SavePath;
		[SerializeField]
		Vector2 LatitudesScrollPosition = Vector2.zero;
		[SerializeField]
		int SelectedIndex = -1;

		Mercator Mercator;
		Latitude Selected 
		{
			get 
			{
				if (SelectedIndex < 0 || Mercator == null || Mercator.Latitudes == null || Mercator.Latitudes.Count == 0) return null;
				return Mercator.Latitudes[SelectedIndex];
			}
		}

		[MenuItem ("Window/Lunra Games/Noise Maker - Mercator")]
		static void Init () 
		{
			var window = EditorWindow.GetWindow(typeof (MercatorMakerWindow), false, "Mercator") as MercatorMakerWindow;
			window.titleContent = new GUIContent("Mercator", NoiseMakerConfig.Instance.MercatorTab);
			window.Show();
		}

		#region Messages
		void OnGUI()
		{
			try 
			{
				if (State == States.Idle && Mercator == null && !StringExtensions.IsNullOrWhiteSpace(SavePath))
				{
					var config = AssetDatabase.LoadAssetAtPath<MercatorMap>(SavePath);
					if (config == null) State = States.Splash;
					else Mercator = config.MercatorInstantiation;
				}

				if (State == States.Splash) DrawSplash();
				else if (State == States.Idle)
				{
					DrawHeader();
					DrawLatitudes();
					DrawLatitudeEditor();
					DrawPreview();
				}
			}
			catch (Exception e)
			{
				EditorGUILayout.HelpBox("Exception occured: \n"+e.Message, MessageType.Error);
				Debug.LogException(e);
				GUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("Print Exception")) Debug.LogException(e);
					GUI.color = Color.red;
					if (GUILayout.Button("Reset")) Reset();
				}
				GUILayout.EndHorizontal();
			}
		}

		void OnFocus() { Save(); }
		void OnLostFocus() { Save(); }
		void OnProjectChange() { Save(); }
		#endregion

		void DrawSplash ()
		{
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
				{
					GUILayout.Label("Create or Open Mercator Map");
					GUILayout.BeginHorizontal();
					{
						if (GUILayout.Button("New")) 
						{
							var savePath = UnityEditor.EditorUtility.SaveFilePanelInProject("New Mercator Map", "Mercator", "asset", null);
							if (!StringExtensions.IsNullOrWhiteSpace(savePath))
							{
								SavePath = savePath;
								var config = MercatorMap.CreateInstance<MercatorMap>();
								AssetDatabase.CreateAsset(config, SavePath);
								Mercator = new Mercator();
								State = States.Idle;
							}
						}
						if (GUILayout.Button("Open"))
						{
							var openPath = UnityEditor.EditorUtility.OpenFilePanel("Open Mercator Map", Application.dataPath, "asset");
							if (!StringExtensions.IsNullOrWhiteSpace(openPath))
							{
								if (openPath.StartsWith(Application.dataPath))
								{
									SavePath = "Assets"+openPath.Substring(Application.dataPath.Length);
									var config = AssetDatabase.LoadAssetAtPath<MercatorMap>(SavePath);
									Mercator = config.MercatorInstantiation;
									State = States.Idle;
								}
								else UnityEditor.EditorUtility.DisplayDialog("Invalid", "Selected mercator map must be inside project directory.", "Okay");
							}
						}
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
		}

		void DrawHeader()
		{
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("Reset", Styles.ToolbarButtonMiddle)) Reset();
				if (GUILayout.Button("Save", Styles.ToolbarButtonMiddle)) Save();
			}
			GUILayout.EndHorizontal();
		}

		void DrawLatitudes()
		{
			if (Mercator == null) return;

			if (Mercator.Latitudes.Count == 0)
			{
//				if (GUILayout.Button("Add Latitude", Styles.ToolbarButtonMiddle))
//				{
//					Selected = new Latitude();
//					Mercator.Latitudes.Add(Selected);
//				}

				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();
					GUILayout.BeginVertical();
					{
						GUILayout.Label("No Latitudes in Mercator", Styles.NoPreviewLabel);
						if (GUILayout.Button("Click Here to Create a Latitude", Styles.CreateFirstLatitudeButton))
						{
							Mercator.Latitudes.Add(new Latitude());
						}
					}
					GUILayout.EndVertical();
					GUILayout.FlexibleSpace();
				}
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				return;
			}
			LatitudesScrollPosition = GUILayout.BeginScrollView(new Vector2(0f, LatitudesScrollPosition.y), false, true);
			{
				foreach(var latitude in Mercator.Latitudes)
				{
					var unmodifiedLatitude = latitude;
					GUILayout.BeginHorizontal(GUILayout.Height(128f));
					{
						GUILayout.BeginVertical(GUILayout.Width(64f));
						{
							if (GUILayout.Button("^", GUILayout.ExpandHeight(true)))
							{

							}
							if (GUILayout.Button("v", GUILayout.ExpandHeight(true)))
							{

							}
						}
						GUILayout.EndVertical();

					}
					GUILayout.EndHorizontal();
				}
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndScrollView();
		}

		void DrawLatitudeEditor()
		{
			if (Selected != null)
			{
				GUILayout.Box("No Latitude Selected");
				return;
			}

			GUILayout.Box("lol selected");
		}

		void DrawPreview()
		{
			GUILayout.BeginVertical(GUILayout.Height(Layouts.PreviewHeight));
			{
				GUILayout.Label("lol preview");
			}
			GUILayout.BeginVertical();
		}

		void Reset()
		{
			State = States.Splash;
			SavePath = null;
			Mercator = null;
			SelectedIndex = -1;
		}

		void Save()
		{
			if (State != States.Idle || Mercator == null) return;
			if (StringExtensions.IsNullOrWhiteSpace(SavePath)) throw new NullReferenceException("SavePath cannot be null");
			var config = AssetDatabase.LoadAssetAtPath<MercatorMap>(SavePath);
			config.MercatorInstantiation = Mercator;
			UnityEditor.EditorUtility.SetDirty(config);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}