using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LunraGames.NoiseMaker
{
	[InitializeOnLoad]
	sealed class NodeEditorCacher : Editor 
	{
		static Dictionary<Type, NodeEditorEntry> _Editors = new Dictionary<Type, NodeEditorEntry>();

		internal static Dictionary<Type, NodeEditorEntry> Editors { get { return new Dictionary<Type, NodeEditorEntry>(_Editors); }}

		static NodeEditorCacher()
		{
			Refresh();
		}

		static void Refresh()
		{
			_Editors = new Dictionary<Type, NodeEditorEntry>();
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

						var linkers = new List<NodeLinker>();

						var fields = attribute.Target.GetFields();
						foreach (var field in fields)
						{
							var unmodifiedField = field;
							var fieldAttributes = unmodifiedField.GetCustomAttributes(typeof(NodeLinker), true);
							if (0 < fieldAttributes.Length) 
							{
								var fieldAttribute = fieldAttributes[0] as NodeLinker;
								fieldAttribute.Type = unmodifiedField.FieldType;
								fieldAttribute.Name = fieldAttribute.Name ?? ObjectNames.NicifyVariableName(unmodifiedField.Name);
								fieldAttribute.Field = unmodifiedField;
								linkers.Add(fieldAttribute);
							}
						}

						_Editors.Add(attribute.Target, new NodeEditorEntry { Details = attribute, Editor = Activator.CreateInstance(type) as NodeEditor, Linkers = linkers, IsEditable = typeof(IPropertyNode).IsAssignableFrom(attribute.Target) } );
					}
				}
			}
		}
	}
}