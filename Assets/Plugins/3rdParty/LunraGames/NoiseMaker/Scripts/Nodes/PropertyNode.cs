namespace LunraGames.NoiseMaker
{
	public abstract class PropertyNode<T> : Node<T>, IPropertyNode
	{
		public bool IsEditable { get; set; }

		public abstract void SetProperty(T value);

		public void SetProperty(object value)
		{
			SetProperty((T) value);
		}
	}
}