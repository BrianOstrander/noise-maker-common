using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using Atesh;
using LibNoise.Models;

namespace LunraGames.NoiseMaker
{
	public class NoiseMakerWindow : EditorWindow
	{
		const string GraphKey = "LG_NoiseMaker_Graph";

		public class Layouts
		{
			public const float VisualizationOptionsWidth = 300f;
			public const float VisualizationOptionsHeight = 24f;
			public const float VisualizationHeight = VisualizationOptionsHeight * 4f;
			public const float NodeOptionsWidth = 240f;
			public const float PreviewWidth = NodeOptionsWidth + 13f;
			public const float PreviewHeight = PreviewWidth;
		}

		// todo: Make this dynamic and settable in editor
		public const float SphereScalar = 0.65f;

		enum States
		{
			Splash,
			Idle
		}

		[SerializeField]
		States State = States.Splash;
		[SerializeField]
		int Visualization;
		[SerializeField]
		bool VisualizationShown;
		[SerializeField]
		Vector2 NodeOptionsScrollPosition = Vector2.zero;
		[SerializeField]
		string SavePath;
		[SerializeField]
		Vector3 GraphPosition = Vector3.zero;

		Graph Graph;
		Node ConnectingFrom;
		Node ConnectingTo;
		Dictionary<string, bool> ShownCategories = new Dictionary<string, bool>();
		int PreviewSelected;
		Dictionary<string, Action<Node, Rect>> Previews;
		long PreviewLastUpdated;
		Texture2D PreviewTexture;
		Mesh PreviewMesh;
		Editor PreviewObjectEditor;

		[MenuItem ("Window/Lunra Games/Noise Maker/Editor")]
		static void Init () 
		{
			var window = EditorWindow.GetWindow(typeof (NoiseMakerWindow), false, "Noise Maker") as NoiseMakerWindow;
			window.titleContent = new GUIContent("Noise Maker", NoiseMakerConfig.Instance.AuthorTab);
			window.Show();
		}

		#region Messages
		void OnGUI()
		{
			try 
			{
				// If we're opening the editor from a cold start, and it looks like the user was editing something, load it.
				if (State == States.Idle && Graph == null && !StringExtensions.IsNullOrWhiteSpace(SavePath))
				{
					var config = AssetDatabase.LoadAssetAtPath<NoiseGraph>(SavePath);
					if (config == null) State = States.Splash;
					else Graph = config.GraphInstantiation;
				}
				
				if (State == States.Splash) DrawSplash();
				else if (State == States.Idle)
				{
					if (NodeEditor.Previewer == null) NodeEditor.Previewer = NodeEditor.Visualizations[Visualization];
					DrawGraph();
					DrawVisualizationOptions();
					// Reset back to splash.
					if (GUI.Button(new Rect(Layouts.VisualizationOptionsWidth, 0f, 128f, 24f), "Reset", Styles.ToolbarButtonMiddle)) Reset();
					// Save active file.
					if (GUI.Button(new Rect(Layouts.VisualizationOptionsWidth + 128f, 0f, 128f, 24f), "Save", Styles.ToolbarButtonRight)) Save();

					DrawNodeOptions();
					DrawPreview();

					// Detect if they're dragging or scrolling around.
					if (Event.current != null && Event.current.isMouse && Event.current.button == 0)
					{
						GraphPosition += new Vector3(Event.current.delta.x, Event.current.delta.y);
						Repaint();
					}
					if (Event.current != null && Event.current.rawType == EventType.ScrollWheel)
					{
						GraphPosition += new Vector3(Event.current.delta.x * -6f, Event.current.delta.y * -6f);
						Repaint();
					}
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

		/// <summary>
		/// Draws the splash.
		/// </summary>
		void DrawSplash ()
		{
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
				{
					GUILayout.Label("Welcome to Noise Maker!");
					GUILayout.BeginHorizontal();
					{
						// Create new Noise Maker graph file.
						if (GUILayout.Button("New")) 
						{
							var savePath = UnityEditor.EditorUtility.SaveFilePanelInProject("New Noise Graph", "Noise", "asset", null);
							if (!StringExtensions.IsNullOrWhiteSpace(savePath))
							{
								// Create default save, add required root node, save it, and open it up for editing.
								SavePath = savePath;
								var config = NoiseGraph.CreateInstance<NoiseGraph>();
								AssetDatabase.CreateAsset(config, SavePath);
								Graph = new Graph();
								var root = new RootNode();
								root.Id = Guid.NewGuid().ToString();
								root.EditorPosition = GraphCenter();
								Graph.Nodes.Add(root);
								Graph.RootId = root.Id;
								State = States.Idle;
							}
						}
						// Open existing Noise Maker graph file
						if (GUILayout.Button("Open"))
						{
							var openPath = UnityEditor.EditorUtility.OpenFilePanel("Open Noise Graph", Application.dataPath, "asset");
							if (!StringExtensions.IsNullOrWhiteSpace(openPath))
							{
								if (openPath.StartsWith(Application.dataPath))
								{
									// Open up existing file for editing.
									SavePath = "Assets"+openPath.Substring(Application.dataPath.Length);
									var config = AssetDatabase.LoadAssetAtPath<NoiseGraph>(SavePath);
									Graph = config.GraphInstantiation;
									State = States.Idle;
								}
								else UnityEditor.EditorUtility.DisplayDialog("Invalid", "Selected noise graph must be inside project directory.", "Okay");
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

		/// <summary>
		/// Draws the graph containing all the nodes and connections for the file currently being edited.
		/// </summary>
		void DrawGraph ()
		{
			// Cache connecting from so we know if the user just clicked on a node, or deselected it by clicking the background of the graph.
			var connectingFromWas = ConnectingFrom;
			// Use this to determine the start point of the connection when the user's mouse is moving around with it.
			var fromRect = new Rect();
			// Index to assign the ConnectingFrom node to.
			var targetIn = 0;

			// Shift the GUI over so the windows don't draw under the node option area to the right of the editor.
			var wasMatrix = GUI.matrix;
			GUI.matrix = Matrix4x4.TRS(new Vector3(3f - Layouts.NodeOptionsWidth, 0f), Quaternion.identity, Vector3.one);

			BeginWindows();
			{
				// Cache the out and in connection points because we want to draw lines between the connected ones after the node windows.
				var outDict = new Dictionary<string, Rect>();
				var inDict = new Dictionary<string, List<Rect>>();

				Node deletedNode = null;

				foreach (var node in Graph.Nodes)
				{
					var graphPos = new Vector2(GraphPosition.x, GraphPosition.y);
					var unmodifiedNode = node;
					var drawer = NodeEditorCacher.Editors[unmodifiedNode.GetType()];
					var windowRect = new Rect(unmodifiedNode.EditorPosition + graphPos, new Vector2(216f, 77f));
					// Checks if this node has inputs we need to draw.
					if (unmodifiedNode.SourceIds != null && 0 < unmodifiedNode.SourceIds.Count)
					{
						var inputs = new List<NodeIo>();

						for (var i = 0; i < unmodifiedNode.SourceIds.Count; i++)
						{
							var unmodifiedI = i;

							inputs.Add(new NodeIo 
							{
								Active = !StringExtensions.IsNullOrWhiteSpace(unmodifiedNode.SourceIds[unmodifiedI]),
								OnClick = () => 
								{
									// When a node's input is clicked, cache what was clicked and the index of the input, 
									// unless it already has a connection, then delete that connection.
									if (StringExtensions.IsNullOrWhiteSpace(unmodifiedNode.SourceIds[unmodifiedI]))
									{
										ConnectingTo = unmodifiedNode;
										targetIn = unmodifiedI;
									}
									else 
									{
										unmodifiedNode.SourceIds[unmodifiedI] = null;
										ResetConnections();
									}
								}
							});
						}
						// DrawInputs does what it sounds like, then returns a list of their positions.
						inDict.Add(unmodifiedNode.Id, drawer.Editor.DrawInputs(windowRect, inputs.ToArray()));
					}
					// A single rect is returned when drawing an output: its position.
					var outRect = drawer.Editor.DrawOutput(
						windowRect,
						new NodeIo 
						{
							Connecting = ConnectingFrom == unmodifiedNode,
							OnClick = () => ConnectingFrom = unmodifiedNode
						}
					);
					outDict.Add(unmodifiedNode.Id, outRect);
					if (ConnectingFrom == unmodifiedNode) fromRect = outRect;
					// Can't delete the root node, so don't show the delete button.
					if (!(unmodifiedNode is RootNode) && drawer.Editor.DrawCloseControl(windowRect)) deletedNode = unmodifiedNode;

					GUI.color = unmodifiedNode is RootNode ? Color.cyan : Color.white;
					// Draw the node and cache its position incase it got dragged around.
					windowRect = GUILayout.Window(unmodifiedNode.Id.GetHashCode(), windowRect, id =>
					{
						try
						{
							drawer.Editor.Draw(Graph, unmodifiedNode);
						}
						catch (Exception e)
						{
							EditorGUILayout.HelpBox("Exception occured: \n"+e.Message, MessageType.Error);
							if (GUILayout.Button("Print Exception")) Debug.LogException(e);
						}
						GUI.DragWindow();
					}, (drawer == null || StringExtensions.IsNullOrWhiteSpace(drawer.Details.Name)) ? "Node" : drawer.Details.Name);

					GUI.color = Color.white;

					unmodifiedNode.EditorPosition = windowRect.position - graphPos;
				}
				// Loop through the nodes and draw any connections between them.
				foreach (var node in Graph.Nodes)
				{
					if (node.SourceIds == null || node.SourceIds.Count == 0) continue;

					var unmodifiedNode = node;
					var inRects = inDict[unmodifiedNode.Id];

					for (var i = 0; i < unmodifiedNode.SourceIds.Count; i++)
					{
						var source = unmodifiedNode.SourceIds[i];
						if (StringExtensions.IsNullOrWhiteSpace(source)) continue;

						var outRect = outDict[source];
						DrawCurve(outRect, inRects[i]);
					}
				}
				// Delete the node now, since we know it won't cause any null reference exceptions.
				if (deletedNode != null) Graph.Remove(deletedNode);
			}
	        EndWindows();
	        // If the user clicks the background of the graph area, it cancels the process of connecting nodes.
			if (connectingFromWas != null && Event.current.rawType == EventType.mouseUp && ConnectingTo == null) ResetConnections();
			// Draw the node that's following the cursor around if the user is currently connecting nodes.
			if (ConnectingFrom != null)
			{
				if (ConnectingTo == null)
				{
					// dragging around
					var cursorRect = new Rect(Event.current.mousePosition, Vector2.one);
					DrawCurve(fromRect, cursorRect);
					Repaint();
				}
				else
				{
					// just connected
					ConnectingTo.SourceIds[targetIn] = ConnectingFrom.Id;
					ResetConnections();
				}
			}
			// Shift the gui back to where it was.
			GUI.matrix = wasMatrix;
		}

		/// <summary>
		/// Draws the node spawning area to the right of the editor.
		/// </summary>
		void DrawNodeOptions()
		{
			var optionCategories = new Dictionary<string, List<NodeEditorEntry>>();
			// Go through and add all nodes to the appropriate category. 
			foreach (var option in NodeEditorCacher.Editors)
			{
				if (optionCategories.ContainsKey(option.Value.Details.Category)) optionCategories[option.Value.Details.Category].Add(option.Value);
				else optionCategories.Add(option.Value.Details.Category, new List<NodeEditorEntry>(new [] {option.Value}));
			}

			// todo: remove hacks below for layout when I get around to making a proper GUISkin for the editor.
			// hacks start
			var area = new Rect(position.width - (Layouts.NodeOptionsWidth - 3f), -4f, 8f, position.height - (Layouts.PreviewHeight - 11f));
			GUILayout.BeginArea(area);
			{
				GUILayout.Box(GUIContent.none, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
			}
			GUILayout.EndArea();
			area.x += 8f;
			area.y += 1f;
			area.height -= 2f;
			area.width = 232f;
			// hacks end

			GUILayout.BeginArea(area, Styles.OptionBox);
			{
				NodeOptionsScrollPosition = GUILayout.BeginScrollView(new Vector2(0f, NodeOptionsScrollPosition.y));
				{
					foreach(var category in optionCategories) 
					{
						// Special nodes like the root are not spawnable.
						if (category.Key == Strings.Hidden) continue;

						if (!ShownCategories.ContainsKey(category.Key)) ShownCategories.Add(category.Key, true);
						var shown = ShownCategories[category.Key];
						shown = EditorGUILayout.Foldout(shown, category.Key, Styles.Foldout);
						ShownCategories[category.Key] = shown;
						if (shown)
						{
							GUILayout.BeginHorizontal();
							{
								// Add a nice little margin
								GUILayout.Box(GUIContent.none, EditorStyles.miniButtonLeft, GUILayout.Width(16f), GUILayout.ExpandHeight(true));
								// Start drawing the actual options
								GUILayout.BeginVertical();
								{
									foreach (var option in category.Value)
									{
										if (GUILayout.Button(option.Details.Name, Styles.OptionButton)) 
										{
											var node = Activator.CreateInstance(option.Details.Target) as Node;
											node.Id = Guid.NewGuid().ToString();
											node.EditorPosition = GraphCenter();
											Graph.Nodes.Add(node);
										}
									}
								}
								GUILayout.EndVertical();
							}
							GUILayout.EndHorizontal();
						}
					}
					GUILayout.FlexibleSpace();
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndArea();
		}

		/// <summary>
		/// Draws the visualization area on the top left side of the editor.
		/// </summary>
		void DrawVisualizationOptions()
		{
			// Little lambda for drawing the button in the appropriate place.
			var drawToggler = new Action<float, float>((x, y) =>
			{
				var toggled = GUI.Button(new Rect(x, y, Layouts.VisualizationOptionsWidth, Layouts.VisualizationOptionsHeight), VisualizationShown ? "Close Visualizations" : "Open Visualizations", Styles.VisualizationToggle);
				if (toggled) 
				{
					VisualizationShown = !VisualizationShown;
					Repaint();
				}
			});

			if (!VisualizationShown) drawToggler(0f, 0f);
			else
			{
				var area = new Rect(0f, 0f, Layouts.VisualizationOptionsWidth, Layouts.VisualizationHeight);

				GUILayout.BeginArea(area, Styles.OptionBox);
				{
					var optionNames = new List<string>();
					var options = NodeEditor.Visualizations;
					foreach (var option in options) optionNames.Add(option.Name);
					Visualization = Mathf.Min(Visualization, options.Count);

					var oldVisualization = Visualization;
					Visualization = GUILayout.Toolbar(Visualization, optionNames.ToArray());
					if (oldVisualization != Visualization || NodeEditor.Previewer == null) 
					{	
						// User has selected a new visualization option.
						NodeEditor.Previewer = options[Visualization];
						Repaint();
					}
					GUILayout.BeginHorizontal();
					{
						GUILayout.FlexibleSpace();
						GUILayout.Box(options[Visualization].Preview);
						GUILayout.FlexibleSpace();
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					{
						GUILayout.Label(NodeEditor.Previewer.LowestCutoff.ToString("N2"), Styles.VisualizationRangeLabel);
						GUILayout.FlexibleSpace();
						GUILayout.Label(NodeEditor.Previewer.HighestCutoff.ToString("N2"), Styles.VisualizationRangeLabel);
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndArea();
				drawToggler(0f, Layouts.VisualizationHeight);
			}
		}

		/// <summary>
		/// Draws the preview area on the lower right side of the editor.
		/// </summary>
		void DrawPreview()
		{
			var area = new Rect(position.width - Layouts.PreviewWidth, position.height - Layouts.PreviewHeight, Layouts.PreviewWidth, Layouts.PreviewHeight);

			GUILayout.BeginArea(area);
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Box(GUIContent.none, EditorStyles.miniButtonLeft, GUILayout.Width(16f), GUILayout.Height(area.height));

					GUILayout.BeginVertical(Styles.PreviewBackground);
					{
						if (Previews == null) 
						{
							Previews = new Dictionary<string, Action<Node, Rect>> {
								{ "Flat", DrawFlatPreview },
								{ "Sphere", DrawSpherePreview },
								{ "Elevation", DrawElevationPreview }
							};
						}

						var keys = Previews.Keys.ToArray();
						GUILayout.BeginHorizontal();
						{
							for (var i = 0; i < keys.Length; i++)
							{
								if (GUILayout.Button(keys[i], i == PreviewSelected ? Styles.PreviewToolbarSelected : Styles.PreviewToolbar) && PreviewSelected != i) 
								{
									// When previews change, we reset any cached info.
									PreviewSelected = i;
									PreviewLastUpdated = 0L;
									PreviewObjectEditor = null;
									PreviewMesh = null;
								}
							}
						}
						GUILayout.EndHorizontal();
						// todo: remove the magic numbers below
						var previewArea = new Rect(14f, 24f, area.width - 14f, area.height - 24f);

						GUILayout.BeginArea(previewArea);
						{
							var rootNode = Graph == null ? null : Graph.Nodes.FirstOrDefault(n => n.Id == Graph.RootId);
							if (rootNode == null || rootNode.SourceIds == null || StringExtensions.IsNullOrWhiteSpace(rootNode.SourceIds.FirstOrDefault()) || rootNode.GetModule(Graph.Nodes) == null)
							{
								// A proper root with a source hasn't been defined.
								GUILayout.FlexibleSpace();
								GUILayout.BeginHorizontal();
								{
									GUILayout.FlexibleSpace();
									GUILayout.Label("Invalid Root", Styles.NoPreviewLabel);
									GUILayout.FlexibleSpace();
								}
								GUILayout.EndHorizontal();
								GUILayout.FlexibleSpace();
							}
							else Previews[keys[PreviewSelected]](rootNode, new Rect(0f, 0f, previewArea.width, previewArea.height));
						}
						GUILayout.EndArea();
						GUILayout.FlexibleSpace();
					}
					GUILayout.EndVertical();
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
		void DrawFlatPreview(Node node, Rect area)
		{
			var lastUpdate = NodeEditor.LastUpdated(node.Id);
			// todo: remove these duplicate update checks.
			// Check if node has been updated since the last time we had drawn it.
			if (lastUpdate != PreviewLastUpdated) 
			{
				if (PreviewTexture == null) PreviewTexture = new Texture2D((int)area.width, (int)area.height);

				var module = node.GetModule(Graph.Nodes);
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
		void DrawSpherePreview(Node node, Rect area)
		{
			// Reset mesh, incase another preview has modified it.
			NoiseMakerConfig.Instance.Ico4Vertex.GetComponent<MeshFilter>().sharedMesh = NoiseMakerConfig.Instance.Ico4VertexMesh;

			var lastUpdate = NodeEditor.LastUpdated(node.Id);
			// todo: remove these duplicate update checks.
			// Check if node has been updated since the last time we had drawn it.
			if (lastUpdate != PreviewLastUpdated) 
			{
				PreviewTexture = GetSphereTexture(node.GetModule(Graph.Nodes));

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
			PreviewObjectEditor.OnPreviewGUI(new Rect(1f, 0f, area.width - 1f, area.height), Styles.OptionBox);
		}

		/// <summary>
		/// Draws the elevation preview.
		/// </summary>
		/// <param name="node">Node to draw, typically the root.</param>
		/// <param name="area">Area the editor should take up.</param>
		void DrawElevationPreview(Node node, Rect area)
		{
			var lastUpdate = NodeEditor.LastUpdated(node.Id);
			// todo: remove these duplicate update checks.
			// Check if node has been updated since the last time we had drawn it.
			if (lastUpdate != PreviewLastUpdated) 
			{
				// Reset mesh
				NoiseMakerConfig.Instance.Ico5Vertex.GetComponent<MeshFilter>().sharedMesh = NoiseMakerConfig.Instance.Ico5VertexMesh;

				if (PreviewMesh == null) PreviewMesh = Instantiate(NoiseMakerConfig.Instance.Ico5VertexMesh);

				var module = node.GetModule(Graph.Nodes);
				var sphere = new Sphere(module);

				var verts = PreviewMesh.vertices;
				var newVerts = new Vector3[verts.Length];
				for (var i = 0; i < verts.Length; i++)
				{
					// Get the value of the specified vert, by converting it's euler position to a latitude and longitude.
					var vert = verts[i];
					var latLong = SphereUtils.CartesianToPolar(vert.normalized);
					newVerts[i] = (vert.normalized * SphereScalar) + (vert.normalized * (float)sphere.GetValue(latLong.x, latLong.y) * 0.1f);
				}
				PreviewMesh.vertices = newVerts;

				PreviewTexture = GetSphereTexture(module);

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
			mat.mainTextureOffset = new Vector2(0.5f, 0f);

			if (mat.mainTexture != PreviewTexture)
			{
				mat.mainTexture = PreviewTexture;
				Repaint();
			}

			if (PreviewObjectEditor == null) PreviewObjectEditor = Editor.CreateEditor(NoiseMakerConfig.Instance.Ico5Vertex);
			// Draw interactable preview.
			PreviewObjectEditor.OnPreviewGUI(new Rect(1f, 0f, area.width - 1f, area.height), Styles.OptionBox);
		}
		#endregion

		/// <summary>
		/// Gets the sphere projected texture, a simple heightmap using the active previewer.
		/// </summary>
		/// <returns>The sphere texture.</returns>
		/// <param name="module">Module to get the texture from.</param>
		/// <param name="height">Height.</param>
		public static Texture2D GetSphereTexture(IModule module, int width = 98, Mercator map = null, Texture2D existing = null)
		{
			var result = existing == null || existing.width != width || existing.height != width * 2 ? new Texture2D(width, width * 2) : existing;

			var lolTime = DateTime.Now;
			 
			var sphere = new Sphere(module);
			var pixels = new Color[result.width * result.height];
			for (var x = 0; x < result.width; x++)
			{
				for (var y = 0; y < result.height; y++)
				{
					var lat = SphereUtils.GetLatitude(y, result.height);
					var lon = SphereUtils.GetLongitude(x, result.width);
					var value = (float)sphere.GetValue((double)lat, (double)lon);
					pixels[(result.width * y) + x] = map == null ? NodeEditor.Previewer.Calculate(value, NodeEditor.Previewer) : map.GetColor(lat, lon, value);
				}
			}

			Debug.Log("Delta: "+(DateTime.Now - lolTime).Duration().TotalSeconds);
			TextureFarmer.Queue(result, pixels);

			return result;
		}

		/// <summary>
		/// Reset editor to splash.
		/// </summary>
		void Reset()
		{
			State = States.Splash;
			SavePath = null;
			Graph = null;
			GraphPosition = Vector3.zero;
			PreviewLastUpdated = 0L;
			ResetConnections();
		}

		/// <summary>
		/// Resets the active connections.
		/// </summary>
		void ResetConnections()
		{
			ConnectingFrom = null;
			ConnectingTo = null;
			Repaint();
		}

		/// <summary>
		/// Save the current file being edited.
		/// </summary>
		void Save()
		{
			if (State != States.Idle || Graph == null) return;
			if (StringExtensions.IsNullOrWhiteSpace(SavePath)) throw new NullReferenceException("SavePath cannot be null");
			var config = AssetDatabase.LoadAssetAtPath<NoiseGraph>(SavePath);
			config.GraphInstantiation = Graph;
			UnityEditor.EditorUtility.SetDirty(config);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Draws a bezier curve with a shadow.
		/// </summary>
		/// <remarks>
		/// This snippet taken from a Unity Answers question.
		/// </remarks>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		void DrawCurve(Rect start, Rect end)
		{
			Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2f, 0f);      
	        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2f, 0f);    
	        Vector3 startTan = startPos + Vector3.right * 50f;      
	        Vector3 endTan = endPos + Vector3.left * 50f;       
	        Color shadowCol = new Color(0f, 0f, 0f, 0.06f);       
			// Draw a shadow 
	        for (int i = 0; i < 3; i++) Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);      
	        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);      
    	}  

    	/// <summary>
    	/// Gets the current center of the graph, from the users perspective.
    	/// </summary>
    	/// <returns>The center.</returns>
    	Vector2 GraphCenter()
    	{
			return new Vector2(-(GraphPosition.x - (position.width * 0.3f)), -(GraphPosition.y - (position.height * 0.3f)));
    	}
	}
}