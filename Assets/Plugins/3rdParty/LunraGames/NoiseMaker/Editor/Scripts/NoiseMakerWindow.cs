using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Models;

namespace LunraGames.NoiseMaker
{
	public class NoiseMakerWindow : EditorWindow
	{
		public static class Layouts
		{
			public const float VisualizationOptionsWidth = 300f;
			public const float VisualizationOptionsHeight = 24f;
			public const float VisualizationHeight = VisualizationOptionsHeight * 4f;
			public const float NodeOptionsWidth = 240f;
			public const float PreviewWidth = NodeOptionsWidth + 13f;
			public const float PreviewHeight = PreviewWidth;
			public const float EditableOptionWidth = 32f;
		}

		enum States
		{
			Splash,
			Idle
		}

		static NoiseMakerWindow Instance;

		NoiseMakerWindow()
		{
			Instance = this;
		}

		static bool RepaintQueued;

		[SerializeField]
		States State = States.Splash;
		[SerializeField]
		int Visualization;
		[SerializeField]
		bool VisualizationShown;
		[SerializeField]
		Vector2 NodeOptionsScrollPosition = Vector2.zero;
		[SerializeField]
		string SaveGuid;
		[SerializeField]
		Vector3 GraphPosition = Vector3.zero;

		string SavePath { get { return StringExtensions.IsNullOrWhiteSpace(SaveGuid) ? null : AssetDatabase.GUIDToAssetPath(SaveGuid); } }

		Graph Graph;
		List<Property> Properties;
		INode ConnectingFrom;
		INode ConnectingTo;
		Dictionary<string, bool> ShownCategories = new Dictionary<string, bool>();
		int PreviewSelected;
		Dictionary<string, Action<Node<IModule>, Rect, int>> Previews;
		long PreviewLastUpdated;
		Texture2D PreviewTexture;
		Mesh PreviewMesh;
		Editor PreviewObjectEditor;
		bool PreviewUpdating;

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
				
				// If we're opening the editor from a cold start, and it looks like the user was editing something, load it.
				if (State == States.Idle && Graph == null && !StringExtensions.IsNullOrWhiteSpace(SaveGuid))
				{
					var config = AssetDatabase.LoadAssetAtPath<NoiseGraph>(SavePath);
					if (config == null) State = States.Splash;
					else 
					{
						Graph = config.GraphInstantiation;
						Properties = config.PropertiesInstantiation;

						Graph.Apply(Properties.ToArray());
					}
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

		void Update() 
		{
			if (RepaintQueued)
			{
				Repaint();
				RepaintQueued = false;
			}
		}
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
								var config = ScriptableObject.CreateInstance<NoiseGraph>();
								AssetDatabase.CreateAsset(config, savePath);
								SaveGuid = AssetDatabase.AssetPathToGUID(savePath);
								Graph = new Graph();
								Properties = new List<Property>();
								var root = new RootNode();
								root.Id = Guid.NewGuid().ToString();
								root.EditorPosition = GraphCenter();
								Graph.Nodes.Add(root);
								Graph.RootId = root.Id;
								State = States.Idle;
								Save();
							}
						}
						// Open existing Noise Maker graph file
						if (GUILayout.Button("Open"))
						{
							var openPath = UnityEditor.EditorUtility.OpenFilePanel("Open Noise Graph", Application.dataPath, "asset");
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

				INode deletedNode = null;

				foreach (var node in Graph.Nodes)
				{
					var graphPos = new Vector2(GraphPosition.x, GraphPosition.y);
					var unmodifiedNode = node;
					var drawer = NodeEditorCacher.Editors[unmodifiedNode.GetType()];
					var windowRect = new Rect(unmodifiedNode.EditorPosition + graphPos, new Vector2(216f, 77f));
					// Checks if this node has inputs we need to draw.
					if (unmodifiedNode.SourceIds != null && 0 < unmodifiedNode.SourceIds.Count && drawer.Linkers.Count == unmodifiedNode.SourceIds.Count)
					{
						var inputs = new List<NodeIo>();

						for (var i = 0; i < unmodifiedNode.SourceIds.Count; i++)
						{
							var unmodifiedI = i;
							var linker = drawer.Linkers[unmodifiedI];

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
								},
								MatchedType = ConnectingFrom != null && ConnectingFrom.OutputType == linker.Type,
								Name = linker.Name,
								Type = linker.Type
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
					// Property nodes can be renamed.
					var unmodifiedProperty = unmodifiedNode is IPropertyNode ? unmodifiedNode as IPropertyNode : null;
					if (unmodifiedProperty != null && unmodifiedProperty.IsEditable && drawer.Editor.DrawRenameControl(windowRect))
					{
						var property = Properties.FirstOrDefault(p => p.Id == unmodifiedNode.Id);
						var propertyName = property == null || StringExtensions.IsNullOrWhiteSpace(property.Name) ? string.Empty : property.Name;
						TextDialogPopup.Show(
							"Rename Property", 
							freshName =>
							{
								if (freshName == propertyName) return;
								else if (StringExtensions.IsNullOrWhiteSpace(freshName))
								{
									UnityEditor.EditorUtility.DisplayDialog("Invalid Name", "A property's name can't be empty.", "Okay");
									return;
								}
								foreach (var prop in Properties)
								{
									if (prop.Name == freshName && prop != property)
									{
										UnityEditor.EditorUtility.DisplayDialog("Duplicate Name", "A property with that name already exists.", "Okay");
										return;
									}
								}

								if (property == null) Properties.Add(new Property { Name = freshName, Id = unmodifiedNode.Id, Value = (unmodifiedNode as IPropertyNode).RawPropertyValue });
								else property.Name = freshName;
							},
							text: propertyName
						);
					} 

					var nodeName = (drawer == null || StringExtensions.IsNullOrWhiteSpace(drawer.Details.Name)) ? "Node" : drawer.Details.Name;

					if (unmodifiedNode is IPropertyNode)
					{
						var propertyNode = unmodifiedNode as IPropertyNode;
						var foundProperty = Properties.FirstOrDefault(p => p.Id == unmodifiedNode.Id);
						nodeName = propertyNode.IsEditable && foundProperty != null && !StringExtensions.IsNullOrWhiteSpace(foundProperty.Name) ? foundProperty.Name : nodeName;
						GUI.color = propertyNode.IsEditable ? Color.green : Color.white;
					}
					else GUI.color = unmodifiedNode is RootNode ? Styles.RootColor : Color.white;
					// Draw the node and cache its position incase it got dragged around.
					windowRect = GUILayout.Window(unmodifiedNode.Id.GetHashCode(), windowRect, id =>
					{
						GUI.DragWindow(new Rect(0f, 0f, windowRect.width, 20f));
						try
						{
							var result = drawer.Editor.Draw(Graph, unmodifiedNode);
							if (result is IPropertyNode)
							{
								var propertyNode = result as IPropertyNode;
								if (propertyNode.IsEditable)
								{
									var property = Properties.FirstOrDefault(p => p.Id == propertyNode.Id);
									if (property == null)
									{
										property = new Property { Name = "", Id = propertyNode.Id, Value = propertyNode.RawPropertyValue };
										Properties.Add(property);
									}
									else property.Value = propertyNode.RawPropertyValue;
								}
							}
						}
						catch (Exception e)
						{
							// if we errored inside a guilayout begin / end, we want to try and print our helpbox inside a catch statement.
							try 
							{ 
								EditorGUILayout.HelpBox("Exception occured: \n"+e.Message, MessageType.Error); 
								if (GUILayout.Button("Print Exception")) Debug.LogException(e);
							}
							catch {}
						}
					}, nodeName);

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
				if (deletedNode != null) 
				{
					Graph.Remove(deletedNode);
					var deletedProperty = Properties.FirstOrDefault(p => p.Id == deletedNode.Id);
					if (deletedProperty != null) Properties.Remove(deletedProperty);
				}
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
										var unmodifiedOption = option;

										if (unmodifiedOption.IsEditable) GUILayout.BeginHorizontal();

										if (GUILayout.Button(unmodifiedOption.Details.Name, unmodifiedOption.IsEditable ? Styles.OptionButtonMiddle : Styles.OptionButtonRight)) 
										{
											var node = Activator.CreateInstance(unmodifiedOption.Details.Target) as INode;
											node.Id = Guid.NewGuid().ToString();
											node.EditorPosition = GraphCenter();
											Graph.Nodes.Add(node);
										}

										if (unmodifiedOption.IsEditable) 
										{
											if (GUILayout.Button(NoiseMakerConfig.Instance.EditableOption, Styles.OptionButtonRight, GUILayout.Width(Layouts.EditableOptionWidth), GUILayout.ExpandHeight(true))) 
											{
												TextDialogPopup.Show(
													"Create New Property", 
													propertyName => 
													{
 														if (StringExtensions.IsNullOrWhiteSpace(propertyName)) UnityEditor.EditorUtility.DisplayDialog("Invalid", "An empty name is not valid for a property node.", "Okay");
														else if (Properties.FirstOrDefault(p => p.Name == propertyName) != null) UnityEditor.EditorUtility.DisplayDialog("Property Exists", "A property named \""+propertyName+"\" already exists.", "Okay");
														else
														{

															var node = Activator.CreateInstance(unmodifiedOption.Details.Target) as IPropertyNode;
															node.Id = Guid.NewGuid().ToString();
															node.EditorPosition = GraphCenter();
															node.IsEditable = true;
															Graph.Nodes.Add(node);

															Properties.Add(new Property { Name = propertyName, Id = node.Id, Value = node.RawPropertyValue });
														}
													},
													description: "Enter a unique name for this property node."
												);
											}
											GUILayout.EndHorizontal();
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
							Previews = new Dictionary<string, Action<Node<IModule>, Rect, int>> {
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
						// todo: remove the magic numbers below
						var previewArea = new Rect(14f, 24f, area.width - 14f, area.height - 24f);

						GUILayout.BeginArea(previewArea);
						{
							var rootNode = Graph == null ? null : Graph.Nodes.FirstOrDefault(n => n.Id == Graph.RootId);

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
								Previews[keys[PreviewSelected]](rootNode as Node<IModule>, new Rect(0f, 0f, previewArea.width, previewArea.height), PreviewSelected);

								if (PreviewUpdating)
								{
									Pinwheel.Draw(new Vector2(previewArea.width * 0.5f, previewArea.height * 0.5f));
									Repaint();
								}
							}
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
						pixels[SphereUtils.PixelCoordinateToIndex(x, y, PreviewTexture.width, PreviewTexture.height)] = NodeEditor.Previewer.Calculate(value, NodeEditor.Previewer);
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
				PreviewTexture = GetSphereTexture(node.GetValue(Graph), completed: () => PreviewUpdating = (PreviewLastUpdated == lastUpdate && PreviewSelected == index) ? false : PreviewUpdating);

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
				PreviewTexture = GetSphereTexture(module, completed: () => PreviewUpdating = (PreviewLastUpdated == lastUpdate && PreviewSelected == index) ? false : PreviewUpdating);

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
			PreviewObjectEditor.OnPreviewGUI(new Rect(1f, 0f, area.width - 1f, area.height), Styles.OptionBox);
		}
		#endregion

		/// <summary>
		/// Gets the sphere projected texture, a simple heightmap using the active previewer.
		/// </summary>
		/// <returns>The sphere texture.</returns>
		/// <param name="module">Module to get the texture from.</param>
		/// <param name="width">The width of the texture, the height will be half of it.</param>
		/// <param name="map">Mercator map to use for texturing, uses current previewer if one is specified.</param>
		/// <param name="existing">An existing texture to write to, for performance reasons. Creates a new one if the size isn't right.</param>
		/// <param name="completed">Completed callback</param>
		public static Texture2D GetSphereTexture(IModule module, int width = 98, Mercator map = null, Texture2D existing = null, Action completed = null)
		{
			var result = existing == null || existing.width != width || existing.height != width * 2 ? new Texture2D(width, width * 2) : existing;

			var sphere = new Sphere(module);
			var resultWidth = result.width;
			var resultHeight = result.height;
			var unmodifiedMap = map;
			var pixels = new Color[resultWidth * resultHeight];

			Thrifty.Queue(
				() =>
				{
					if (unmodifiedMap == null)
					{
						for (var x = 0; x < resultWidth; x++)
						{
							for (var y = 0; y < resultHeight; y++)
							{
								var lat = SphereUtils.GetLatitude(y, resultHeight);
								var lon = SphereUtils.GetLongitude(x, resultWidth);
								var value = (float)sphere.GetValue((double)lat, (double)lon);
								pixels[SphereUtils.PixelCoordinateToIndex(x, y, resultWidth, resultHeight)] = NodeEditor.Previewer.Calculate(value, NodeEditor.Previewer);
							}
						}
					}
					else unmodifiedMap.GetSphereColors(resultWidth, resultHeight, sphere, ref pixels);
				},
				() => TextureFarmer.Queue(result, pixels, completed)
			);
			return result;
		}

		/// <summary>
		/// Reset editor to splash.
		/// </summary>
		void Reset()
		{
			State = States.Splash;
			SaveGuid = null;
			Graph = null;
			Properties = null;
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

			if (config == null) 
			{
				UnityEditor.EditorUtility.DisplayDialog("Missing Noise Graph", "The Noise Graph you were editing is now missing.", "Okay");
				Reset();
				return;
			}

			config.GraphInstantiation = Graph;
			config.PropertiesInstantiation = Properties;
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

		public static Type ConnectingFromOutputType { get { return Instance == null || Instance.ConnectingFrom == null ? null : Instance.ConnectingFrom.OutputType; } }
		public static string ActiveSavePath { get { return Instance == null || StringExtensions.IsNullOrWhiteSpace(Instance.SavePath) ? null : Instance.SavePath; } }

		public static void QueueRepaint()
		{
			RepaintQueued = true;
		}

		public static void OpenNoiseGraph(string path)
		{
			if (StringExtensions.IsNullOrWhiteSpace(path)) return;

			if (Instance == null) Init();

			Instance.Open(path);
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
					var result = UnityEditor.EditorUtility.DisplayDialogComplex("Editing in Progress", "You're in the middle of editing another Noise Graph, what would you like to do?", "Save", "Cancel", "Discard Changes");

					if (result == 0) Save();
					else if (result == 1) return;

					Reset();
				}

				// Open up existing file for editing.
				SaveGuid = AssetDatabase.AssetPathToGUID(fromAssets ? path : "Assets"+path.Substring(Application.dataPath.Length));
				var config = AssetDatabase.LoadAssetAtPath<NoiseGraph>(SavePath);
				Graph = config.GraphInstantiation;
				Properties = config.PropertiesInstantiation;
				Graph.Apply(Properties.ToArray());
				State = States.Idle;

				Repaint();
			}
			else UnityEditor.EditorUtility.DisplayDialog("Invalid", "Selected noise graph must be inside project directory.", "Okay");	
		}
	}
}