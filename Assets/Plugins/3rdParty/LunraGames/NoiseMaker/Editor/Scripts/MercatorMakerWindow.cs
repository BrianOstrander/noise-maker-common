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
			public const float SelectionEditorWidth = 64f;
			public const float DomainsWidthScalar = (1f - PreviewXOffsetScalar) * 0.9f;
			public const float DomainEditorHeightScalar = 0.4f;
			public const float DomainEditorWidthScalar = 0.5f;
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
					if (!string.IsNullOrEmpty(DomainSelection)) DrawDomainEditor(Mercator.Domains.FirstOrDefault(d => d.Id == DomainSelection));
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
			var height = string.IsNullOrEmpty(DomainSelection) ? position.height - Layouts.HeaderHeight : (position.height * (1f - Layouts.DomainEditorHeightScalar)) - Layouts.HeaderHeight;

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

		void DrawDomainEditor(Domain domain)
		{
			var area = new Rect(0f, position.height - (position.height * Layouts.DomainEditorHeightScalar), position.width * Layouts.DomainEditorWidthScalar, position.height * Layouts.DomainEditorHeightScalar);
			var headerArea = new Rect(area.x, area.y, area.width, 38f);
			var contentArea = new Rect(0f, headerArea.y + headerArea.height, area.width, area.height - headerArea.height);

			GUILayout.BeginArea(contentArea, Styles.BoxButton);
			{
				GUILayout.BeginHorizontal();
				{
					var editorEntry = DomainEditorCacher.Editors.FirstOrDefault(e => e.Value.Details.Target == domain.GetType()).Value;

					// Domain editor
					if (string.IsNullOrEmpty(BiomeSelection)) 
					{
						// No Biome is selected for editing, so show the Domain editor.
						Texture2D preview;
						editorEntry.Editor.Draw(Mercator, domain, PreviewModule, out preview);
						PreviewTexture = preview;
						GUILayout.Box(editorEntry.Details.Name, Styles.OptionButtonMiddle);
					}
					else GUILayout.Box("", Styles.OptionButtonMiddle, GUILayout.Width(Layouts.SelectionEditorWidth));
					// Biome editor

					// Altitude editor


				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();

			GUILayout.BeginArea(headerArea);
			{
				GUILayout.BeginHorizontal();
				{
					if (string.IsNullOrEmpty(BiomeSelection)) GUILayout.Box("Domain Header", Styles.OptionButtonMiddle);
					else 
					{
						GUILayout.Box("Domain Header", Styles.OptionButtonMiddle, GUILayout.Width(Layouts.SelectionEditorWidth));

						if (string.IsNullOrEmpty(AltitudeSelection)) GUILayout.Box("Biome Header", Styles.OptionButtonMiddle);
						else
						{
							GUILayout.Box("Biome Header", Styles.OptionButtonMiddle, GUILayout.Width(Layouts.SelectionEditorWidth));

							GUILayout.Box("Altitude Header", Styles.OptionButtonMiddle);
						}
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();

			/*
			var headerArea = new Rect(area.x, area.y, area.width, 38f);
			var contentArea = new Rect(area.x, area.y + headerArea.height - 2, headerArea.width, area.height + 4f - headerArea.height);

			var editorEntry = DomainEditorCacher.Editors.FirstOrDefault(e => e.Value.Details.Target == domain.GetType()).Value;

			GUILayout.BeginArea(contentArea, EditorStyles.helpBox);
			{
				GUILayout.Space(2f);

				GUILayout.BeginHorizontal();
				{
					Texture2D preview;
					editorEntry.Editor.Draw(Mercator, domain, PreviewModule, out preview);
					PreviewTexture = preview;

				}
				GUILayout.EndHorizontal();

				GUILayout.Space(4f);
			}
			GUILayout.EndArea();

			GUI.Box(headerArea, editorEntry.Details.Name, Styles.OptionButtonMiddle);
			*/
		}

		void DrawBiome(Biome biome)
		{
			
		}

		void DrawAltitudeEditor(Altitude altitude)
		{

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