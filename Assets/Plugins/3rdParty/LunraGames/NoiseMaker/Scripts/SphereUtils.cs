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

		// Taken from http://answers.unity3d.com/questions/189724/polar-spherical-coordinates-to-xyz-and-vice-versa.html
		public static Vector2 CartesianToPolar(Vector3 point)
		{
			var polar = new Vector2();

			//calc longitude
			polar.y = Mathf.Atan2(point.x, point.z);

			//this is easier to write and read than sqrt(pow(x,2), pow(y,2))!
			var xzLen = new Vector2(point.x,point.z).magnitude; 
			//atan2 does the magic
			polar.x = Mathf.Atan2(-point.y, xzLen);

			//convert to deg
			polar *= Mathf.Rad2Deg;

			return polar;
		}
	}
}