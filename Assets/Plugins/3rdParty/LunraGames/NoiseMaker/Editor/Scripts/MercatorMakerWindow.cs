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
			public const float LatitudeEntryHeight = 64f;
			public const float AltitudeEntriesHeight = 128f;
			public const float AltitudeEntrySelectWidth = 64f;
			public const float AltitudeEntryDeleteWidth = 32f;
			public const float AltitudeEntryEditorHeight = 128f;
			public const float LatitudeEditorHeight = AltitudeEntriesHeight + AltitudeEntryEditorHeight;
		}

		const float AltitudeOffset = 0.01f;

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
		int SelectedLatitudeIndex = -1;
		[SerializeField]
		int SelectedAltitudeIndex = -1;

		Mercator Mercator;

		Latitude SelectedLatitude 
		{
			get 
			{
				if (SelectedLatitudeIndex < 0 || Mercator == null || Mercator.Latitudes == null || Mercator.Latitudes.Count == 0 || Mercator.Latitudes.Count <= SelectedLatitudeIndex) return null;
				return Mercator.Latitudes[SelectedLatitudeIndex];
			}
		}

		[MenuItem ("Window/Lunra Games/Noise Maker/Mercator")]
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
				try
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
				} catch {}
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

			if (GUILayout.Button("+", Styles.ToolbarButtonMiddle)) 
			{
				Mercator.Latitudes.Insert(0, new Latitude());
				if (SelectedLatitudeIndex != -1) SelectedLatitudeIndex++;
			}

			LatitudesScrollPosition = GUILayout.BeginScrollView(new Vector2(0f, LatitudesScrollPosition.y), false, true);
			{
				int? deletedIndex = null;

				for(var i = 0; i < Mercator.Latitudes.Count; i++)
				{
					if (SelectedLatitudeIndex == i) GUI.color = Color.magenta;

					var unmodifiedI = i;
					var latitude = Mercator.Latitudes[i];
					GUILayout.BeginHorizontal();
					{
						if (GUILayout.Button("Copy", Styles.ToolbarButtonLeft, GUILayout.Width(64f), GUILayout.Height(Layouts.LatitudeEntryHeight)))
						{

						}

						if (GUILayout.Button("Entry", Styles.ToolbarButtonMiddle, GUILayout.ExpandWidth(true), GUILayout.Height(Layouts.LatitudeEntryHeight))) 
						{
							if (SelectedLatitudeIndex == unmodifiedI) SelectedLatitudeIndex = -1;
							else SelectedLatitudeIndex = unmodifiedI;
						}

						if (GUILayout.Button("X", EditorStyles.miniButtonRight, GUILayout.Width(32f), GUILayout.Height(Layouts.LatitudeEntryHeight))) deletedIndex = unmodifiedI;
					}
					GUILayout.EndHorizontal();

					GUI.color = Color.white;
				}
				GUILayout.FlexibleSpace();

				if (deletedIndex.HasValue) 
				{
					if (SelectedLatitudeIndex == deletedIndex.Value) SelectedLatitudeIndex = -1;
					if (deletedIndex.Value < SelectedLatitudeIndex && SelectedLatitudeIndex != -1) SelectedLatitudeIndex--;
					Mercator.Latitudes.RemoveAt(deletedIndex.Value);
				}

			}
			GUILayout.EndScrollView();

			if (GUILayout.Button("+", Styles.ToolbarButtonMiddle)) Mercator.Latitudes.Add(new Latitude());
		}

		void DrawLatitudeEditor()
		{
			GUILayout.BeginVertical(GUILayout.Height(Layouts.LatitudeEditorHeight));
			{
				if (SelectedLatitude == null) GUILayout.Box("No Latitude Selected");
				else
				{

					var latitude = SelectedLatitude;

					if (latitude.Altitudes == null)
					{
						EditorGUILayout.HelpBox("Latitude has null Altitudes array.", MessageType.Error);
						return;
					}

					var editors = AltitudeEditorCacher.Editors;
					var keys = new List<string>(new [] {"Select Altitude Drawer"});
					foreach (var val in editors.Values) keys.Add(val.Details.Name);

					var editorSelected = EditorGUILayout.Popup(0, keys.ToArray());

					if (editorSelected != 0)
					{
						var selectedEditor = editors.FirstOrDefault(kv => kv.Value.Details.Name == keys[editorSelected]);
						var altitude = Activator.CreateInstance(selectedEditor.Value.Details.Target) as Altitude;

						altitude.MaxAltitude = 1f;

						var last = latitude.Altitudes.LastOrDefault();

						if (last == null)
						{
							altitude.MinAltitude = 0f;
						}
						else
						{
							var delta = last.MaxAltitude - last.MinAltitude;
							last.MaxAltitude = last.MinAltitude + (delta * 0.5f);
							delta = last.MaxAltitude - last.MinAltitude;

							altitude.MinAltitude = Mathf.Max(last.MinAltitude + (delta * 0.5f), last.MaxAltitude - AltitudeOffset);
						}
						altitude.Id = Guid.NewGuid().ToString();
						latitude.Altitudes.Add(altitude);
						Repaint();
					}
					else
					{
						GUILayout.BeginScrollView(Vector2.one, false, true, GUILayout.Height(Layouts.AltitudeEntriesHeight));
						{
							var width = position.width - Layouts.AltitudeEntryDeleteWidth - Layouts.AltitudeEntrySelectWidth - 32f; // Magic value for scroll bar.
							int? deletedIndex = null;

							for (var i = 0; i < latitude.Altitudes.Count; i++)
							{
								var min = 0f;
								var max = 1f;

								var maxMin = 1f;
								var minMax = 0f;

								if (0 < i)
								{
									// not first
									var previous = latitude.Altitudes[i - 1];
									var midway = previous.MinAltitude + ((previous.MaxAltitude - previous.MinAltitude) * 0.5f);
									min = Mathf.Min(midway, previous.MinAltitude + AltitudeOffset);
									minMax = Mathf.Max(midway, previous.MaxAltitude - AltitudeOffset);
								}
								else minMax = 0f;

								if (i < latitude.Altitudes.Count - 1)
								{
									// not last

									var next = latitude.Altitudes[i + 1];

									if (i < latitude.Altitudes.Count - 2)
									{
										// not second to last
										var boundry = latitude.Altitudes[i + 2];
										max = Mathf.Max(next.MinAltitude + ((boundry.MinAltitude - next.MinAltitude) * 0.5f), boundry.MinAltitude - AltitudeOffset);
										maxMin = Mathf.Min(next.MinAltitude + ((boundry.MinAltitude - next.MinAltitude) * 0.5f), next.MinAltitude + AltitudeOffset);
									}
									else 
									{
										// second to last
										max = next.MaxAltitude - AltitudeOffset;
										maxMin = Mathf.Min(next.MinAltitude + ((next.MaxAltitude - next.MinAltitude) * 0.5f), next.MinAltitude + AltitudeOffset);
									}
								}
								else maxMin = 1f;

								if (SelectedAltitudeIndex == i) GUI.color = Color.magenta;

								var unmodifiedI = i;
								var altitude = latitude.Altitudes[unmodifiedI];
								GUILayout.BeginHorizontal();
								{
									if (GUILayout.Button(unmodifiedI+" Select", GUILayout.Width(Layouts.AltitudeEntrySelectWidth)))
									{
										if (SelectedAltitudeIndex == unmodifiedI) SelectedAltitudeIndex = -1;
										else SelectedAltitudeIndex = unmodifiedI;
									}
									altitude.MinAltitude = Mathf.Max(min, altitude.MinAltitude);
									altitude.MaxAltitude = Mathf.Min(max, altitude.MaxAltitude);
									EditorGUILayout.MinMaxSlider(GUIContent.none, ref altitude.MinAltitude, ref altitude.MaxAltitude, 0f, 1f, GUILayout.Width(width));
									if (altitude.MaxAltitude < maxMin) altitude.MaxAltitude = maxMin;
									if (minMax < altitude.MinAltitude) altitude.MinAltitude = minMax;

									if (GUILayout.Button("x", GUILayout.Width(Layouts.AltitudeEntryDeleteWidth))) deletedIndex = unmodifiedI;
								}
								GUILayout.EndHorizontal();

								GUI.color = Color.white;
							}

							if (deletedIndex.HasValue) 
							{
								if (SelectedAltitudeIndex == deletedIndex.Value) SelectedAltitudeIndex = -1;
								if (deletedIndex.Value < SelectedAltitudeIndex && SelectedAltitudeIndex != -1) SelectedAltitudeIndex--;
								latitude.Altitudes.RemoveAt(deletedIndex.Value);
							}
						}
						GUILayout.EndScrollView();

						GUILayout.BeginVertical();
						{
							if (SelectedAltitudeIndex < 0)
							{
								GUILayout.FlexibleSpace();
								GUILayout.BeginHorizontal();
								{
									GUILayout.FlexibleSpace();
									GUILayout.BeginVertical();
									{
										GUILayout.Label("Select an Altitude", Styles.NoPreviewLabel);
									}
									GUILayout.EndVertical();
									GUILayout.FlexibleSpace();
								}
								GUILayout.EndHorizontal();
								GUILayout.FlexibleSpace();
							}
							else
							{
								var altitude = latitude.Altitudes[SelectedAltitudeIndex];
								var editor = editors.FirstOrDefault(kv => kv.Value.Details.Target == altitude.GetType());
								if (editor.Value == null) EditorGUILayout.HelpBox("Unable to find editor for altitude type \""+altitude.GetType()+"\".", MessageType.Error);
								else 
								{
									editor.Value.Editor.Draw(altitude);
								}
								GUILayout.FlexibleSpace();
							}
						}
						GUILayout.EndVertical();
					}
				}
			}
			GUILayout.EndVertical();
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
			SelectedLatitudeIndex = -1;
			SelectedAltitudeIndex = -1;
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