using UnityEngine;

namespace LunraGames.NoiseMaker
{
	public static class SphereUtils 
	{
		public static float GetLatitude(int y, int height) 
		{
			return GetLatitude((float)y, (float)height);
		}

		public static float GetLatitude(float y, float height) 
		{
			return (((height - y) / height) * 180f) - 90f;
		}

		public static float GetLongitude(int x, int width) 
		{
			return GetLongitude((float)x, (float)width);
		}

		public static float GetLongitude(float x, float width) 
		{
			return (((width - x) / width) * 360f) - 180f;
		}
	}
}