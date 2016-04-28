using UnityEngine;
using System.Collections;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(PerlinNode), Strings.Generators, "Perlin")]
	public class PerlinNodeEditor : NodeEditor
	{
		public override Node Draw(Node node)
		{
			GUILayout.Label("lol");
			return node;
		}
	}
}