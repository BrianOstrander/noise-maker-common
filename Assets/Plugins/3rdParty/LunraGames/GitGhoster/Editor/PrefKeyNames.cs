namespace LunraGames.GitGhoster
{
	public static class PrefKeyNames
	{
		const string Prefix = "GGh_";
		public const string IsActive = Prefix+"IsActive";
		public const string CheckPeriod = Prefix+"CheckPeriod";
		public const string LastGitCommit = Prefix+"LastGitCommit";
		public const string LogOnNoCleanup = Prefix+"LogOnNoCleanup";
		public const string LogOnCleanup = Prefix+"LogOnCleanup";
	}
}