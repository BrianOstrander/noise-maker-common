using UnityEngine;
using System.Collections;
using UnityEditor;
using Atesh;
using System;
using Newtonsoft.Json;
using LunraGames.Converters;

namespace LunraGames
{
	public static class Serialization 
	{
		static JsonConverter[] Converters = 
		{
			new Vector2Converter(),
			new Vector3Converter(),
			new Vector4Converter(),
			new QuaternionConverter(),
			new ColorConverter()
		};

		static JsonSerializerSettings _SerializerSettings;

		static JsonSerializerSettings SerializerSettings 
		{
			get
			{
				if (_SerializerSettings == null)
				{
					_SerializerSettings = new JsonSerializerSettings();
					_SerializerSettings.TypeNameHandling = TypeNameHandling.All;
					_SerializerSettings.ObjectCreationHandling = ObjectCreationHandling.Replace;
					foreach (var converter in Converters) _SerializerSettings.Converters.Add(converter);
				}
				return _SerializerSettings;
			}
		}
		
		public static T DeserializeJson<T>(string json, T defaultValue = default(T))
		{
			if (StringExtensions.IsNullOrWhiteSpace(json)) return defaultValue;

			try 
			{
				return JsonConvert.DeserializeObject<T>(json, SerializerSettings);
			}
			catch (Exception e)
			{
				Debug.LogError("Problem parsing value: \n\t"+json+"\nReturning default value\n Exception:\n"+e.Message);
				return defaultValue;
			}
		}

		public static string SerializeJson(object value)
		{
			return value == null ? string.Empty : JsonConvert.SerializeObject(value, Formatting.None, SerializerSettings);
		}
	}
}