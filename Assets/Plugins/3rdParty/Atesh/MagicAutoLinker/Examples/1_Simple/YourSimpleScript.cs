// ReSharper disable UnusedMember.Local
#pragma warning disable 649

using UnityEngine;

namespace Atesh.MagicAutoLinker.Examples
{
    // Don't forget to add the AutoLinker component to the same object along with this script.
    [RequireComponent(typeof(AutoLinker))]
    sealed class YourSimpleScript : MonoBehaviour
    {
        public Material AvatarMaterial;

        [AutoLink]
        TextMesh UserName;
        // A game object named as “UserName” must be the child of the object
        // which this script is attached to in scene hierarchy.

        [AutoLink(GameObjectName = "AvatarPortrait")]
        MeshRenderer Avatar;
        // You can override the gameobject name with GameObjectName property
        // of AutoLink attribute so the AutoLinker looks for the new name
        // instead of the default field name. 

        void Start()
        {
            UserName.text = "Guest User";
            // You don’t get a null exception here because
            // AutoLinker links the “UserName” gameobject to 
            // the UserName field in this script even before
            // the start event sent by Unity engine in runtime.

            Avatar.material = AvatarMaterial;

            print("UserName and Avatar fields were not null so we were able to assign their values in start event...");
        }
    }
}