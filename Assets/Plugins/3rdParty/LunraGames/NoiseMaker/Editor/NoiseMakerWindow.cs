using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public class NoiseMakerWindow : EditorWindow
	{
		Dictionary<string, Func<Node>> NodeOptions = new Dictionary<string, Func<Node>> {
			{ "Perlin", () => new PerlinNode() }
		};

		//[SerializeField]
		List<Node> Nodes = new List<Node>();

		[MenuItem ("Window/Noise Maker")]
		static void Init () 
		{
			var window = EditorWindow.GetWindow(typeof (NoiseMakerWindow), false, "Noise Maker") as NoiseMakerWindow;
			window.Show();
		}

		void OnGUI()
		{
			DrawGraph();
	        DrawNodeOptions();
		}

		void DrawGraph()
		{
			BeginWindows();
			{
				foreach (var node in Nodes)
				{
					node.Draw();
				}
			}
	        EndWindows();
		}

		void DrawNodeOptions()
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
							node.Position.position = new Vector2(0f, 0f);
							Nodes.Insert(0, node);
						}
					}
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndArea();
		}
	}
}