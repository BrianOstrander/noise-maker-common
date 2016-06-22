namespace LunraGames
{
	public static class StringExtensions 
	{
		public static bool IsNullOrWhiteSpace(string value)
		{
			return string.IsNullOrEmpty(value) || value.Trim().Length == 0;

		}
	}
}