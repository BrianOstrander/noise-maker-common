// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Xtro
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Transform))]
    // ReSharper disable once UnusedMember.Global
    sealed class TransformInspector : Editor
    {
        SerializedProperty Position;
        SerializedProperty Rotation;
        SerializedProperty Scale;

        #region Messages
        // ReSharper disable UnusedMember.Local
        void OnEnable()
        {
            Position = serializedObject.FindProperty("m_LocalPosition");
            Rotation = serializedObject.FindProperty("m_LocalRotation");
            Scale = serializedObject.FindProperty("m_LocalScale");
        }

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.labelWidth = 15f;

            serializedObject.Update();

            DrawPosition();
            DrawRotation();
            DrawScale();

            serializedObject.ApplyModifiedProperties();
        }
        // ReSharper restore UnusedMember.Local
        #endregion

        void DrawPosition()
        {
            GUILayout.BeginHorizontal();

            var Reset = GUILayout.Button("P", GUILayout.Width(20f));

            EditorGUILayout.PropertyField(Position.FindPropertyRelative("x"));
            EditorGUILayout.PropertyField(Position.FindPropertyRelative("y"));
            EditorGUILayout.PropertyField(Position.FindPropertyRelative("z"));

            GUILayout.EndHorizontal();

            if (Reset) Position.vector3Value = Vector3.zero;
        }

        void DrawScale()
        {
            GUILayout.BeginHorizontal();
            {
                var Reset = GUILayout.Button("S", GUILayout.Width(20f));

                EditorGUILayout.PropertyField(Scale.FindPropertyRelative("x"));
                EditorGUILayout.PropertyField(Scale.FindPropertyRelative("y"));
                EditorGUILayout.PropertyField(Scale.FindPropertyRelative("z"));

                if (Reset) Scale.vector3Value = Vector3.one;
            }
            GUILayout.EndHorizontal();
        }

        [Flags]
        enum Axes
        {
            None = 0,
            X = 1,
            Y = 2,
            Z = 4,
            All = 7
        }

        static Axes CheckDifference(Transform Transform, Vector3 Original)
        {
            var Next = Transform.localEulerAngles;

            var Result = Axes.None;

            if (Differs(Next.x, Original.x)) Result |= Axes.X;
            if (Differs(Next.y, Original.y)) Result |= Axes.Y;
            if (Differs(Next.z, Original.z)) Result |= Axes.Z;

            return Result;
        }

        Axes CheckDifference(SerializedProperty Property)
        {
            var Result = Axes.None;

            if (Property.hasMultipleDifferentValues)
            {
                var Original = Property.quaternionValue.eulerAngles;

                foreach (var Object in serializedObject.targetObjects)
                {
                    Result |= CheckDifference(Object as Transform, Original);
                    if (Result == Axes.All) break;
                }
            }

            return Result;
        }

        /// <summary>
        /// Draw an editable float field.
        /// </summary>
        /// <param name="Value">Value of the field</param>
        /// <param name="Hidden">Whether to replace the value with a dash</param>
        /// <param name="GreyedOut">Whether the value should be greyed out or not</param>
        /// <param name="Name">Name of the field</param>
        /// <param name="Option">Options</param>
        static bool FloatField(string Name, ref float Value, bool Hidden, bool GreyedOut, GUILayoutOption Option)
        {
            var NewValue = Value;
            GUI.changed = false;

            if (!Hidden)
            {
                if (GreyedOut)
                {
                    GUI.color = new Color(0.7f, 0.7f, 0.7f);
                    NewValue = EditorGUILayout.FloatField(Name, NewValue, Option);
                    GUI.color = Color.white;
                }
                else
                {
                    NewValue = EditorGUILayout.FloatField(Name, NewValue, Option);
                }
            }
            else if (GreyedOut)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                float.TryParse(EditorGUILayout.TextField(Name, "--", Option), out NewValue);
                GUI.color = Color.white;
            }
            else
            {
                float.TryParse(EditorGUILayout.TextField(Name, "--", Option), out NewValue);
            }

            if (GUI.changed && Differs(NewValue, Value))
            {
                Value = NewValue;
                return true;
            }
            return false;
        }

        // Because Mathf.Approximately is too sensitive.
        static bool Differs(float A, float B)
        {
            return Mathf.Abs(A - B) > 0.0001f;
        }

        void DrawRotation()
        {
            GUILayout.BeginHorizontal();
            {
                var Reset = GUILayout.Button("R", GUILayout.Width(20f));

                var Visible = ((Transform)serializedObject.targetObject).localEulerAngles;
                var Changed = CheckDifference(Rotation);
                var Altered = Axes.None;

                var Opt = GUILayout.MinWidth(30f);

                if (FloatField("X", ref Visible.x, (Changed & Axes.X) != 0, false, Opt)) Altered |= Axes.X;
                if (FloatField("Y", ref Visible.y, (Changed & Axes.Y) != 0, false, Opt)) Altered |= Axes.Y;
                if (FloatField("Z", ref Visible.z, (Changed & Axes.Z) != 0, false, Opt)) Altered |= Axes.Z;

                if (Reset)
                {
                    Rotation.quaternionValue = Quaternion.identity;
                }
                else if (Altered != Axes.None)
                {
                    RegisterUndo("Change Rotation", serializedObject.targetObjects);

                    foreach (var Object in serializedObject.targetObjects)
                    {
                        var Transform = (Transform)Object;
                        var Angles = Transform.localEulerAngles;

                        if ((Altered & Axes.X) != 0) Angles.x = Visible.x;
                        if ((Altered & Axes.Y) != 0) Angles.y = Visible.y;
                        if ((Altered & Axes.Z) != 0) Angles.z = Visible.z;

                        Transform.localEulerAngles = Angles;
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        static void RegisterUndo(string Name, params Object[] Objects)
        {
            if (Objects == null || Objects.Length <= 0) return;

            Undo.RecordObjects(Objects, Name);

            foreach (var Object in Objects.Where(Object => Object != null))
            {
                EditorUtility.SetDirty(Object);
            }
        }
    }
}