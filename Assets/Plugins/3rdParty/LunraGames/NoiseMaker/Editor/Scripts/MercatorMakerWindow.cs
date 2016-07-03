﻿using UnityEditor;
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

			public const float SelectedEditorsHeightScalar = 0.4f;
			public const float SelectedEditorsWidthScalar = 1f - PreviewXOffsetScalar;

			public const float SelectedEditorsDivider = 2f;
			public const float SelectedEditorsMinimizedWidth = 28f;
			public const float SelectedEditorsHeaderHeight = SelectedEditorsMinimizedWidth;
			public const float SelectedEditorsMaximizedWidthOffset = 2f * (SelectedEditorsDivider + SelectedEditorsMinimizedWidth);
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

		Graph Graph;
		int PreviewSelected;
		Dictionary<string, Action<Node<IModule>, Rect, int>> Previews;
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
							var savePath = UnityEditor.EditorUtility.SaveFilePanelInProject("New Mercator Map", "Mercator", "asset", null);
							if (!StringExtensions.IsNullOrWhiteSpace(savePath))
							{
								var config = MercatorMap.CreateInstance<MercatorMap>();
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
					{ "Flat", DrawFlatPreview },
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
							PreviewMesh = null;
						}
					}
				}
				GUILayout.EndHorizontal();

				UnityEngine.Object freshNoiseGraph = null;
				// Apparently unity likes to randomly throw this error... whatever...
				try { freshNoiseGraph = EditorGUILayout.ObjectField("Noise", NoiseGraph, typeof(NoiseGraph), false); }
				catch (Exception e) { if (!(e is ExitGUIException)) throw; }

				if (freshNoiseGraph != NoiseGraph || (Graph == null && NoiseGraph != null))
				{
					// When previews change, we reset any cached info.
					PreviewUpdating = false;
					PreviewLastUpdated = 0L;
					PreviewObjectEditor = null;
					PreviewMesh = null;
					NoiseGraph = freshNoiseGraph as NoiseGraph;

					if (NoiseGraph == null) Graph = null;
					else Graph = NoiseGraph.GraphInstantiation;
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
			var height = string.IsNullOrEmpty(DomainSelection) ? position.height - Layouts.HeaderHeight : (position.height * (1f - Layouts.SelectedEditorsHeightScalar)) - Layouts.HeaderHeight;

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
										DomainSelection = null;
										PreviewLastUpdated = DateTime.Now.Ticks;
									}
									else DomainSelection = unmodifiedDomain.Id;
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
			GUI.color = alreadySelected ? Color.magenta : Color.white;

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

			var biome = string.IsNullOrEmpty(BiomeSelection) ? null : Mercator.Biomes.FirstOrDefault(b => b.Id == BiomeSelection);
			var altitude = string.IsNullOrEmpty(AltitudeSelection) ? null : Mercator.Altitudes.FirstOrDefault(a => a.Id == AltitudeSelection);

			var showDomain = biome == null;
			var showBiome = !showDomain && altitude == null;
			var showAltitude = altitude != null;

			var area = new Rect(0f, Mathf.Round(position.height - (position.height * Layouts.SelectedEditorsHeightScalar)), Mathf.Round(position.width * Layouts.SelectedEditorsWidthScalar), Mathf.Round(position.height * Layouts.SelectedEditorsHeightScalar));

			var domainHeaderArea = new Rect(area.x, area.y, showDomain ? area.width - Layouts.SelectedEditorsMaximizedWidthOffset : Layouts.SelectedEditorsMinimizedWidth, Layouts.SelectedEditorsHeaderHeight);
			var biomeHeaderArea = new Rect(domainHeaderArea.x + domainHeaderArea.width + Layouts.SelectedEditorsDivider, area.y, showBiome ? area.width - Layouts.SelectedEditorsMaximizedWidthOffset : Layouts.SelectedEditorsMinimizedWidth, Layouts.SelectedEditorsHeaderHeight);
			var altitudeHeaderArea = new Rect(biomeHeaderArea.x + biomeHeaderArea.width + Layouts.SelectedEditorsDivider, area.y, showAltitude ? area.width - Layouts.SelectedEditorsMaximizedWidthOffset : Layouts.SelectedEditorsMinimizedWidth, Layouts.SelectedEditorsHeaderHeight);

			var domainArea = new Rect(domainHeaderArea.x, domainHeaderArea.y + domainHeaderArea.height + Layouts.SelectedEditorsDivider, domainHeaderArea.width - 1f, area.height - domainHeaderArea.height - Layouts.SelectedEditorsDivider);
			var biomeArea = new Rect(biomeHeaderArea.x, biomeHeaderArea.y + biomeHeaderArea.height + Layouts.SelectedEditorsDivider, biomeHeaderArea.width - 1f, area.height - biomeHeaderArea.height - Layouts.SelectedEditorsDivider);
			var altitudeArea = new Rect(altitudeHeaderArea.x, altitudeHeaderArea.y + altitudeHeaderArea.height + Layouts.SelectedEditorsDivider, altitudeHeaderArea.width - 1f, area.height - altitudeHeaderArea.height - Layouts.SelectedEditorsDivider);

			if (DrawSelectedEditorHeader(showDomain, domainHeaderArea, NoiseMakerConfig.Instance.DomainIcon, editorEntry.Details.Name))
			{
				if (!showDomain)
				{
					BiomeSelection = null;
					AltitudeSelection = null;
				}
			}
			if (!showBiome) GUI.color = GUI.color.NewV(0.85f);
			if (DrawSelectedEditorHeader(showBiome, biomeHeaderArea, NoiseMakerConfig.Instance.BiomeIcon, biome == null || string.IsNullOrEmpty(biome.Name) ? "Biome" : biome.Name))
			{
				if (showDomain) 
				{
					if (string.IsNullOrEmpty(domain.BiomeId)) UnityEditor.EditorUtility.DisplayDialog("No Biome", "Select or create a biome from the domain panel first.", "Okay");
					else BiomeSelection = domain.BiomeId;
				}
				else if (showAltitude) AltitudeSelection = null;
			}
			if (!showAltitude) GUI.color = GUI.color.NewV(0.75f);
			if (DrawSelectedEditorHeader(showAltitude, altitudeHeaderArea, NoiseMakerConfig.Instance.AltitudeIcon, "altitude name"))
			{
				if (showBiome) UnityEditor.EditorUtility.DisplayDialog("Select Altitude", "Select or create a an altitude from the biome panel first.", "Okay");
				else
				{
					BiomeSelection = domain.BiomeId;
					if (string.IsNullOrEmpty(domain.BiomeId)) UnityEditor.EditorUtility.DisplayDialog("No Biome", "Select or create a biome from the domain panel first, then create an altitude to start editing.", "Okay");
				}
			}

			GUI.color = Color.white;

			GUILayout.BeginArea(domainArea, GUI.skin.box);
			{
				if (showDomain)
				{
					Texture2D preview;
					editorEntry.Editor.Draw(Mercator, domain, PreviewModule, out preview);
					PreviewTexture = preview;

					GUILayout.FlexibleSpace();

					var biomeOptions = new List<string>(new [] {"Select a Biome...", "Create a New Biome"});
					var biomes = Mercator.Biomes.OrderBy(b => b.Name);
					var unnamedCount = 0;

					foreach (var orderedBiome in biomes)
					{
						biomeOptions.Add(string.IsNullOrEmpty(orderedBiome.Name) ? "Unnamed "+unnamedCount : orderedBiome.Name);
						if (orderedBiome.Name == null) unnamedCount++;
					}

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
			}
			GUILayout.EndArea();

			GUI.color = showBiome ? Color.white : GUI.color.NewV(0.7f);
			GUILayout.BeginArea(biomeArea, GUI.skin.box);
			{
				if (showBiome)
				{
					
				}
			}
			GUILayout.EndArea();

			GUI.color = showAltitude ? Color.white : GUI.color.NewV(0.55f);
			GUILayout.BeginArea(altitudeArea, GUI.skin.box);
			{
				if (showAltitude)
				{
					
				}
			}
			GUILayout.EndArea();
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
		void DrawFlatPreview(Node<IModule> node, Rect area, int index)
		{
			var lastUpdate = NodeEditor.LastUpdated(node.Id);
			// todo: remove these duplicate update checks.
			// Check if node has been updated since the last time we had drawn it.
			if (lastUpdate != PreviewLastUpdated) 
			{
				if (PreviewTexture == null) PreviewTexture = new Texture2D((int)area.width, (int)area.height);

				PreviewUpdating = true;
				var module = node.GetValue(Graph);
				PreviewModule = module;

				var pixels = new Color[PreviewTexture.width * PreviewTexture.height];
				for (var x = 0; x < PreviewTexture.width; x++)
				{
					for (var y = 0; y < PreviewTexture.height; y++)
					{
						// Get the value from the specified location, and run it through the selected previewer. 
						var value = (float)module.GetValue((double)x, (double)y, 0.0);
						pixels[(PreviewTexture.width * y) + x] = NodeEditor.Previewer.Calculate(value, NodeEditor.Previewer);
					}
				}
				PreviewTexture.SetPixels(pixels);
				PreviewTexture.Apply();

				PreviewLastUpdated = lastUpdate;

				Repaint();
			}
			GUI.DrawTexture(new Rect(2f, 2f, PreviewTexture.width - 4f, PreviewTexture.height -4f), PreviewTexture);
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

				PreviewTexture = NoiseMakerWindow.GetSphereTexture(module, completed: () => PreviewUpdating = (PreviewLastUpdated == lastUpdate && PreviewSelected == index && PreviewModule == sphere) ? false : PreviewUpdating);

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
				PreviewTexture = NoiseMakerWindow.GetSphereTexture(module, completed: () => PreviewUpdating = (PreviewLastUpdated == lastUpdate && PreviewSelected == index && PreviewModule == sphere) ? false : PreviewUpdating);

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

		public static void QueueRepaint()
		{
			RepaintQueued = true;
		}
	}
}