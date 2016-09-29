using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LibNoise;
using LibNoise.Models;

namespace LunraGames.NoiseMaker
{
	public class MercatorMakerWindow : EditorWindow
	{
		public class Layouts
		{
			public const float HeaderHeight = 24f;
			public const float PreviewOptionsHeight = 58;
			public const float PreviewOptionsWidth = 650f;
			public const float PreviewXOffsetScalar = 0.6f;
			public const float PreviewWidthScalar = 1.25f;

			public const float DomainsWidthScalar = (1f - PreviewXOffsetScalar) * 0.9f;

			public const float SelectedEditorsMinHeight = 450f;
			public const float SelectedEditorsHeightScalar = 0.4f;
			public const float SelectedEditorsWidthScalar = 1f - PreviewXOffsetScalar;

			public const float SelectedEditorsDivider = 2f;
			public const float SelectedEditorsMinimizedWidth = 28f;
			public const float SelectedEditorsHeaderHeight = SelectedEditorsMinimizedWidth;
			public const float SelectedEditorsMaximizedWidthOffset = 2f * (SelectedEditorsDivider + SelectedEditorsMinimizedWidth);

			public const float ConvertToPrefabButtonWidth = 150f;

			public const float AltitudesEditorOffsetY = 42f;
			public const float AltitudesEditorEntryDivider = 2f;

			public static float AltitudesEditorMarginHeight { get { return NoiseMakerConfig.Instance.BiomeAltitudeWidgetMarginTop.active.background.height; } }
			public static float AltitudesEditorTabHeight { get { return NoiseMakerConfig.Instance.BiomeAltitudeWidgetTop.active.background.height; } }
			public static float AltitudesEditorEntryWidth { get { return NoiseMakerConfig.Instance.BiomeAltitudeWidgetLine.active.background.width; } }
			public static float AltitudesEditorEditHeight = AltitudesEditorTabHeight;
			public static float AltitudesEditorDeleteHeight = AltitudesEditorEditHeight;
		}

		enum States
		{
			Splash,
			Idle
		}

		static MercatorMakerWindow Instance;

		MercatorMakerWindow()
		{
			Instance = this;
		}

		public static bool PreviewUpdating;
		static bool RepaintQueued;

		[SerializeField]
		States State = States.Splash;
		[SerializeField]
		string SaveGuid;
		[SerializeField]
		NoiseGraph NoiseGraph;
		[SerializeField]
		Vector2 DomainsScrollPosition = Vector2.zero;
		[SerializeField]
		string DomainSelection;
		[SerializeField]
		string BiomeSelection;
		[SerializeField]
		string AltitudeSelection;
		[SerializeField]
		List<bool> EditorFoldouts = new List<bool>();
		[SerializeField]
		Vector2 BiomeScrollPosition = Vector2.zero;

		long GraphLastUpdated;
		Graph Graph;
		Dictionary<string, Action<Node<IModule>, Rect, int>> Previews;
		int PreviewSelected;
		long PreviewLastUpdated;
		Texture2D PreviewTexture;
		Mesh PreviewMesh;
		Editor PreviewObjectEditor;
		object PreviewModule;

		string SavePath { get { return StringExtensions.IsNullOrWhiteSpace(SaveGuid) ? null : AssetDatabase.GUIDToAssetPath(SaveGuid); } }

		Mercator Mercator;

		[MenuItem ("Window/Lunra Games/Noise Maker/Mercator")]
		static void Init () 
		{
			var window = EditorWindow.GetWindow(typeof (MercatorMakerWindow), false, "Mercator") as MercatorMakerWindow;
			window.titleContent = new GUIContent("Mercator", NoiseMakerConfig.Instance.MercatorTab);
			window.minSize = new Vector2(650f, 650f);
			window.Show();
		}

		#region Messages
		void OnGUI()
		{
			try 
			{
				if (Application.isPlaying)
				{
					GUILayout.FlexibleSpace();
					GUILayout.BeginHorizontal();
					{
						GUILayout.FlexibleSpace();
						GUILayout.Label("Editing Not Permitted While Playing", Styles.NoPreviewLabel);
						GUILayout.FlexibleSpace();
					}
					GUILayout.EndHorizontal();
					GUILayout.FlexibleSpace();
					return;
				}

				var splashImage = NoiseMakerConfig.Instance.Splash;
				GUI.Box (new Rect (0f, position.height - splashImage.height, splashImage.width, splashImage.height), splashImage, GUIStyle.none);
				
				if (State == States.Idle && Mercator == null && !StringExtensions.IsNullOrWhiteSpace(SaveGuid))
				{
					var config = AssetDatabase.LoadAssetAtPath<MercatorMap>(SavePath);
					if (config == null) State = States.Splash;
					else Mercator = config.MercatorInstantiation;
				}

				if (State == States.Splash) DrawSplash();
				else if (State == States.Idle)
				{
					DrawDomains();
					DrawPreview();
					DrawHeader();
					if (!string.IsNullOrEmpty(DomainSelection)) DrawSelectedEditors(Mercator.Domains.FirstOrDefault(d => d.Id == DomainSelection));
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

		void Update() 
		{
			if (RepaintQueued)
			{
				Repaint();
				RepaintQueued = false;
			}
		}
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
							var savePath = EditorUtility.SaveFilePanelInProject("New Mercator Map", "Mercator", "asset", null);
							if (!StringExtensions.IsNullOrWhiteSpace(savePath))
							{
								var config = CreateInstance<MercatorMap>();
								AssetDatabase.CreateAsset(config, savePath);
								SaveGuid = AssetDatabase.AssetPathToGUID(savePath);
								Mercator = new Mercator();
								State = States.Idle;
							}
						}
						if (GUILayout.Button("Open"))
						{

							var openPath = UnityEditor.EditorUtility.OpenFilePanel("Open Mercator Map", Application.dataPath, "asset");
							if (!StringExtensions.IsNullOrWhiteSpace(openPath)) Open(openPath);
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
			// Reset back to splash.
			if (GUI.Button(new Rect(0f, 0f, 128f, 24f), "Reset", Styles.ToolbarButtonMiddle)) Reset();
			// Save active file.
			if (GUI.Button(new Rect(0f + 128f, 0f, 128f, 24f), "Save", Styles.ToolbarButtonRight)) Save();
		}

		void DrawPreview()
		{
			if (Previews == null) 
			{
				Previews = new Dictionary<string, Action<Node<IModule>, Rect, int>> {
					{ "Flat", DrawPlanePreview },
					{ "Sphere", DrawSpherePreview },
					{ "Elevation", DrawElevationPreview }
				};
			}
			var keys = Previews.Keys.ToArray();

			GUILayout.BeginArea(new Rect(position.width - Layouts.PreviewOptionsWidth, 0f, Layouts.PreviewOptionsWidth, Layouts.PreviewOptionsHeight));
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Box(GUIContent.none, Styles.PreviewToolbarLeft, GUILayout.Width(24f));
					for (var i = 0; i < keys.Length; i++)
					{
						if (GUILayout.Button(keys[i], i == PreviewSelected ? Styles.PreviewToolbarSelected : Styles.PreviewToolbarMiddle) && PreviewSelected != i) 
						{
							// When previews change, we reset any cached info.
							PreviewUpdating = false;
							PreviewSelected = i;
							PreviewLastUpdated = 0L;
							PreviewObjectEditor = null;
							PreviewTexture = null;
							PreviewMesh = null;
						}
					}
				}
				GUILayout.EndHorizontal();

				UnityEngine.Object freshNoiseGraph = null;
				// Apparently unity likes to randomly throw this error... whatever...
				try { freshNoiseGraph = EditorGUILayout.ObjectField("Noise", NoiseGraph, typeof(NoiseGraph), false); }
				catch (Exception e) { if (!(e is ExitGUIException)) throw; }
				NoiseGraph = freshNoiseGraph as NoiseGraph;

				if (GraphLastUpdated < NoiseMakerWindow.ActiveLastUpdated || Graph == null && NoiseGraph != null)
				{
					GraphLastUpdated = DateTime.Now.Ticks;
					if (NoiseGraph == null) Graph = null;
					else Graph = NoiseGraph.GraphInstantiation;
					PreviewUpdating = false;
					PreviewLastUpdated = 0L;
					PreviewObjectEditor = null;
					PreviewTexture = null;
					PreviewMesh = null;
				}
			}
			GUILayout.EndArea();

			var leftOffset = position.width * (1f - Layouts.PreviewXOffsetScalar);
			var previewArea = new Rect(leftOffset, Layouts.PreviewOptionsHeight, (position.width - leftOffset) * Layouts.PreviewWidthScalar, position.height - Layouts.PreviewOptionsHeight);

			GUILayout.BeginArea(previewArea);
			{
				var rootNode = Graph == null ? null : Graph.RootNode;

				try
				{
					if (rootNode == null || rootNode.SourceIds == null || StringExtensions.IsNullOrWhiteSpace(rootNode.SourceIds.FirstOrDefault()) || (rootNode as Node<IModule>).GetValue(Graph) == null)
					{
						rootNode = null;
					}
				}
				catch
				{
					rootNode = null;
				}

				if (rootNode == null)
				{
					// A proper root with a source hasn't been defined.
					GUILayout.FlexibleSpace();
					GUILayout.BeginHorizontal();
					{
						GUILayout.FlexibleSpace();
						GUILayout.Label("Invalid Root", Styles.NoPreviewSmallLabel);
						GUILayout.FlexibleSpace();
					}
					GUILayout.EndHorizontal();
					GUILayout.FlexibleSpace();
				}
				else 
				{
					Previews[keys[PreviewSelected]](rootNode, new Rect(previewArea.x, previewArea.y, previewArea.width, previewArea.height), PreviewSelected);

					if (PreviewUpdating)
					{
						Pinwheel.Draw(new Vector2(previewArea.width * 0.5f, previewArea.height * 0.5f));
						Repaint();
					}
				}
			}
			GUILayout.EndArea();
		}

		void DrawDomains()
		{
			var height = Mathf.Max(Layouts.SelectedEditorsMinHeight, string.IsNullOrEmpty(DomainSelection) ? position.height - Layouts.HeaderHeight : (position.height * (1f - Layouts.SelectedEditorsHeightScalar)) - Layouts.HeaderHeight);

			GUILayout.BeginArea(new Rect(0f, Layouts.HeaderHeight, position.width * Layouts.DomainsWidthScalar, height));
			{
				DomainsScrollPosition = GUILayout.BeginScrollView(new Vector2(0f, DomainsScrollPosition.y), false, false, GUIStyle.none, GUIStyle.none);
				{
					var editors = DomainEditorCacher.Editors;
					var editorTypes = editors.Keys.ToArray();

					for (var editorTypeIndex = 0; editorTypeIndex < editorTypes.Length; editorTypeIndex++)
					{
						var editorType = editorTypes[editorTypeIndex];
						var editorEntry = editors[editorType];

						if (EditorFoldouts.Count <= editorTypeIndex) EditorFoldouts.Add(false);	

						if (EditorFoldouts[editorTypeIndex] = EditorGUILayout.Foldout(EditorFoldouts[editorTypeIndex], editorEntry.Details.Name, Styles.Foldout))
						{
							if (GUILayout.Button("Add New "+editorEntry.Details.Name))
							{
								var domain = Activator.CreateInstance(editorEntry.Details.Target) as Domain;
								domain.Id = Guid.NewGuid().ToString();
								Mercator.Domains.Add(domain);
								DomainSelection = domain.Id;
								BiomeSelection = null;
								AltitudeSelection = null;
							}

							int? deletedIndex = null;
							var domainCount = Mercator.Domains.Count;
							for (var domainIndex = 0; domainIndex < domainCount; domainIndex++)
							{
								var unmodifiedI = domainIndex;
								var unmodifiedDomain = Mercator.Domains[domainIndex];

								if (unmodifiedDomain.GetType() != editorEntry.Details.Target) continue;

								bool wasDeleted;
								bool wasSelected;
								bool alreadySelected = DomainSelection == unmodifiedDomain.Id;
								
								DrawDomain(editorEntry.Details.Name, alreadySelected, out wasSelected, out wasDeleted);
								
								if (wasSelected) 
								{
									if (alreadySelected) 
									{
                                        PreviewLastUpdated = 0L;
										DomainSelection = null;
										BiomeSelection = null;
										AltitudeSelection = null;
									}
									else 
									{
										DomainSelection = unmodifiedDomain.Id;
										BiomeSelection = null;
										AltitudeSelection = null;
									}
								}
								if (wasDeleted) 
								{
									deletedIndex = unmodifiedI;
									if (unmodifiedDomain.Id == DomainSelection) DomainSelection = null;
								}
							}

							if (deletedIndex.HasValue) Mercator.Remove(Mercator.Domains[deletedIndex.Value]);
						}
					}
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndArea();
		}

		Rect DrawDomain(string domainName, bool alreadySelected, out bool selected, out bool deleted)
		{
			GUI.color = alreadySelected ? Color.green : Color.white;

			GUILayout.BeginHorizontal();
			{
				deleted = GUILayout.Button("x", Styles.PreviewToolbarLeft, GUILayout.Width(24f));
				selected = GUILayout.Button(domainName, Styles.PreviewToolbarMiddle, GUILayout.Width(128f));
			}
			GUILayout.EndHorizontal();

			GUI.color = Color.white;

			return GUILayoutUtility.GetLastRect();
		}

		void DrawSelectedEditors(Domain domain)
		{
			var editorEntry = DomainEditorCacher.Editors.FirstOrDefault(e => e.Value.Details.Target == domain.GetType()).Value;
			var altitudeEditors = AltitudeEditorCacher.Editors.Values.OrderBy(e => e.Details.Name).ToList();

			var biome = string.IsNullOrEmpty(BiomeSelection) ? null : Mercator.Biomes.FirstOrDefault(b => b.Id == BiomeSelection);
			var altitude = string.IsNullOrEmpty(AltitudeSelection) ? null : Mercator.Altitudes.FirstOrDefault(a => a.Id == AltitudeSelection);

			var showDomain = biome == null;
			var showBiome = !showDomain && altitude == null;
			var showAltitude = altitude != null;

			var areaHeight = Mathf.Max(Layouts.SelectedEditorsMinHeight, position.height * Layouts.SelectedEditorsHeightScalar);
			var area = new Rect(0f, Mathf.Round(position.height - areaHeight), Mathf.Round(position.width * Layouts.SelectedEditorsWidthScalar), Mathf.Round(areaHeight));

			var domainHeaderArea = new Rect(area.x, area.y, showDomain ? area.width - Layouts.SelectedEditorsMaximizedWidthOffset : Layouts.SelectedEditorsMinimizedWidth, Layouts.SelectedEditorsHeaderHeight);
			var biomeHeaderArea = new Rect(domainHeaderArea.x + domainHeaderArea.width + Layouts.SelectedEditorsDivider, area.y, showBiome ? area.width - Layouts.SelectedEditorsMaximizedWidthOffset : Layouts.SelectedEditorsMinimizedWidth, Layouts.SelectedEditorsHeaderHeight);
			var altitudeHeaderArea = new Rect(biomeHeaderArea.x + biomeHeaderArea.width + Layouts.SelectedEditorsDivider, area.y, showAltitude ? area.width - Layouts.SelectedEditorsMaximizedWidthOffset : Layouts.SelectedEditorsMinimizedWidth, Layouts.SelectedEditorsHeaderHeight);

			var domainArea = new Rect(domainHeaderArea.x, domainHeaderArea.y + domainHeaderArea.height + Layouts.SelectedEditorsDivider, domainHeaderArea.width - 1f, area.height - domainHeaderArea.height - Layouts.SelectedEditorsDivider);
			var biomeArea = new Rect(biomeHeaderArea.x, biomeHeaderArea.y + biomeHeaderArea.height + Layouts.SelectedEditorsDivider, biomeHeaderArea.width - 1f, area.height - biomeHeaderArea.height - Layouts.SelectedEditorsDivider);
			var altitudeArea = new Rect(altitudeHeaderArea.x, altitudeHeaderArea.y + altitudeHeaderArea.height + Layouts.SelectedEditorsDivider, altitudeHeaderArea.width - 1f, area.height - altitudeHeaderArea.height - Layouts.SelectedEditorsDivider);

			GUI.color = showDomain ? Color.white : (showBiome ? GUI.color.NewV(0.85f) : GUI.color.NewV(0.75f));
			if (DrawSelectedEditorHeader(showDomain, domainHeaderArea, NoiseMakerConfig.Instance.DomainIcon, string.IsNullOrEmpty(domain.Name) ? editorEntry.Details.Name+" Domain" : domain.Name+" Domain"))
			{
				if (!showDomain)
				{
					BiomeSelection = null;
					AltitudeSelection = null;
				}
			}
			GUI.color = showBiome ? Color.white : GUI.color.NewV(0.85f);
			if (DrawSelectedEditorHeader(showBiome, biomeHeaderArea, NoiseMakerConfig.Instance.BiomeIcon, biome == null || string.IsNullOrEmpty(biome.Name) ? "Biome" : biome.Name+" Biome"))
			{
				if (showDomain) 
				{
					if (string.IsNullOrEmpty(domain.BiomeId)) EditorUtility.DisplayDialog("No Biome", "Select or create a biome from the domain panel first.", "Okay");
					else BiomeSelection = domain.BiomeId;
				}
				else if (showAltitude) AltitudeSelection = null;
			}
			GUI.color = showAltitude ? Color.white : (showDomain ? GUI.color.NewV(0.75f) : GUI.color.NewV(0.85f));
			if (DrawSelectedEditorHeader(showAltitude, altitudeHeaderArea, NoiseMakerConfig.Instance.AltitudeIcon, altitude == null ? "Altitude" : (string.IsNullOrEmpty(altitude.Name) ? altitudeEditors.FirstOrDefault(e => e.Details.Target == altitude.GetType()).Details.Name+" Altitude" : altitude.Name+" Altitude")))
			{
				PreviewUpdating = false;
				if (showBiome) EditorUtility.DisplayDialog("Select Altitude", "Select or create a an altitude from the biome panel first.", "Okay");
				else
				{
					BiomeSelection = domain.BiomeId;
					if (string.IsNullOrEmpty(domain.BiomeId)) EditorUtility.DisplayDialog("No Biome", "Select or create a biome from the domain panel first, then create an altitude to start editing.", "Okay");
				}
			}

			GUI.color = Color.white;

			GUI.color = showDomain ? Color.white : (showBiome ? GUI.color.NewV(0.7f) : GUI.color.NewV(0.55f));
			GUILayout.BeginArea(domainArea, GUI.skin.box);
			{
				if (showDomain)
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Label(editorEntry.Details.Description+".");
						if (string.IsNullOrEmpty(domain.Name))
						{
							if (GUILayout.Button("Convert Domain to Prefab", GUILayout.Width(Layouts.ConvertToPrefabButtonWidth)))
							{
								TextDialogPopup.Show(
									"Convert Domain to Prefab", 
									name =>
									{
										if (StringExtensions.IsNullOrWhiteSpace(name)) UnityEditor.EditorUtility.DisplayDialog("Invalid", "A Domain can't have a blank name.", "Okay"); 
										else if (Mercator.Domains.Any(d => d.Name == name)) UnityEditor.EditorUtility.DisplayDialog("Invalid", "A Domain with the name \""+name+"\" already exists.", "Okay");
										else domain.Name = name;
									},
									description: "Choose a unique name for this Domain."
								);
							}
						}
						else if (GUILayout.Button("Rename Domain", GUILayout.Width(Layouts.ConvertToPrefabButtonWidth)))
						{
							TextDialogPopup.Show(
								"Rename Domain", 
								name =>
								{
									if (StringExtensions.IsNullOrWhiteSpace(name)) UnityEditor.EditorUtility.DisplayDialog("Invalid", "A Domain can't have a blank name.", "Okay"); 
									else if (Mercator.Domains.Any(d => d.Name == name && d.Id != domain.Id)) UnityEditor.EditorUtility.DisplayDialog("Invalid", "A Domain with the name \""+name+"\" already exists.", "Okay");
									else domain.Name = name;
								},
								description: "Choose a unique name for this Domain."
							);
						}
					}
					GUILayout.EndHorizontal();

					DomainPreview preview;
					editorEntry.Editor.Draw(domain, PreviewModule, out preview);
					if (preview.Stale) PreviewUpdating = true;
					PreviewTexture = preview.Preview;

					GUILayout.FlexibleSpace();

					GUILayout.BeginHorizontal();
					{
						var biomeOptions = new List<string>(new [] {"Select a Biome...", "Create a New Biome"});
						var biomes = Mercator.Biomes.Where(b => !string.IsNullOrEmpty(b.Name)).OrderBy(b => b.Name);
						foreach (var orderedBiome in biomes) biomeOptions.Add(orderedBiome.Name);

						var selected = EditorGUILayout.Popup(0, biomeOptions.ToArray());

						if (1 < selected) 
						{
							biome = biomes.ToList()[selected - 2];
							domain.BiomeId = biome.Id;
							BiomeSelection = biome.Id;
						}
						else if (selected == 1) 
						{
							biome = new Biome();
							biome.Id = Guid.NewGuid().ToString();
							Mercator.Biomes.Add(biome);
							domain.BiomeId = biome.Id;
							BiomeSelection = biome.Id;
						}

						GUI.enabled = !string.IsNullOrEmpty(domain.BiomeId);
						if (GUILayout.Button("Edit "+(biome == null || string.IsNullOrEmpty(biome.Name) ? "Biome" : biome.Name))) BiomeSelection = domain.BiomeId;
						GUI.enabled = true;
					}
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndArea();

			var biomePreview = biome == null ? null : BiomeEditor.GetPreview(Mercator, domain, biome, PreviewModule);

			GUI.color = showBiome ? Color.white : GUI.color.NewV(0.7f);
			GUILayout.BeginArea(biomeArea, GUI.skin.box);
			{
				if (showBiome)
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Label("Add and link Altitudes to populate this Biome.");
						if (string.IsNullOrEmpty(biome.Name))
						{
							if (GUILayout.Button("Convert Biome to Prefab", GUILayout.Width(Layouts.ConvertToPrefabButtonWidth)))
							{
								TextDialogPopup.Show(
									"Convert Biome to Prefab", 
									name =>
									{
										if (StringExtensions.IsNullOrWhiteSpace(name)) EditorUtility.DisplayDialog("Invalid", "A Biome can't have a blank name.", "Okay"); 
										else if (Mercator.Biomes.Any(b => b.Name == name)) EditorUtility.DisplayDialog("Invalid", "A Biome with the name \""+name+"\" already exists.", "Okay");
										else biome.Name = name;
									},
									description: "Choose a unique name for this biome."
								);
							}
						}
						else if (GUILayout.Button("Rename Biome", GUILayout.Width(Layouts.ConvertToPrefabButtonWidth)))
						{
							TextDialogPopup.Show(
								"Rename Biome", 
								name =>
								{
									if (StringExtensions.IsNullOrWhiteSpace(name)) EditorUtility.DisplayDialog("Invalid", "A Biome can't have a blank name.", "Okay"); 
									else if (Mercator.Biomes.Any(b => b.Name == name && b.Id != biome.Id)) EditorUtility.DisplayDialog("Invalid", "A Biome with the name \""+name+"\" already exists.", "Okay");
									else biome.Name = name;
								},
								description: "Choose a unique name for this biome."
							);
						}
					}
					GUILayout.EndHorizontal();

					var altitudeOptions = new List<string>(new [] {"Select an Altitude...", "--- Create a New Altitude ---"});
					foreach (var orderedEditor in altitudeEditors) altitudeOptions.Add(orderedEditor.Details.Name);

					var existingIndex = altitudeOptions.Count;
					altitudeOptions.Add("--- Link Existing Altitude ---");
					var altitudes = Mercator.Altitudes.Where(a => !string.IsNullOrEmpty(a.Name)).OrderBy(a => a.Name).ToList();
					foreach (var orderedAltitude in altitudes) altitudeOptions.Add(orderedAltitude.Name);

					GUILayout.BeginHorizontal();
					{
						var selected = EditorGUILayout.Popup(0, altitudeOptions.ToArray());

						if (1 < selected && selected != existingIndex) 
						{
							if (selected < existingIndex)
							{
								// Creating a new altitude.
								altitude = Activator.CreateInstance(altitudeEditors.ToList()[selected - 2].Details.Target) as Altitude;
								altitude.Id = Guid.NewGuid().ToString();
								Mercator.Altitudes.Add(altitude);
								biome.AltitudeIds.Add(altitude.Id);
							}
							else
							{
								// Linking an existing altitude.
								var existingAltitude = altitudes[selected - existingIndex];
								Debug.Log("Linking "+existingAltitude.Name);
								if (biome.AltitudeIds.Contains(existingAltitude.Id)) EditorUtility.DisplayDialog("Invalid", "The Altitude \""+existingAltitude.Name+"\" already exists in this Biome.", "Okay");
								else biome.AltitudeIds.Add(existingAltitude.Id);
							}
							biomePreview.Stale = true;
						}
					}
					GUILayout.EndHorizontal();

					var biomeAltitudesArea = new Rect(4f, Layouts.AltitudesEditorOffsetY, biomeArea.width - 8f, biomeArea.height - Layouts.AltitudesEditorOffsetY - 4f);
					GUI.BeginGroup(biomeAltitudesArea);
					{
						var scrollArea = new Rect(0f, 4f, biomeAltitudesArea.width + 15f, biomeAltitudesArea.height - 4f); // 15 is for hiding the vertical scrollbar.
						var totalWidth = (Layouts.AltitudesEditorEntryWidth + Layouts.AltitudesEditorEntryDivider) * biome.AltitudeIds.Count;
						var topMargin = new Rect(0f, Layouts.AltitudesEditorEditHeight, Mathf.Max(totalWidth, biomeArea.width), Layouts.AltitudesEditorMarginHeight);
						var bottomMargin = new Rect(topMargin.x, biomeAltitudesArea.height - Layouts.AltitudesEditorMarginHeight - 20f - Layouts.AltitudesEditorDeleteHeight, topMargin.width, topMargin.height); // 20 is for offsetting from the horizontal scrollbar.
						GUI.Box(topMargin, GUIContent.none, NoiseMakerConfig.Instance.BiomeAltitudeWidgetMarginTop);
						GUI.Box(bottomMargin, GUIContent.none, NoiseMakerConfig.Instance.BiomeAltitudeWidgetMarginBottom);

						var sliderArea = new Rect(0f, topMargin.yMax, totalWidth, bottomMargin.y - topMargin.yMax);

						BiomeScrollPosition = GUI.BeginScrollView(scrollArea, new Vector2(BiomeScrollPosition.x, 0f), new Rect(0f, 0f, totalWidth, scrollArea.height), true, true);
						{
							string deletedId = null;
							for (var i = 0; i < biome.AltitudeIds.Count; i++) 
							{
								var unmodifiedAltitude = Mercator.Altitudes.FirstOrDefault(a => a.Id == biome.AltitudeIds[i]);
								bool deletedAltitude;
								DrawBiomeAltitude(unmodifiedAltitude, i, sliderArea, ref biomePreview.Stale, out deletedAltitude);
								if (deletedAltitude) deletedId = unmodifiedAltitude.Id;
							}
							if (!string.IsNullOrEmpty(deletedId)) biome.AltitudeIds.Remove(deletedId);
						}
						GUI.EndScrollView();

					}
					GUI.EndGroup();
				}
			}
			GUILayout.EndArea();

			GUI.color = showAltitude ? Color.white : GUI.color.NewV(0.55f);
			GUILayout.BeginArea(altitudeArea, GUI.skin.box);
			{
				if (showAltitude)
				{
					var altitudeEditor = altitudeEditors.FirstOrDefault(e => e.Details.Target == altitude.GetType());
					GUILayout.Label(altitudeEditor.Details.Description+".");
					try
					{
						altitudeEditor.Editor.Draw(altitude, ref biomePreview.Stale);
					}
					catch (Exception e)
					{
						if (e.GetType() != typeof(ExitGUIException)) Debug.LogException(e);
					}
				}
			}
			GUILayout.EndArea();

			if ((showBiome || showAltitude) && biomePreview != null)
			{
				if (biomePreview.Stale) PreviewUpdating = true;
				PreviewTexture = biomePreview.Preview;
			}
		}

		void DrawBiomeAltitude(Altitude altitude, int index, Rect sliderArea, ref bool changed, out bool deleted)
		{
			var hint = string.IsNullOrEmpty(altitude.Name) ? AltitudeEditorCacher.Editors[altitude.GetType()].Details.Name+" Altitude" : altitude.Name;
			var xPos = (Layouts.AltitudesEditorEntryWidth + Layouts.AltitudesEditorEntryDivider) * (float)index;

			var maxFromTop = sliderArea.y + (sliderArea.height * ((1f - altitude.MaxAltitude) / 2f));
			var minFromTop = sliderArea.y + (sliderArea.height * ((1f - altitude.MinAltitude) / 2f));

			var editButtonArea = new Rect(xPos, sliderArea.y - Layouts.AltitudesEditorTabHeight - Layouts.AltitudesEditorEditHeight, Layouts.AltitudesEditorEntryWidth, Layouts.AltitudesEditorEditHeight);
			var deleteButtonArea = new Rect(xPos, sliderArea.yMax + Layouts.AltitudesEditorTabHeight, Layouts.AltitudesEditorEntryWidth, Layouts.AltitudesEditorDeleteHeight);
			var middleSliderArea = new Rect(xPos, maxFromTop, Layouts.AltitudesEditorEntryWidth, minFromTop - maxFromTop);

			var topTabArea = new Rect(middleSliderArea.x, middleSliderArea.y - Layouts.AltitudesEditorTabHeight, Layouts.AltitudesEditorEntryWidth, Layouts.AltitudesEditorTabHeight);
			var bottomTabArea = new Rect(middleSliderArea.x, middleSliderArea.yMax, Layouts.AltitudesEditorEntryWidth, Layouts.AltitudesEditorTabHeight);

			if (GUI.Button(editButtonArea, new GUIContent(string.Empty, hint), NoiseMakerConfig.Instance.BiomeAltitudeEditWidget))
			{
				AltitudeSelection = altitude.Id;
			}

			deleted = (GUI.Button(deleteButtonArea, new GUIContent(string.Empty, "Delete Altitude"), NoiseMakerConfig.Instance.BiomeAltitudeDeleteWidget));
			changed = changed || deleted;

			if (GUI.RepeatButton(middleSliderArea, new GUIContent(string.Empty, "Drag Altitude"), NoiseMakerConfig.Instance.BiomeAltitudeWidgetLine))
			{
				var altitudeDelta = (middleSliderArea.center - Event.current.mousePosition).y / sliderArea.height;
				altitude.MaxAltitude = Mathf.Clamp(altitude.MaxAltitude + altitudeDelta, -1, 1f);
				altitude.MinAltitude = Mathf.Clamp(altitude.MinAltitude + altitudeDelta, -1f, 1f);
				changed = true;
				Repaint();
			}
			if (GUI.RepeatButton(topTabArea, new GUIContent(string.Empty, "Max Altitude"), NoiseMakerConfig.Instance.BiomeAltitudeWidgetTop))
			{
				var delta = topTabArea.center - Event.current.mousePosition;
				altitude.MaxAltitude = Mathf.Clamp(altitude.MaxAltitude + (delta.y / sliderArea.height), -1, 1f);
				if (altitude.MaxAltitude <= altitude.MinAltitude) altitude.MinAltitude = altitude.MaxAltitude;
				changed = true;
				Repaint();
			}
			if (GUI.RepeatButton(bottomTabArea, new GUIContent(string.Empty, "Min Altitude"), NoiseMakerConfig.Instance.BiomeAltitudeWidgetBottom))
			{
				var delta = bottomTabArea.center - Event.current.mousePosition;
				altitude.MinAltitude = Mathf.Clamp(altitude.MinAltitude + (delta.y / sliderArea.height), -1, 1f);
				if (altitude.MaxAltitude <= altitude.MinAltitude) altitude.MaxAltitude = altitude.MinAltitude;
				changed = true;
				Repaint();
			}
		}

		bool DrawSelectedEditorHeader(bool active, Rect area, Texture2D icon, string headerTitle)
		{
			var value = false;

			GUILayout.BeginArea(area);
			{
				GUILayout.BeginHorizontal();
				{
					if (active)
					{
						GUILayout.Box(icon, Styles.MercatorSelectionEditorHeader, GUILayout.Width(Layouts.SelectedEditorsMinimizedWidth), GUILayout.ExpandHeight(true));
						GUILayout.Box(headerTitle, Styles.MercatorSelectionEditorHeader, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
					}
					else if (GUILayout.Button(icon, Styles.MercatorSelectionEditorHeader, GUILayout.Width(Layouts.SelectedEditorsMinimizedWidth), GUILayout.ExpandHeight(true))) value = true;
				}
				GUILayout.EndHorizontal();
				// Add one pixel for shadow.
				GUILayout.Space(1f);
			}
			GUILayout.EndArea();

			return value;
		}

		#region Previews
		/// <summary>
		/// Draws the flat preview.
		/// </summary>
		/// <param name="node">Node to draw, typically the root.</param>
		/// <param name="area">Area the editor should take up.</param>
		// todo: consolidate this logic somewhere so it's not duplicated here and NoiseMakerWindow
		void DrawPlanePreview(Node<IModule> node, Rect area, int index)
		{
			var lastUpdate = NodeEditor.LastUpdated(node.Id);
			// todo: remove these duplicate update checks.
			// Check if node has been updated since the last time we had drawn it.
			if (lastUpdate != PreviewLastUpdated) 
			{
				PreviewUpdating = true;
				var module = node.GetValue(Graph);
				PreviewModule = module;

				PreviewTexture = NoiseMakerWindow.GetPlaneTexture(module, (int)area.width, (int)area.height, Mercator, completed: () => PreviewUpdating = (PreviewLastUpdated == lastUpdate && PreviewSelected == index && PreviewModule == module) ? false : PreviewUpdating);

				PreviewLastUpdated = lastUpdate;

				Repaint();
			}
			GUI.DrawTexture(new Rect(2f, 2f, area.width - 4f, area.height - 4f), PreviewTexture);
		}

		/// <summary>
		/// Draws the sphere preview.
		/// </summary>
		/// <param name="node">Node to draw, typically the root.</param>
		/// <param name="area">Area the editor should take up.</param>
		// todo: consolidate this logic somewhere so it's not duplicated here and NoiseMakerWindow
		void DrawSpherePreview(Node<IModule> node, Rect area, int index)
		{
			// Reset mesh, incase another preview has modified it.
			NoiseMakerConfig.Instance.Ico4Vertex.GetComponent<MeshFilter>().sharedMesh = NoiseMakerConfig.Instance.Ico4VertexMesh;

			var lastUpdate = NodeEditor.LastUpdated(node.Id);
			// todo: remove these duplicate update checks.
			// Check if node has been updated since the last time we had drawn it.
			if (lastUpdate != PreviewLastUpdated) 
			{
				PreviewUpdating = true;
				var module = node.GetValue(Graph);
				var sphere = new Sphere(module);
				PreviewModule = sphere;

				PreviewTexture = NoiseMakerWindow.GetSphereTexture(module, map: Mercator, completed: () => PreviewUpdating = (PreviewLastUpdated == lastUpdate && PreviewSelected == index && PreviewModule == sphere) ? false : PreviewUpdating);

				PreviewLastUpdated = lastUpdate;

				Repaint();
			}
			// Reset material
			var mat = NoiseMakerConfig.Instance.Ico4Vertex.GetComponent<MeshRenderer>().sharedMaterial;
			// If material hasn't been set, set it.
			if (mat.mainTexture != PreviewTexture)
			{
				mat.mainTexture = PreviewTexture;
				Repaint();
			}

			if (PreviewObjectEditor == null) PreviewObjectEditor = Editor.CreateEditor(NoiseMakerConfig.Instance.Ico4Vertex);
			// Draw interactable preview.
			PreviewObjectEditor.OnPreviewGUI(new Rect(1f, 0f, area.width - 1f, area.height), Styles.Blank);
		}

		/// <summary>
		/// Draws the elevation preview.
		/// </summary>
		/// <param name="node">Node to draw, typically the root.</param>
		/// <param name="area">Area the editor should take up.</param>
		// todo: consolidate this logic somewhere so it's not duplicated here and NoiseMakerWindow
		void DrawElevationPreview(Node<IModule> node, Rect area, int index)
		{
			var lastUpdate = NodeEditor.LastUpdated(node.Id);
			// todo: remove these duplicate update checks.
			// Check if node has been updated since the last time we had drawn it.
			if (lastUpdate != PreviewLastUpdated) 
			{
				// Reset mesh
				NoiseMakerConfig.Instance.Ico5Vertex.GetComponent<MeshFilter>().sharedMesh = NoiseMakerConfig.Instance.Ico5VertexMesh;

				if (PreviewMesh == null) PreviewMesh = Instantiate(NoiseMakerConfig.Instance.Ico5VertexMesh);

				var module = node.GetValue(Graph);
				var sphere = new Sphere(module);
				PreviewModule = sphere;

				var verts = PreviewMesh.vertices;
				Graph.GetSphereAltitudes(sphere, ref verts, 0.75f);
				PreviewMesh.vertices = verts;

				PreviewUpdating = true;
                PreviewTexture = NoiseMakerWindow.GetSphereTexture(module, map: Mercator, completed: () => PreviewUpdating = (PreviewLastUpdated == lastUpdate && PreviewSelected == index && PreviewModule == sphere) ? false : PreviewUpdating);

				PreviewLastUpdated = lastUpdate;

				Repaint();
			}
			var filter = NoiseMakerConfig.Instance.Ico5Vertex.GetComponent<MeshFilter>();

			if (filter.sharedMesh != PreviewMesh)
			{
				filter.sharedMesh = PreviewMesh;
				Repaint();
			}

			var mat = NoiseMakerConfig.Instance.Ico5Vertex.GetComponent<MeshRenderer>().sharedMaterial;

			if (mat.mainTexture != PreviewTexture)
			{
				mat.mainTexture = PreviewTexture;
				Repaint();
			}

			if (PreviewObjectEditor == null) PreviewObjectEditor = Editor.CreateEditor(NoiseMakerConfig.Instance.Ico5Vertex);
			// Draw interactable preview.
			PreviewObjectEditor.OnPreviewGUI(new Rect(1f, 0f, area.width - 1f, area.height), Styles.Blank);
		}
		#endregion

		void Reset()
		{
			State = States.Splash;
			SaveGuid = null;
			Mercator = null;
			DomainSelection = null;

			PreviewLastUpdated = 0L;
			PreviewTexture = null;
			PreviewMesh = null;
			PreviewObjectEditor = null;
			PreviewModule = null;
		}

		void Save()
		{
			if (State != States.Idle || Mercator == null) return;
			if (StringExtensions.IsNullOrWhiteSpace(SavePath)) throw new NullReferenceException("SavePath cannot be null");
			var config = AssetDatabase.LoadAssetAtPath<MercatorMap>(SavePath);

			if (config == null)
			{
				UnityEditor.EditorUtility.DisplayDialog("Missing Mercator Map", "The Mercator Map you were editing is now missing.", "Okay");
				Reset();
				return;
			}

			config.MercatorInstantiation = Mercator;
			UnityEditor.EditorUtility.SetDirty(config);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public void Open(string path)
		{
			if (StringExtensions.IsNullOrWhiteSpace(path)) return;

			var fromRoot = path.StartsWith(Application.dataPath);
			var fromAssets = !fromRoot && path.StartsWith("Assets");

			if (fromRoot || fromAssets)
			{
				if (State != States.Splash) 
				{
					var result = UnityEditor.EditorUtility.DisplayDialogComplex("Editing in Progress", "You're in the middle of editing another Mercator Map, what would you like to do?", "Save", "Cancel", "Discard Changes");

					if (result == 0) Save();
					else if (result == 1) return;

					Reset();
				}

				// Open up existing file for editing.
				SaveGuid = AssetDatabase.AssetPathToGUID(fromAssets ? path : "Assets"+path.Substring(Application.dataPath.Length));
				var config = AssetDatabase.LoadAssetAtPath<MercatorMap>(SavePath);
				Mercator = config.MercatorInstantiation;
				State = States.Idle;

				Repaint();
			}
			else UnityEditor.EditorUtility.DisplayDialog("Invalid", "Selected mercator map must be inside project directory.", "Okay");	
		}

		public static void OpenMercatorMap(string path)
		{
			if (StringExtensions.IsNullOrWhiteSpace(path)) return;

			if (Instance == null) Init();

			Instance.Open(path);
		}

		public static string ActiveSavePath { get { return Instance == null || StringExtensions.IsNullOrWhiteSpace(Instance.SavePath) ? null : Instance.SavePath; } }

		public static string ActiveDomainId { get { return Instance == null ? null : Instance.DomainSelection; } }

		public static string ActiveBiomeId { get { return Instance == null ? null : Instance.BiomeSelection; } }

		public static void RepaintNow()
		{
			if (Instance != null) Instance.Repaint();
		}

		public static void QueueRepaint()
		{
			RepaintQueued = true;
		}
		[Interloper.InterloperLinked]
		static void LolNullIt()
		{
			Instance.Graph = null;
		}
	}
}