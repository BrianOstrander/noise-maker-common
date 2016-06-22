using UnityEngine;

namespace LunraGames
{
	public static class ColorExtensions
	{
		#region Rgb
		public static Color NewR(this Color color, float r)
        {	
        	return new Color(r, color.g, color.b, color.a);
		}

		public static Color NewG(this Color color, float g)
        {	
			return new Color(color.r, g, color.b, color.a);
		}

		public static Color NewB(this Color color, float b)
        {	
			return new Color(color.r, color.g, b, color.a);
        }

		public static Color NewRgba(this Color color, float? r = null, float? g = null, float? b = null, float? a = null)
        {	
        	return new Color(r ?? color.r, g ?? color.g, b ?? color.b, a ?? color.a);
		}
		#endregion

		#region Hsv
		public static Color NewH(this Color color, float h)
        {	
        	return NewHsva(color, h);
		}

		public static Color NewS(this Color color, float s)
        {	
			return NewHsva(color, s: s);
		}

		public static Color NewV(this Color color, float v)
        {	
			return NewHsva(color, v: v);
        }

		public static Color NewHsva(this Color color, float? h = null, float? s = null, float? v = null, float? a = null, bool hdr = false)
        {	
        	float wasH;
			float wasS;
			float wasV;
			Color.RGBToHSV(color, out wasH, out wasS, out wasV);
			return Color.HSVToRGB(h ?? wasH, s ?? wasS, v ?? wasV, hdr).NewA(a ?? color.a);
		}
		#endregion

		#region Shared
		public static Color NewA(this Color color, float a)
		{	
			return new Color(color.r, color.g, color.b, a);
		}
		#endregion
	}
}