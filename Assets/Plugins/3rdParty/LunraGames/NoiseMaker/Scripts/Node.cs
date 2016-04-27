using UnityEngine;
using System.Collections.Generic;
using Atesh;

namespace LunraGames.NoiseMaker
{
	public abstract class Node
	{
		public string Id;
		public string Name;
		public Rect Position = new Rect(100f, 100f, 150f, 200f);
		public List<Connection<object>> Inputs = new List<Connection<object>>();
		public List<Connection<object>> Outputs = new List<Connection<object>>();
		public List<Field> Fields = new List<Field>();

		public void Draw()
		{
			Position = GUILayout.Window(0, Position, id =>
			{
				OnDraw();
				GUI.DragWindow();
			}, StringExtensions.IsNullOrWhiteSpace(Name) ? "Node" : Name);
		}

		protected abstract void OnDraw();

		protected void DrawFields()
		{
			foreach (var field in Fields) field.Draw();
		}
	}
}