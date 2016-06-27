using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace LunraGames.NoiseMaker
{
	[InitializeOnLoad]
	sealed class DomainEditorCacher : Editor 
	{
		static Dictionary<Type, DomainEditorEntry> _Editors = new Dictionary<Type, DomainEditorEntry>();

		internal static Dictionary<Type, DomainEditorEntry> Editors { get { return new Dictionary<Type, DomainEditorEntry>(_Editors); }}

		static DomainEditorCacher()
		{
			Refresh();
		}

		static void Refresh()
		{
			_Editors = new Dictionary<Type, DomainEditorEntry>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types) 
				{
					var attributes = type.GetCustomAttributes(typeof(DomainDrawer), true);
					if (0 < attributes.Length && !_Editors.ContainsKey(type)) 
					{
						if (!typeof(DomainEditor).IsAssignableFrom(type)) 
						{
							Debug.LogError("The class \""+type.FullName+"\" tries to include the \"DomainDrawer\" attribute without inheriting from the \"DomainEditor\" class");
							continue;
						}
						var attribute = attributes[0] as DomainDrawer;

						_Editors.Add(attribute.Target, new DomainEditorEntry { Details = attribute, Editor = Activator.CreateInstance(type) as DomainEditor } );
					}
				}
			}
		}
	}
}