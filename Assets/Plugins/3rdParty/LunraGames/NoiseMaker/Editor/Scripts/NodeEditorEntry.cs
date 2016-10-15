using System.Collections.Generic;

namespace LunraGames.NoiseMaker
{
	public class NodeEditorEntry
	{
		public NodeDrawer Details;
		public NodeEditor Editor;
		public List<NodeLinker> Linkers;
		public bool IsEditable;
	}
}