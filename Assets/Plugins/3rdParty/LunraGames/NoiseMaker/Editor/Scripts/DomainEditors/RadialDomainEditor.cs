using UnityEngine;
using UnityEditor;
using LibNoise;

namespace LunraGames.NoiseMaker
{
	[DomainDrawer(typeof(RadialDomain), "Radial", "Define a radial constraint for a Biome")]
	public class RadialDomainEditor : DomainEditor
	{
		
		public override Domain Draw(Domain domain, object module, out DomainPreview preview)
		{
			var radial = domain as RadialDomain;

			preview = GetPreview(radial, module);

			radial.Center = Deltas.DetectDelta(radial.Center, EditorGUILayout.Vector2Field("Center", radial.Center), ref preview.Stale);
			radial.Radius = Deltas.DetectDelta(radial.Radius, EditorGUILayout.FloatField("Radius", radial.Radius), ref preview.Stale);

			return domain;
		}
	}
}