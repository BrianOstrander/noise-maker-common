using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	[InitializeOnLoad]
	sealed class NodeEditorCacher : Editor 
	{
		static Dictionary<Type, EditorEntry> _Editors = new Dictionary<Type, EditorEntry>();

		internal static Dictionary<Type, EditorEntry> Editors { get { return new Dictionary<Type, EditorEntry>(_Editors); }}

		static NodeEditorCacher()
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode) Refresh();
		}

		static void Refresh()
		{
			_Editors = new Dictionary<Type, EditorEntry>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types) 
				{
					var attributes = type.GetCustomAttributes(typeof(NodeDrawer), true);
					if (0 < attributes.Length && !_Editors.ContainsKey(type)) 
					{
						if (!typeof(NodeEditor).IsAssignableFrom(type)) 
						{
							Debug.LogError("The class \""+type.FullName+"\" tries to include the \"NodeDrawer\" attribute without inheriting from the \"NodeEditor\" class");
							continue;
						}
						var attribute = attributes[0] as NodeDrawer;
						_Editors.Add(attribute.Target, new EditorEntry { Details = attribute, Editor = Activator.CreateInstance(type) as NodeEditor } );
					}
				}
			}
		}
	}
}