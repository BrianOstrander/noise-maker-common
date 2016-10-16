using UnityEngine;
using UnityEditor;

namespace LunraGames.NoiseMaker
{
	[NodeDrawer(typeof(RemapSimpleNode), Strings.Modifiers, "Remap Simple", Strings.SpecifyAnInput)]
	public class RemapSimpleNodeEditor : NodeEditor {}
}