// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Atesh
{
    public static class TransformExtensions
    {
        public static void ClearChildren(this Transform This)
        {
            foreach (Transform Child in This)
            {
                Object.Destroy(Child.gameObject);
            }
        }

        /// <summary>
        /// Depth first. Transform.Find only searches for immediate children but this method searches for all the desendants.
        /// Unlike Transform.Find, this method doen't support path name search such as "Body/Arm/Hand"
        /// </summary>
        public static Transform FindInDescendants(this Transform This, string Name)
        {
            if (This.name == Name) return This;

            for (var I = 0; I < This.childCount; I++)
            {
                var Result = FindInDescendants(This.GetChild(I), Name);
                if (Result != null) return Result;
            }

            return null;
        }

        /// <summary>
        /// Enumarates transform's descendants by breadth first method.
        /// </summary>
        public static IEnumerable<Transform> Descendants(this Transform This)
        {
            var Transforms = new Queue<Transform>(new[] { This });
            while (Transforms.Any())
            {
                var Transform = Transforms.Dequeue();
                if (Transform != This) yield return Transform;

                foreach (var Child in Transform) Transforms.Enqueue((Transform)Child);
            }
        }

        public static IEnumerable<Transform> Parents(this Transform This)
        {
            var Transforms = new List<Transform>();

            var Parent = This.parent;
            while (Parent)
            {
                Transforms.Add(Parent);
                Parent = Parent.parent;
            }

            return Transforms.Reverse<Transform>();
        }

        /// <summary>
        /// Executes the callback function for the transform and its descendants by depth first method.
        /// </summary>
        public static void Process<DataType>(this Transform This, TransformProcessCallback<DataType> Callback, DataType Data)
        {
            if (Callback == null) throw new ArgumentNullException(nameof(Callback));

            var ResultData = Callback(This, Data);

            foreach (Transform Child in This)
            {
                Child.Process(Callback, ResultData);
            }
        }

        public static bool IsLastSibling(this Transform This) => This.GetSiblingIndex() == This.parent.childCount - 1;

        /// <summary>
        /// Play mode only!
        /// </summary>
        public static bool IsPrefab(this Transform This)
        {
            if (Application.isEditor && !Application.isPlaying) throw new InvalidOperationException(Strings.MethodIsOnlyAllowedInPlayMode("IsPrefab"));

            return This.gameObject.scene.buildIndex < 0;
        }

        public static void SetPositionX(this Transform This, float X)
        {
            var Position = This.position;
            Position.x = X;
            This.position = Position;
        }

        public static void SetPositionY(this Transform This, float Y)
        {
            var Position = This.position;
            Position.y = Y;
            This.position = Position;
        }

        public static void SetPositionZ(this Transform This, float Z)
        {
            var Position = This.position;
            Position.z = Z;
            This.position = Position;
        }

        public static void SetPositionXY(this Transform This, float X, float Y)
        {
            var Position = This.position;
            Position.x = X;
            Position.y = Y;
            This.position = Position;
        }

        public static void SetPositionXZ(this Transform This, float X, float Z)
        {
            var Position = This.position;
            Position.x = X;
            Position.z = Z;
            This.position = Position;
        }

        public static void SetPositionYZ(this Transform This, float Y, float Z)
        {
            var Position = This.position;
            Position.y = Y;
            Position.z = Z;
            This.position = Position;
        }

        public static void SetLocalPositionX(this Transform This, float X)
        {
            var Position = This.localPosition;
            Position.x = X;
            This.localPosition = Position;
        }

        public static void SetLocalPositionY(this Transform This, float Y)
        {
            var Position = This.localPosition;
            Position.y = Y;
            This.localPosition = Position;
        }

        public static void SetLocalPositionZ(this Transform This, float Z)
        {
            var Position = This.localPosition;
            Position.z = Z;
            This.localPosition = Position;
        }

        public static void SetLocalPositionXY(this Transform This, float X, float Y)
        {
            var Position = This.localPosition;
            Position.x = X;
            Position.y = Y;
            This.localPosition = Position;
        }

        public static void SetLocalPositionXZ(this Transform This, float X, float Z)
        {
            var Position = This.localPosition;
            Position.x = X;
            Position.z = Z;
            This.localPosition = Position;
        }

        public static void SetLocalPositionYZ(this Transform This, float Y, float Z)
        {
            var Position = This.localPosition;
            Position.y = Y;
            Position.z = Z;
            This.localPosition = Position;
        }

        public static void SetEulerX(this Transform This, float X)
        {
            var EulerAngles = This.eulerAngles;
            EulerAngles.x = X;
            This.eulerAngles = EulerAngles;
        }

        public static void SetEulerY(this Transform This, float Y)
        {
            var EulerAngles = This.eulerAngles;
            EulerAngles.y = Y;
            This.eulerAngles = EulerAngles;
        }

        public static void SetEulerZ(this Transform This, float Z)
        {
            var EulerAngles = This.eulerAngles;
            EulerAngles.z = Z;
            This.eulerAngles = EulerAngles;
        }

        public static void SetEulerXY(this Transform This, float X, float Y)
        {
            var EulerAngles = This.eulerAngles;
            EulerAngles.x = X;
            EulerAngles.y = Y;
            This.eulerAngles = EulerAngles;
        }

        public static void SetEulerXZ(this Transform This, float X, float Z)
        {
            var EulerAngles = This.eulerAngles;
            EulerAngles.x = X;
            EulerAngles.z = Z;
            This.eulerAngles = EulerAngles;
        }

        public static void SetEulerYZ(this Transform This, float Y, float Z)
        {
            var EulerAngles = This.eulerAngles;
            EulerAngles.y = Y;
            EulerAngles.z = Z;
            This.eulerAngles = EulerAngles;
        }

        public static void SetLocalEulerX(this Transform This, float X)
        {
            var EulerAngles = This.localEulerAngles;
            EulerAngles.x = X;
            This.localEulerAngles = EulerAngles;
        }

        public static void SetLocalEulerY(this Transform This, float Y)
        {
            var EulerAngles = This.localEulerAngles;
            EulerAngles.y = Y;
            This.localEulerAngles = EulerAngles;
        }

        public static void SetLocalEulerZ(this Transform This, float Z)
        {
            var EulerAngles = This.localEulerAngles;
            EulerAngles.z = Z;
            This.localEulerAngles = EulerAngles;
        }

        public static void SetLocalEulerXY(this Transform This, float X, float Y)
        {
            var EulerAngles = This.localEulerAngles;
            EulerAngles.x = X;
            EulerAngles.y = Y;
            This.localEulerAngles = EulerAngles;
        }

        public static void SetLocalEulerXZ(this Transform This, float X, float Z)
        {
            var EulerAngles = This.localEulerAngles;
            EulerAngles.x = X;
            EulerAngles.z = Z;
            This.localEulerAngles = EulerAngles;
        }

        public static void SetLocalEulerYZ(this Transform This, float Y, float Z)
        {
            var EulerAngles = This.localEulerAngles;
            EulerAngles.y = Y;
            EulerAngles.z = Z;
            This.localEulerAngles = EulerAngles;
        }

        public static void SetLocalScaleX(this Transform This, float X)
        {
            var Scale = This.localScale;
            Scale.x = X;
            This.localScale = Scale;
        }

        public static void SetLocalScaleY(this Transform This, float Y)
        {
            var Scale = This.localScale;
            Scale.y = Y;
            This.localScale = Scale;
        }

        public static void SetLocalScaleZ(this Transform This, float Z)
        {
            var Scale = This.localScale;
            Scale.z = Z;
            This.localScale = Scale;
        }

        public static void SetLocalScaleXY(this Transform This, float X, float Y)
        {
            var Scale = This.localScale;
            Scale.x = X;
            Scale.y = Y;
            This.localScale = Scale;
        }

        public static void SetLocalScaleXZ(this Transform This, float X, float Z)
        {
            var Scale = This.localScale;
            Scale.x = X;
            Scale.z = Z;
            This.localScale = Scale;
        }

        public static void SetLocalScaleYZ(this Transform This, float Y, float Z)
        {
            var Scale = This.localScale;
            Scale.y = Y;
            Scale.z = Z;
            This.localScale = Scale;
        }
    }
}