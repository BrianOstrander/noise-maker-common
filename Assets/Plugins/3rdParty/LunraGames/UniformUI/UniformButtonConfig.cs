using UnityEngine;
using UnityEngine.UI;

namespace LunraGames.UniformUI
{
	public class UniformButtonConfig : ScriptableObject 
	{
		public Selectable.Transition Transition = Selectable.Transition.ColorTint;
		public ColorBlock Colors = ColorBlock.defaultColorBlock;
	}
}