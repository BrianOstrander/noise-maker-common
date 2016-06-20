namespace LunraGames.NoiseMaker
{
	public interface IPropertyNode : INode
	{
		bool IsEditable { get; set; }
		object RawPropertyValue { get; set; }
	}
}