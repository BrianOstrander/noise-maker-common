using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public class NoiseMakerWindow : EditorWindow
	{
		const string GraphKey = "LG_NoiseMaker_Graph";

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

		Dictionary<string, Func<Node>> NodeOptions = new Dictionary<string, Func<Node>> 
		{
			{ "Perlin", () => new PerlinNode() }
		};

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
					DrawInspector();
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

		void DrawInspector ()
		{
			var area = new Rect(0f, 0f, 300f, position.height);
			GUILayout.BeginArea(area);
			{
				// todo: rename this something meaningful
				if (GUILayout.Button("Reset")) Reset();
			}
			GUILayout.EndArea();
		}

		void DrawGraph ()
		{
			BeginWindows();
			{
				foreach (var node in Graph.Nodes)
				{
 					var drawer = NodeEditorCacher.Editors[node.GetType()];
 					var windowRect = new Rect(node.EditorPosition, new Vector2(128f, 200f));
					windowRect = GUILayout.Window(node.Id.GetHashCode(), windowRect, id =>
					{
						drawer.Editor.Draw(node);
						GUI.DragWindow();
					}, (drawer == null || StringExtensions.IsNullOrWhiteSpace(drawer.Details.Name)) ? "Node" : drawer.Details.Name);
					node.EditorPosition = windowRect.position;
				}
			}
	        EndWindows();
		}

		void DrawNodeOptions ()
		{
			var area = new Rect(position.width - 200f, 0f, 200f, position.height);
			GUILayout.BeginArea(area);
			{
				GUILayout.BeginScrollView(Vector2.zero);
				{
					foreach(var option in NodeOptions) 
					{
						if (GUILayout.Button(option.Key, GUILayout.Height(48f))) 
						{
							var node = option.Value();
							node.Id = Guid.NewGuid().ToString();
							node.EditorPosition = new Vector2(position.width * 0.5f, position.height * 0.5f);
							Graph.Nodes.Add(node);
						}
					}
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndArea();
		}

		void Reset ()
		{
			State = States.Splash;
			Config = null;
			Graph = null;
		}

		void Cache ()
		{
			EditorPrefsExtensions.SetJson(GraphKey, Graph);
		}
	}
}