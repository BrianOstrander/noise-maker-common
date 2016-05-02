using UnityEditor;
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

		class Layouts
		{
			public const float VisualizationOptionsWidth = 300f;
			public const float VisualizationOptionsHeight = 24f;
			public const float VisualizationHeight = 200f;
		}

		enum States
		{
			Splash,
			Idle,
			Connecting
		}

		[SerializeField]
		States State = States.Splash;
		[SerializeField]
		Graph _Graph;
		[SerializeField]
		GraphConfig Config;
		[SerializeField]
		int Visualization;

		Graph Graph 
		{ 
			get 
			{ 
				return Config == null ? _Graph : Config.Graph; 
			}

			set
			{
				if (Config == null) _Graph = value;
				else Config.Graph = value;
			}
		}

		Node ConnectingFrom;
		Node ConnectingTo;
		Dictionary<string, bool> ShownCategories = new Dictionary<string, bool>();
		float? VisualizationShown;

		NoiseMakerWindow()
		{
			_Graph = EditorPrefsExtensions.GetJson<Graph>(GraphKey, new Graph());
		}

		[MenuItem ("Window/Noise Maker")]
		static void Init () 
		{
			var window = EditorWindow.GetWindow(typeof (NoiseMakerWindow), false, "Noise Maker") as NoiseMakerWindow;
			window.Show();
		}

		void OnGUI()
		{
			try 
			{
				if (Graph == null) State = States.Splash;
				
				if (State == States.Splash) DrawSplash();
				else if (State == States.Idle || State == States.Connecting)
				{
					DrawGraph();
					DrawVisualizationOptions();
					if (GUI.Button(new Rect(Layouts.VisualizationOptionsWidth, 0f, 128f, 24f), "Reset", Styles.ResetButton)) Reset();
					DrawNodeOptions();

		        	if (State == States.Connecting)
		        	{
		        		// todo: detect when user stops trying to connect nodes
		        	}
		        	else Cache();
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
							Reset();
							Graph = new Graph();
							State = States.Idle;
						}
						if (GUILayout.Button("Open")) Debug.Log("not implimented");
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

				foreach (var node in Graph.Nodes)
				{
					var unmodifiedNode = node;
					var drawer = NodeEditorCacher.Editors[unmodifiedNode.GetType()];
					var windowRect = new Rect(unmodifiedNode.EditorPosition, new Vector2(216f, 216f));

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
					unmodifiedNode.EditorPosition = windowRect.position;
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

			var area = new Rect(position.width - 208f, -1f, 8f, position.height + 2f);
			GUILayout.BeginArea(area);
			{
				GUILayout.Box(GUIContent.none, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
			}
			GUILayout.EndArea();
			area.x += 8f;
			area.y += 1f;
			area.height -= 2f;
			area.width = 200f;
			GUILayout.BeginArea(area, Styles.OptionBox);
			{
				GUILayout.BeginScrollView(Vector2.zero);
				{
					foreach(var category in optionCategories) 
					{
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
											node.EditorPosition = new Vector2(position.width * 0.5f, position.height * 0.5f);
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
				var toggled = GUI.Button(new Rect(x, y, Layouts.VisualizationOptionsWidth, Layouts.VisualizationOptionsHeight), VisualizationShown.HasValue ? "Close Visualizations" : "Open Visualizations", Styles.VisualizationToggle);
				if (toggled) 
				{
					if (VisualizationShown.HasValue) VisualizationShown = null;
					else VisualizationShown = Layouts.VisualizationHeight;
					Repaint();
				}
			});

			if (!VisualizationShown.HasValue) drawToggler(0f, 0f);
			else if (Mathf.Approximately(VisualizationShown.Value, Layouts.VisualizationHeight))
			{
				var area = new Rect(0f, 0f, Layouts.VisualizationOptionsWidth, 24f + (VisualizationShown.HasValue ? VisualizationShown.Value : 0f));

				GUILayout.BeginArea(area, Styles.OptionBox);
				{
					var optionNames = new List<string>();
					var options = NodeEditor.Visualizations;
					foreach (var option in options) optionNames.Add(option.Name);
					Visualization = Mathf.Min(Visualization, options.Count);

					var oldVisualization = Visualization;
					Visualization = GUILayout.Toolbar(Visualization, optionNames.ToArray());
					if (oldVisualization != Visualization) 
					{	
						options[Visualization].Activate();
						Repaint();
					}
					GUILayout.BeginHorizontal();
					{
						//GUILayout.Label();
						GUILayout.Box(options[Visualization].Preview);
//						GUILayout.Label();
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndArea();
				drawToggler(0f, Layouts.VisualizationHeight);
			}
		}

		void Reset()
		{
			State = States.Splash;
			Config = null;
			Graph = null;
			ResetConnections();
		}

		void ResetConnections()
		{
			ConnectingFrom = null;
			ConnectingTo = null;
			Repaint();
		}

		void Cache()
		{
			EditorPrefsExtensions.SetJson(GraphKey, Graph);
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
	}
}