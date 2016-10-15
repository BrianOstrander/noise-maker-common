using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	[InitializeOnLoad]
	sealed class AltitudeEditorCacher : Editor 
	{
		static Dictionary<Type, AltitudeEditorEntry> _Editors = new Dictionary<Type, AltitudeEditorEntry>();

		internal static Dictionary<Type, AltitudeEditorEntry> Editors { get { return new Dictionary<Type, AltitudeEditorEntry>(_Editors); }}

		static AltitudeEditorCacher()
		{
			Refresh();
		}

		static void Refresh()
		{
			_Editors = new Dictionary<Type, AltitudeEditorEntry>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types) 
				{
					var attributes = type.GetCustomAttributes(typeof(AltitudeDrawer), true);
					if (0 < attributes.Length && !_Editors.ContainsKey(type)) 
					{
						if (!typeof(AltitudeEditor).IsAssignableFrom(type)) 
						{
							Debug.LogError("The class \""+type.FullName+"\" tries to include the \"AltitudeDrawer\" attribute without inheriting from the \"AltitudeEditor\" class");
							continue;
						}
						var attribute = attributes[0] as AltitudeDrawer;
						_Editors.Add(attribute.Target, new AltitudeEditorEntry { Details = attribute, Editor = Activator.CreateInstance(type) as AltitudeEditor } );
					}
				}
			}
		}
	}
}