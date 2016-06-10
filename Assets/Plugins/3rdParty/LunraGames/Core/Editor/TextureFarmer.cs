using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace LunraGames
{
	public class TextureFarmer 
	{
		const int PixelBudget = 1024;

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
			if (Entries == null || Entries.Count == 0) return;

			var remainingBudget = PixelBudget;
			var deletions = new List<Entry>();

			foreach (var entry in Entries)
			{
				remainingBudget -= entry.Target.width;

				var pixels = new Color[entry.Target.width];
				var start = entry.YProgress * entry.Target.width;
				var end = start + entry.Target.width;

				for (var i = start; i < end; i++) pixels[i - start] = entry.Replacements[i];

				entry.Target.SetPixels(0, entry.YProgress, entry.Target.width, 1, pixels);
				entry.Target.Apply();

				entry.YProgress++;

				if (entry.YProgress == entry.Target.height) deletions.Add(entry);

				if (remainingBudget <= 0) break;
			}

			foreach (var deletion in deletions) Entries.Remove(deletion);
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