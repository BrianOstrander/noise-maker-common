namespace LunraGames.NoiseMaker
{
	public interface IPropertyNode : INode
	{
		bool IsEditable { get; set; }
		void SetProperty(object value);
	}
}