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

		[SerializeField]
		States State = States.Splash;
		[SerializeField]
		string SaveGuid;
		[SerializeField]
		NoiseGraph NoiseGraph;
		[SerializeField]
		Vector2 DomainsScrollPosition = Vector2.zero;
		[SerializeField]
		int DomainSelection = -1;

		Graph Graph;
		int PreviewSelected;
		Dictionary<string, Action<Node<IModule>, Rect, int>> Previews;
		long PreviewLastUpdated;
		Texture2D PreviewTexture;
		Mesh PreviewMesh;
		Editor PreviewObjectEditor;
		bool PreviewUpdating;

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
					if (0 <= DomainSelection) DrawDomainEditor(null);
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
			var height = DomainSelection < 0 ? position.height - Layouts.HeaderHeight : (position.height * (1f - Layouts.DomainEditorHeightScalar)) - Layouts.HeaderHeight;

			GUILayout.BeginArea(new Rect(0f, Layouts.HeaderHeight, position.width * Layouts.DomainsWidthScalar, height));
			{
				DomainsScrollPosition = GUILayout.BeginScrollView(new Vector2(0f, DomainsScrollPosition.y), false, false, GUIStyle.none, GUIStyle.none);
				{
					int? deletedIndex = null;

					for (var i = 0; i < 100; i++)
					{
						var unmodifiedI = i;

						bool wasDeleted;
						bool wasSelected;
						bool alreadySelected = DomainSelection == unmodifiedI;

						DrawDomain(null, alreadySelected, out wasSelected, out wasDeleted);

						if (wasSelected) 
						{
							if (alreadySelected) DomainSelection = -1;
							else DomainSelection = unmodifiedI;
						}
						if (wasDeleted) deletedIndex = unmodifiedI;
					}
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndArea();
		}

		Rect DrawDomain(Domain domain, bool alreadySelected, out bool selected, out bool deleted)
		{
			GUI.color = alreadySelected ? Color.magenta : Color.white;

			GUILayout.BeginHorizontal();
			{
				deleted = GUILayout.Button("x", Styles.PreviewToolbarLeft, GUILayout.Width(24f));
				selected = GUILayout.Button("Lol", Styles.PreviewToolbarMiddle, GUILayout.Width(128f));
			}
			GUILayout.EndHorizontal();

			GUI.color = Color.white;

			return GUILayoutUtility.GetLastRect();
		}

		void DrawDomainEditor(Domain domain)
		{
			GUILayout.BeginArea(new Rect(0f, position.height - (position.height * Layouts.DomainEditorHeightScalar), position.width * Layouts.DomainEditorWidthScalar, position.height * Layouts.DomainEditorHeightScalar));
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Box("Lol", EditorStyles.miniButtonMid);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();
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

				var module = node.GetValue(Graph);
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
				PreviewTexture = NoiseMakerWindow.GetSphereTexture(node.GetValue(Graph), completed: () => PreviewUpdating = (PreviewLastUpdated == lastUpdate && PreviewSelected == index) ? false : PreviewUpdating);

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

				var verts = PreviewMesh.vertices;
				Graph.GetSphereAltitudes(sphere, ref verts, 0.75f);
				PreviewMesh.vertices = verts;

				PreviewUpdating = true;
				PreviewTexture = NoiseMakerWindow.GetSphereTexture(module, completed: () => PreviewUpdating = (PreviewLastUpdated == lastUpdate && PreviewSelected == index) ? false : PreviewUpdating);

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

		/*
		public Texture2D GetPreview(Latitude latitude, int width, int height)
		{
			var texture = new Texture2D(width, height);

			var pixels = new Color[width * height];
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					pixels[(width * y) + x] = latitude.GetColor(0f, 0f, (float)x / (float)width);
				}
			}
			texture.SetPixels(pixels);
			texture.Apply();
			return texture;
		}
		*/

		void Reset()
		{
			State = States.Splash;
			SaveGuid = null;
			Mercator = null;
			DomainSelection = -1;
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
	}
}