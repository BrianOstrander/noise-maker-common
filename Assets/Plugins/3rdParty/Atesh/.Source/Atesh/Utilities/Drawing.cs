// Line drawing routine originally courtesy of Linusmartensson:
// http://forum.unity3d.com/threads/71979-Drawing-lines-in-the-editor
//
// Rewritten to improve performance by Yossarian King / August 2013.
//
// Modified by Onur "Xtro" Er and included in Atesh Framework / November 2014.

using System.Reflection;
using UnityEngine;

namespace Atesh
{
    public static class Drawing
    {
        static Texture2D AntiAliasingLineTexture;
        static Texture2D LineTexture;
        static Material BlitMaterial;
        static Material BlendMaterial;
        static readonly Rect LineRect = new Rect(0, 0, 1, 1);
        static readonly MethodInfo GUI_GetBlitMaterialMethod = typeof(GUI).GetMethod("get_blitMaterial", BindingFlags.NonPublic | BindingFlags.Static);
        static readonly MethodInfo GUI_GetBlendMaterialMethod = typeof(GUI).GetMethod("get_blendMaterial", BindingFlags.NonPublic | BindingFlags.Static);

        // Draw a line in screen space, suitable for use from OnGUI calls from either
        // MonoBehaviour or EditorWindow. Note that this should only be called during repaint
        // events, when (Event.current.type == EventType.Repaint).
        //
        // Works by computing a matrix that transforms a unit square -- Rect(0,0,1,1) -- into
        // a scaled, rotated, and offset rectangle that corresponds to the line and its width.
        // A DrawTexture call used to draw a line texture into the transformed rectangle.
        //
        // More specifically:
        //      scale x by line length, y by line width
        //      rotate around z by the angle of the line
        //      offset by the position of the upper left corner of the target rectangle
        //
        public static void DrawLine(Vector2 Point1, Vector2 Point2, Color Color, float Width, bool AntiAlias)
        {
            // Normally the static initializer does this, but to handle texture reinitialization
            // after editor play mode stops we need this check in the Editor.
            if (Application.isEditor && !LineTexture) Initialize();

            // Note that theta = atan2(dy, dx) is the angle we want to rotate by, but instead
            // of calculating the angle we just use the sine (dy/len) and cosine (dx/len).
            var X = Point2.x - Point1.x;
            var Y = Point2.y - Point1.y;
            var Length = Mathf.Sqrt(X * X + Y * Y);

            // Early out on tiny lines to avoid divide by zero.
            // Plus what's the point of drawing a line 1/1000th of a pixel long??
            if (Length < 0.001f) return;

            // Pick texture and material (and tweak width) based on anti-alias setting.
            Texture2D Texture;
            Material Material;

            if (AntiAlias)
            {
                // Multiplying by three is fine for anti-aliasing width-1 lines, but make a wide "fringe"
                // for thicker lines, which may or may not be desirable.
                Width = Width * 3.0f;
                Texture = AntiAliasingLineTexture;
                Material = BlendMaterial;
            }
            else
            {
                Texture = LineTexture;
                Material = BlitMaterial;
            }

            var Wdx = Width * Y / Length;
            var Wdy = Width * X / Length;

            var Matrix = Matrix4x4.identity;
            Matrix.m00 = X;
            Matrix.m01 = -Wdx;
            Matrix.m03 = Point1.x + 0.5f * Wdx;
            Matrix.m10 = Y;
            Matrix.m11 = Wdy;
            Matrix.m13 = Point1.y - 0.5f * Wdy;

            // Use GL matrix and Graphics.DrawTexture rather than GUI.matrix and GUI.DrawTexture,
            // for better performance. (Setting GUI.matrix is slow, and GUI.DrawTexture is just a
            // wrapper on Graphics.DrawTexture.)
            GL.PushMatrix();
            GL.MultMatrix(Matrix);
            Graphics.DrawTexture(LineRect, Texture, LineRect, 0, 0, 0, 0, Color, Material);
            GL.PopMatrix();
        }

        // Other than method name, DrawBezierLine is unchanged from Linusmartensson's original implementation.
        public static void DrawBezierLine(Bezier2 Bezier, Color Color, float Width, bool AntiAlias, int Segments)
        {
            var LastPoint = Bezier.Interpolate(0);
            for (var I = 1; I < Segments; ++I)
            {
                var Point = Bezier.Interpolate(I / (float)Segments);
                DrawLine(LastPoint, Point, Color, Width, AntiAlias);
                LastPoint = Point;
            }
        }

        // This static initializer works for runtime, but apparently isn't called when
        // Editor play mode stops, so DrawLine will re-initialize if needed.
        static Drawing()
        {
            Initialize();
        }

        static void Initialize()
        {
            if (LineTexture == null)
            {
                LineTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                LineTexture.SetPixel(0, 1, Color.white);
                LineTexture.Apply();
            }

            if (AntiAliasingLineTexture == null)
            {
                AntiAliasingLineTexture = new Texture2D(1, 3, TextureFormat.ARGB32, false);
                AntiAliasingLineTexture.SetPixel(0, 0, new Color(1, 1, 1, 0));
                AntiAliasingLineTexture.SetPixel(0, 1, Color.white);
                AntiAliasingLineTexture.SetPixel(0, 2, new Color(1, 1, 1, 0));
                AntiAliasingLineTexture.Apply();
            }

            // GUI.blitMaterial and GUI.blendMaterial are used internally by GUI.DrawTexture,
            // depending on the alphaBlend parameter. Use reflection to "borrow" these references.
            BlitMaterial = (Material)GUI_GetBlitMaterialMethod.Invoke(null, null);
            BlendMaterial = (Material)GUI_GetBlendMaterialMethod.Invoke(null, null);
        }

        public static void CleanUp()
        {
            if (LineTexture)
            {
                Object.DestroyImmediate(LineTexture);
                LineTexture = null;
            }

            if (AntiAliasingLineTexture)
            {
                Object.DestroyImmediate(AntiAliasingLineTexture);
                LineTexture = null;
            }
        }
    }
}