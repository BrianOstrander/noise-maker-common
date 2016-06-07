using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace LunraGames
{
	public class TextureFarmer 
	{
		class Entry
		{
			public Texture2D Target;
			public Color[] Replacements;
			//public int XProgress;
			public int YProgress;
		}

		static List<Entry> Entries = new List<Entry>();

		[InitializeOnLoadMethod]
		static void Initialize()
		{
			EditorApplication.update += Farm;
		}

		static void Farm()
		{
			var first = Entries.FirstOrDefault();

			if (first == null) return;

			var pixels = new Color[first.Target.width];
			var start = first.YProgress * first.Target.width;
			var end = start + first.Target.width;

			for (var i = start; i < end; i++) pixels[i - start] = first.Replacements[i];

			first.Target.SetPixels(0, first.YProgress, first.Target.width, 1, pixels);
			first.Target.Apply();

			first.YProgress++;

			if (first.YProgress == first.Target.height) Entries.Remove(first);
		}

		public static void Queue(Texture2D target, Color[] replacements)
		{
			var entry = Entries.FirstOrDefault(e => target == e.Target);

			if (entry == null)
			{
				entry = new Entry {
					Target = target,
					Replacements = replacements
				};
				Entries.Add(entry);
			}
			else
			{
				entry.Replacements = replacements;
				//entry.XProgress = 0;
				entry.YProgress = 0;
			}
		}
	}
}