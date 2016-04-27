namespace LunraGames.UniformUI
{
	public class UniformImage : UnityEngine.UI.Image
	{
		public UniformImageConfig Config;

		protected override void Awake()
		{
			base.Awake();
			ApplyConfig();
		}
		// If we're in the editor, we apply the config every frame so we can see changes live.
		#if UNITY_EDITOR
		void Update()
		{
			ApplyConfig();
		}
		#endif
		void ApplyConfig()
		{
			if (Config != null)
			{
				color = Config.Color;
			}
		}
	}
}
