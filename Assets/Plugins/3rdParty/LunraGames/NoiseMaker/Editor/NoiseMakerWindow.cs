﻿using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class NoiseMakerWindow : EditorWindow
	{
		const string GraphKey = "LG_NoiseMaker_Graph";

		public class Layouts
		{
			public const float PreviewWidth = 400f;
			public const float PreviewHeight = 400f;
			public const float VisualizationOptionsWidth = 300f;
			public const float VisualizationOptionsHeight = 24f;
			public const float VisualizationHeight = VisualizationOptionsHeight * 4f;
		}

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
		[SerializeField]
		GameObject PreviewObject;
		[SerializeField]
		Editor PreviewObjectEditor;

		Graph Graph;
		Node ConnectingFrom;
		Node ConnectingTo;
		Dictionary<string, bool> ShownCategories = new Dictionary<string, bool>();
		int PreviewSelected;
		Dictionary<string, Action> Previews;

		[MenuItem ("Window/Noise Maker")]
		static void Init () 
		{
			var window = EditorWindow.GetWindow(typeof (NoiseMakerWindow), false, "Noise Maker") as NoiseMakerWindow;
			window.Show();
		}

		#region Messages
		void OnGUI()
		{
			try 
			{
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
					if (GUI.Button(new Rect(Layouts.VisualizationOptionsWidth, 0f, 128f, 24f), "Reset", Styles.ToolbarButtonMiddle)) Reset();
					if (GUI.Button(new Rect(Layouts.VisualizationOptionsWidth + 128f, 0f, 128f, 24f), "Save", Styles.ToolbarButtonRight)) 
					{
						Save();
					}
					DrawNodeOptions();
					DrawPreview();

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
						if (GUILayout.Button("New")) 
						{
							var savePath = UnityEditor.EditorUtility.SaveFilePanelInProject("New Noise Graph", "Noise", "asset", null);
							if (!StringExtensions.IsNullOrWhiteSpace(savePath))
							{
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
						if (GUILayout.Button("Open"))
						{
							var openPath = UnityEditor.EditorUtility.OpenFilePanel("Open Noise Graph", Application.dataPath, "asset");
							if (!StringExtensions.IsNullOrWhiteSpace(openPath))
							{
								if (openPath.StartsWith(Application.dataPath))
								{
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

		void DrawGraph ()
		{
			var connectingFromWas = ConnectingFrom;
			var fromRect = new Rect();
			var targetIn = 0;

			BeginWindows();
			{
				var outDict = new Dictionary<string, Rect>();
				var inDict = new Dictionary<string, List<Rect>>();

				Node deletedNode = null;

				foreach (var node in Graph.Nodes)
				{
					var graphPos = new Vector2(GraphPosition.x, GraphPosition.y);
					var unmodifiedNode = node;
					var drawer = NodeEditorCacher.Editors[unmodifiedNode.GetType()];
					var windowRect = new Rect(unmodifiedNode.EditorPosition + graphPos, new Vector2(216f, 216f));

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
						inDict.Add(unmodifiedNode.Id, drawer.Editor.DrawInputs(windowRect, inputs.ToArray()));
					}

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

					if (!(unmodifiedNode is RootNode) && drawer.Editor.DrawCloseControl(windowRect)) deletedNode = unmodifiedNode;

					GUI.color = unmodifiedNode is RootNode ? Color.cyan : Color.white;

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

				if (deletedNode != null) Graph.Remove(deletedNode);
			}
	        EndWindows();

			if (connectingFromWas != null && Event.current.rawType == EventType.mouseUp && ConnectingTo == null) ResetConnections();

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
		}


		void DrawNodeOptions()
		{
			var optionCategories = new Dictionary<string, List<EditorEntry>>();

			foreach (var option in NodeEditorCacher.Editors)
			{
				if (optionCategories.ContainsKey(option.Value.Details.Category)) optionCategories[option.Value.Details.Category].Add(option.Value);
				else optionCategories.Add(option.Value.Details.Category, new List<EditorEntry>(new EditorEntry[] {option.Value}));
			}

			var area = new Rect(position.width - 240f, -1f, 8f, position.height - Layouts.PreviewHeight + 2f);
			GUILayout.BeginArea(area);
			{
				GUILayout.Box(GUIContent.none, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
			}
			GUILayout.EndArea();
			area.x += 8f;
			area.y += 1f;
			area.height -= 2f;
			area.width = 232f;
			GUILayout.BeginArea(area, Styles.OptionBox);
			{
				NodeOptionsScrollPosition = GUILayout.BeginScrollView(new Vector2(0f, NodeOptionsScrollPosition.y));
				{
					foreach(var category in optionCategories) 
					{
						if (category.Key == Strings.Hidden) continue;

						if (!ShownCategories.ContainsKey(category.Key)) ShownCategories.Add(category.Key, true);
						var shown = ShownCategories[category.Key];
						shown = EditorGUILayout.Foldout(shown, category.Key, Styles.Foldout);
						ShownCategories[category.Key] = shown;
						if (shown)
						{
							GUILayout.BeginHorizontal();
							{
								GUILayout.Box(GUIContent.none, EditorStyles.miniButtonLeft, GUILayout.Width(16f), GUILayout.ExpandHeight(true));
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

		void DrawVisualizationOptions()
		{
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

		void DrawPreview()
		{
			var area = new Rect(position.width - Layouts.PreviewWidth, position.height - Layouts.PreviewHeight, Layouts.PreviewWidth, Layouts.PreviewHeight);

			GUILayout.BeginArea(area, Styles.OptionBox);
			{
				if (Previews == null) 
				{
					Previews = new Dictionary<string, Action> {
						{ "Flat", DrawFlatPreview },
						{ "Sphere", DrawSpherePreview },
						{ "Elevation", DrawElevationPreview }
					};
				}

				var keys = Previews.Keys.ToArray();
				PreviewSelected = GUILayout.Toolbar(PreviewSelected, keys);

				var previewArea = new Rect(0f, 24f, area.width, area.height - 24f);
				GUILayout.BeginArea(previewArea);
				{
					var rootNode = Graph.Nodes.FirstOrDefault(n => n.Id == Graph.RootId);
					if (rootNode == null || rootNode.SourceIds == null || StringExtensions.IsNullOrWhiteSpace(rootNode.SourceIds[0]))
					{
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
					else Previews[keys[PreviewSelected]]();
				}
				GUILayout.EndArea();
			}
			GUILayout.EndArea();
		/*
			if (PreviewObject == null)
			{
				var targetPreview = NoiseMakerConfig.Instance == null ? null : NoiseMakerConfig.Instance.Ico4Preview;
				if (targetPreview == null)
				{
					Debug.LogError("No target preview available");
					return;
				}
				//PreviewObject = (GameObject)GameObject.Instantiate(targetPreview);
//				PreviewObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector | HideFlags.NotEditable;
				PreviewObject = targetPreview;
				PreviewObjectEditor = null;
			}
			if (PreviewObjectEditor == null)
			{
 				PreviewObjectEditor = Editor.CreateEditor(PreviewObject);
			}


			var area = new Rect(position.width - Layouts.PreviewWidth, position.height - Layouts.PreviewHeight, Layouts.PreviewWidth, Layouts.PreviewHeight);

			PreviewObjectEditor.OnPreviewGUI(area, GUI.skin.box);
			*/
			/*
			GUILayout.BeginArea(area);
			{
				
				//GUILayout.Box(GUIContent.none, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
			}
			GUILayout.EndArea();
			*/
		}

		#region Previews
		void DrawFlatPreview()
		{
			

			//GUI.DrawTexture(area, Graph.Root.)
		}
		void DrawSpherePreview()
		{

		}
		void DrawElevationPreview()
		{

		}
		#endregion

		void Reset()
		{
			State = States.Splash;
			SavePath = null;
			Graph = null;
			GraphPosition = Vector3.zero;
			ResetConnections();
		}

		void ResetConnections()
		{
			ConnectingFrom = null;
			ConnectingTo = null;
			Repaint();
		}

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

		void DrawCurve(Rect start, Rect end)
		{
			Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);      
	        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);    
	        Vector3 startTan = startPos + Vector3.right * 50;      
	        Vector3 endTan = endPos + Vector3.left * 50;       
	        Color shadowCol = new Color(0, 0, 0, 0.06f);       
			// Draw a shadow 
	        for (int i = 0; i < 3; i++) Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);      
	        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);      
    	}  

    	Vector2 GraphCenter()
    	{
			return new Vector2(-(GraphPosition.x - (position.width * 0.3f)), -(GraphPosition.y - (position.height * 0.3f)));
    	}
	}
}