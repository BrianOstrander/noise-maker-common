using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Atesh;
using LibNoise;
using UnityEditor;

namespace LunraGames.NoiseMaker
{
	public abstract class NodeEditor
	{
		public const int PreviewWidth = 198;
		public const int PreviewHeight = 64;

		const float IoStartOffset = 16f;
		const float IoDivider = 8f;
		const float IoWidth = 32f;
		const float IoHeight = 16f;

		const float CloseWidth = 18f;
		const float CloseHeight = 18f;
		const float CloseStartOffset = CloseWidth * 2f;

		public delegate Color CalculateColor(float value, VisualizationPreview previewer);

		public static VisualizationPreview Previewer = Visualizations[0];

		static List<VisualizationPreview> _Visualizations;

		public static List<VisualizationPreview> Visualizations
		{
			get
			{
				if (_Visualizations == null)
				{
					_Visualizations = new List<VisualizationPreview>();
					_Visualizations.Add(new VisualizationPreview 
					{
						Name = "Grayscale",
						Calculate = Visualizers.Grayscale,
						LowestCutoff = -2f,
						HighestCutoff = 2f,
						ValueMin = 0f,
						ValueMax = 1f
					});
					_Visualizations.Add(new VisualizationPreview 
					{
						Name = "Spectrum",
						Calculate = Visualizers.Spectrum,
						LowestCutoff = -2f,
						HighestCutoff = 2f,
						HueMin = 0f,
						HueMax = 1f
					});
					_Visualizations.Add(new VisualizationPreview 
					{
						Name = "Cool",
						Calculate = Visualizers.Spectrum,
						LowestCutoff = -2f,
						HighestCutoff = 2f,
						HueMin = 0.3f,
						HueMax = 0.7f
					});
				}

				return _Visualizations;
			}
		}

		protected static List<NodePreview> Previews = new List<NodePreview>();

		protected NodePreview GetPreview(Type type, Graph graph, INode node)
		{
			var preview = Previews.FirstOrDefault(p => p.Id == node.Id);

			if (preview != null)
			{
				preview.Stale = preview.Stale || node.SourceIds.Count != preview.LastSourceIds.Count || preview.LastVisualizer != Previewer;
				for (var i = 0; i < node.SourceIds.Count; i++)
				{
					var id = node.SourceIds[i];
					preview.Stale = preview.Stale || id != preview.LastSourceIds[i];
					if (StringExtensions.IsNullOrWhiteSpace(id)) continue;
					var sourcePreview = Previews.FirstOrDefault(p => p.Id == id);
					if (sourcePreview == null) continue;
					preview.Stale = preview.Stale || preview.LastUpdated < sourcePreview.LastUpdated;
				}
			}

			if (preview == null)
			{
				preview = new NodePreview { Id = node.Id, Stale = true };
				preview.Preview = new Texture2D(PreviewWidth, PreviewHeight);
				Previews.Add(preview);
			}

			if (preview.Stale)
			{
				if (type == typeof(IModule))
				{
					var module = node.GetRawValue(graph.Nodes) as IModule;
					var width = preview.Preview.width;
					var height = preview.Preview.height;
					var pixels = new Color[width * height];

					Thrifty.Queue(
						() =>
						{
							for (var x = 0; x < width; x++)
							{
								for (var y = 0; y < height; y++)
								{
									var index = (width * y) + x;
									if (module == null) pixels[index] = Color.clear;
									else 
									{
										var value = (float)module.GetValue((double)x, (double)y, 0.0);
										pixels[index] = Previewer.Calculate(value, Previewer);
									}
								}
							}
						},
						() => TextureFarmer.Queue(preview.Preview, pixels)
					);
				}

				preview.Stale = false;
				preview.LastUpdated = DateTime.Now.Ticks;
				preview.LastSourceIds = new List<string>(node.SourceIds);
				preview.LastVisualizer = Previewer;
			}

			return preview;
		}

		protected NodePreview GetPreview<T>(Graph graph, INode node)
		{
			return GetPreview(typeof(T), graph, node);
		}

		public List<Rect> DrawInputs(Rect position, params NodeIo[] inputs)
		{
			var startRect = new Rect(position.x - IoWidth + 1, position.y + IoStartOffset, IoWidth, IoHeight);
			var currRect = new Rect(startRect);
			var rects = new List<Rect>();
			foreach (var input in inputs)
			{
				var wasColor = GUI.color;
				GUI.color = input.MatchedType ? Color.cyan : Color.white;
				if (input.MatchedType)
				{
					var width = Styles.BoxButton.CalcSize(new GUIContent(input.Name)).x;
					currRect.x = Mathf.Min(currRect.x, (currRect.x + currRect.width) - width);
					currRect.width = Mathf.Max(currRect.width, width);
					if (GUI.Button(currRect, input.Name, Styles.BoxButton)) input.OnClick();
				}
				else if (GUI.Button(currRect, input.Active ? new GUIContent("x", input.Name) : new GUIContent(string.Empty, input.Name), Styles.BoxButton)) 
				{
					if (input.Active) input.OnClick();
					else UnityEditor.EditorUtility.DisplayDialog("Invalid", "An input of type \""+input.Type.Name+"\" is required for "+input.Name, "Okay");
				}
				GUI.color = wasColor;

				rects.Add(new Rect(currRect));

				currRect.x = startRect.x;
				currRect.width = startRect.width;
				currRect.y += IoDivider + IoHeight;
			}
			return rects;
		}

		public Rect DrawOutput(Rect position, NodeIo output)
		{
			if (this is RootNodeEditor) return new Rect();
			var currRect = new Rect(position.x + position.width - 2, position.y + IoStartOffset, IoWidth, IoHeight);
			if (GUI.Button(currRect, GUIContent.none, output.Connecting ? Styles.BoxButtonHovered : Styles.BoxButton)) output.OnClick();
			return currRect;
		}

		public INode DrawFields(Graph graph, INode node, bool showPreview = true)
		{
			var preview = GetPreview(node.OutputType, graph, node);

			if (showPreview) GUILayout.Box(preview.Preview, GUILayout.MaxWidth(PreviewWidth), GUILayout.ExpandWidth(true));

			var entry = NodeEditorCacher.Editors[node.GetType()];
			var links = entry.Linkers.OrderBy(l => l.Index).ToList();

			foreach (var link in links)
			{
				if (link.Hide) continue;

				GUI.enabled = StringExtensions.IsNullOrWhiteSpace(node.SourceIds[link.Index]);

				var linkValue = link.Field.GetValue(node);
				if (link.Type == typeof(float))
				{
					var typedValue = (float)linkValue;
					link.Field.SetValue(node, Deltas.DetectDelta<float>(typedValue, EditorGUILayout.FloatField(link.Name, typedValue), ref preview.Stale));
				}
				else if (link.Type == typeof(int))
				{
					var typedValue = (int)linkValue;
					link.Field.SetValue(node, Deltas.DetectDelta<int>(typedValue, EditorGUILayout.IntField(link.Name, typedValue), ref preview.Stale));
				}
				else if (link.Type == typeof(bool))
				{
					var typedValue = (bool)linkValue;
					link.Field.SetValue(node, Deltas.DetectDelta<bool>(typedValue, EditorGUILayout.Toggle(link.Name, typedValue), ref preview.Stale));
				}
				else
				{
					EditorGUILayout.HelpBox("Field of unrecognized type: "+link.Type.Name, MessageType.Error);
				}
			}

			return node;
		}

		public bool DrawCloseControl(Rect position)
		{
			var rect = new Rect(position.x + position.width - CloseStartOffset, position.y - CloseHeight, CloseWidth, CloseHeight);
			return GUI.Button(rect, "x", Styles.CloseButton);
		}

		public static long LastUpdated(string id)
		{
			var preview = Previews.FirstOrDefault(p => p.Id == id);
			return preview == null ? long.MinValue : preview.LastUpdated;
		}

		public abstract INode Draw(Graph graph, INode node);
	}
}