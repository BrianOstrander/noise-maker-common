// ReSharper disable UnusedMember.Local
#pragma warning disable 649

using System;
using UnityEngine;

namespace Atesh.MagicAutoLinker.Examples
{
    // Don't forget to add the AutoLinker component to the same object along with this script.
    [RequireComponent(typeof(AutoLinker))]
    sealed class YourScriptWithSubclass : MonoBehaviour
    {
        [Serializable]
        public sealed class GroupedProperties
        {
            [AutoLink]
            public TextMesh UserName;

            [AutoLink(GameObjectName = "AvatarPortrait")]
            public MeshRenderer Avatar;
        }

        public Material AvatarMaterial;

        [AutoLink]
        public GroupedProperties Properties;
        // You must decorate the group container with [AutoLink] attribute too

        void Start()
        {
            Properties.UserName.text = "Guest User";
            // You don’t get a null exception here because
            // AutoLinker links the “UserName” gameobject to 
            // the UserName field in this script even before
            // the start event sent by Unity engine in runtime.

            Properties.Avatar.material = AvatarMaterial;

            print("UserName and Avatar fields were not null so we were able to assign their values in start event...");
        }
    }
}