using System.Collections.Generic;
using Rotorz.ReorderableList;
using UnityEditor;
using UnityEngine;

namespace Atesh.WeNeedCreatedMessage.Editor
{
    [CustomEditor(typeof(ScriptExecutionOrder))]
    public class ScriptExecutionOrderEditor : UnityEditor.Editor
    {
        class Adaptor : GenericListAdaptor<string>
        {
            public Adaptor(IList<string> List, ReorderableListControl.ItemDrawer<string> ItemDrawer, float ItemHeight) : base(List, ItemDrawer, ItemHeight) { }

            public override void DrawItem(Rect Position, int Index)
            {
                var Item = this[Index];
                if (Item == Strings.DefaultTime) EditorGUI.LabelField(Position, Item);
                else base.DrawItem(Position, Index);
            }

            public override bool CanRemove(int Index) => this[Index] != Strings.DefaultTime;
        }

        ScriptExecutionOrder Target;
        Adaptor ScriptsAdaptor;
        ReorderableListControl ListControl;

        #region Messages
        // ReSharper disable UnusedMember.Local
        void OnEnable()
        {
            Target = (ScriptExecutionOrder)target;
            ScriptsAdaptor = new Adaptor(Target.Scripts, DrawListItem, ReorderableListGUI.DefaultItemHeight);
            ListControl = new ReorderableListControl();

            if (!Target.Scripts.Contains(Strings.DefaultTime)) Target.Scripts.Add(Strings.DefaultTime);
        }
        // ReSharper restore UnusedMember.Local
        #endregion

        public override void OnInspectorGUI()
        {
            ReorderableListGUI.Title("Script Full Names");
            ListControl.Draw(ScriptsAdaptor);

            EditorUtility.SetDirty(Target);
        }

        static string DrawListItem(Rect Position, string Value) => EditorGUI.TextField(Position, Value);
    }
}