namespace LunraGames.UniformUI
{
	public class UniformButton : UnityEngine.UI.Button 
	{
		public UniformButtonConfig Config;

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
				transition = Config.Transition;
				colors = Config.Colors;
			}
		}
	}
}
