// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Atesh.MagicAutoLinker.Editor
{
    class AutoLinkerPreviewWindow : EditorWindow
    {
        static readonly FieldInfo AutoLinkerPreviewField = typeof(AutoLinker).GetField("Preview", BindingFlags.Static | BindingFlags.NonPublic);

        internal static AutoLinker Target;
        static AutoLinkerNode Current;
        static ComponentNode CurrentComponent;
        static MemberNode CurrentMember;
        static Stack<MemberNode> MemberStack;

        static Vector2 AutoLinkersScrollPosition = Vector2.zero;
        static Vector2 GameObjectsScrollPosition = Vector2.zero;
        static Rect AutoLinkersScrollViewRect;
        static bool IncludeChildAutoLinkers = EditorPrefs.GetBool(IncludeChildAutoLinkersPrefName);
        static bool DrawAllLinks = EditorPrefs.GetBool(DrawAllLinksPrefName);
        static readonly Vector2 TangentVector = Vector2.right * 200;
        static readonly Bezier2 Bezier = new Bezier2(1);

        const string IncludeChildAutoLinkersPrefName = "IncludeChildAutoLinkers";
        const string DrawAllLinksPrefName = "DrawAllLinks";

        static AutoLinkerPreviewWindow Instance => GetWindow<AutoLinkerPreviewWindow>("AL Preview");


        internal static new void Show()
        {
            Instance.minSize = new Vector2(600, 600);
            Preview();
        }

        static AutoLinkerPreviewWindow()
        {
            AddEvent(nameof(RegisterComponent));
            AddEvent(nameof(RegisterMember));
            AddEvent(nameof(RegisterValue));
            AddEvent(nameof(CollectionContains));
            AddEvent(nameof(BeginRegisteringSubMembers));
            AddEvent(nameof(EndRegisteringSubMembers));
            AddEvent(nameof(RegisterCollection));
            AddEvent(nameof(RegisterError));
        }

        static void AddEvent(string Name)
        {
            var Event = typeof (AutoLinker).GetEvent(Name, BindingFlags.Static | BindingFlags.NonPublic);
            var Method = typeof (AutoLinkerPreviewWindow).GetMethod(Name, BindingFlags.Static | BindingFlags.NonPublic);
            Event.GetAddMethod(true).Invoke(null, new object[] {Delegate.CreateDelegate(Event.EventHandlerType, Method)});
        }

        #region Messages
        // ReSharper disable UnusedMember.Local
        void OnGUI()
        {
            try
            {
                DrawToolbar();
                DrawPreviewArea();

                if (Event.current.type == EventType.Repaint) AutoLinkerNode.Root.DrawLink();
            }
            catch (MissingReferenceException)
            {
                Show();
            }
        }

        void OnDestroy() => Drawing.CleanUp();

        void OnSelectionChange() => Repaint();
        // ReSharper restore UnusedMember.Local
        #endregion

        static void Preview()
        {
            AutoLinkerPreviewField.SetValue(null, true);
            AutoLinkerNode.ClearRoot();
            TransformNode.ClearRoot();
            MemberStack = new Stack<MemberNode>();

            if (Target)
            {
                if (IncludeChildAutoLinkers) Target.transform.Process(Process, AutoLinkerNode.Root);
                else Process(Target.transform, AutoLinkerNode.Root);
            }
            else
            {
                foreach (var Transform in Utilities.SceneRoots())
                {
                    Transform.Process(Process, AutoLinkerNode.Root);
                }
            }

            AutoLinkerPreviewField.SetValue(null, false);
        }

        static AutoLinkerNode Process(Transform Transform, AutoLinkerNode ParentNode)
        {
            var AutoLinker = Transform.GetComponent<AutoLinker>();

            if (AutoLinker)
            {
                TransformNode.AddTransform(Transform); 
                
                Current = ParentNode.NewAutoLinker(AutoLinker);
                AutoLinker.ProcessAllComponents();
            }
            else Current = null;

            return Current ?? ParentNode;
        }

        static void RegisterComponent(Component Component) => CurrentComponent = Current.NewComponent(Component);

        static void RegisterMember(MemberInfo Member)
        {
            var StackedMember = MemberStack.Count > 0 ? MemberStack.Peek() : null;

            CurrentMember = StackedMember == null ? CurrentComponent.NewMember(Member) : StackedMember.NewMember(Member);
        }

        static void RegisterValue(Object Value)
        {
            if (Value is Component) TransformNode.AddTransform(((Component)Value).transform);
            if (Value is GameObject) TransformNode.AddTransform(((GameObject)Value).transform);

            CurrentMember.Values.Add(new MemberNode.ObjectValue { Object = Value });
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        static bool CollectionContains(Object Value) => CurrentMember.Values.Any(X => X.Object == Value);

        static void BeginRegisteringSubMembers() => MemberStack.Push(CurrentMember);

        static void EndRegisteringSubMembers() => MemberStack.Pop();

        static void RegisterCollection() => CurrentMember.IsCollection = true;

        static void RegisterError(string Text, bool IsWarning)
        {
            if (CurrentMember.IsCollection)
            {
                var Last = CurrentMember.Values[CurrentMember.Values.Count - 1];
                Last.Error = Text;
                Last.IsWarning = IsWarning;
            }
            else
            {
                CurrentMember.Error = Text;
                CurrentMember.IsWarning = IsWarning;
            }
        }

        internal static string GetIndentBlank(int Level)
        {
            var Builder = new StringBuilder();

            for (var I = 0; I < Level; I++)
            {
                Builder.Append("   ");
            }

            return Builder.ToString();
        }

        internal static void DrawLink(TransformNode TransformNode, Rect Rect, Color Color)
        {
            Bezier.Point1 = new Vector2(Rect.max.x, Rect.max.y + Rect.height / 2) - AutoLinkersScrollPosition;

            if (!DrawAllLinks && (Bezier.Point1.y < 0 || Bezier.Point1.y > AutoLinkersScrollViewRect.height)) return;

            Bezier.Point2 = Bezier.Point1 + TangentVector;
            Bezier.Point4 = new Vector2(TransformNode.Rect.min.x + AutoLinkersScrollViewRect.width, TransformNode.Rect.min.y + TransformNode.Rect.height * 1.5f) - GameObjectsScrollPosition;
            Bezier.Point3 = Bezier.Point4 - TangentVector;
            Color.a = 0.2f;
            Drawing.DrawBezierLine(Bezier, Color, 2, true, 50);
        }

        static void DrawPreviewArea()
        {
            GUILayout.BeginHorizontal();
            {
                // AutoLinkers at left
                AutoLinkersScrollPosition = GUILayout.BeginScrollView(AutoLinkersScrollPosition);
                {
                    AutoLinkerNode.Root.OnGUI();
                }
                GUILayout.EndScrollView();
                if (Event.current.type == EventType.Repaint) AutoLinkersScrollViewRect = GUILayoutUtility.GetLastRect();

                // GameObjects at right
                GameObjectsScrollPosition = GUILayout.BeginScrollView(GameObjectsScrollPosition, GUILayout.MinWidth(250));
                {
                    TransformNode.Root.OnGUI();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndHorizontal();
        }

        static void DrawToolbar()
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) Preview();
                if (Target)
                {
                    if (GUILayout.Button("Preview All", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
                    {
                        Target = null;
                        Preview();
                    }

                    var NewIncludeChildAutoLinkers = GUILayout.Toggle(IncludeChildAutoLinkers, "Include Child AutoLinkers", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
                    if (IncludeChildAutoLinkers != NewIncludeChildAutoLinkers)
                    {
                        IncludeChildAutoLinkers = NewIncludeChildAutoLinkers;
                        EditorPrefs.SetBool(IncludeChildAutoLinkersPrefName, IncludeChildAutoLinkers);
                        Preview();
                    }
                }
                else
                {
                    var SelectedObject = Selection.activeGameObject;
                    var SelectedAutoLinker = SelectedObject ? SelectedObject.GetComponent<AutoLinker>() : null;
                    if (SelectedAutoLinker && GUILayout.Button("Preview Selected", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
                    {
                        Target = SelectedAutoLinker;
                        Preview();
                    }
                }

                GUILayout.Label("", EditorStyles.toolbarButton);

                var NewDrawAllLinks = GUILayout.Toggle(DrawAllLinks, "Draw All Links", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
                if (DrawAllLinks != NewDrawAllLinks)
                {
                    DrawAllLinks = NewDrawAllLinks;
                    EditorPrefs.SetBool(DrawAllLinksPrefName, DrawAllLinks);
                    Preview();
                }

                GUILayout.Label($"Previewing: {(Target != null ? Target.name : "All")}", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
            }
            GUILayout.EndHorizontal();
        }
    }
}