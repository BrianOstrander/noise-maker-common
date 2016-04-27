// ReSharper disable UnusedMember.Local
#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;

namespace Atesh.MagicAutoLinker.Examples
{
    // Don't forget to add the AutoLinker component to the same object along with this script.
    [RequireComponent(typeof(AutoLinker))]
    sealed class YourComplexScript : MonoBehaviour
    {
        sealed class GroupedProperties
        {
            [AutoLink]
            public TextMesh UserName;

            [AutoLink(GameObjectName = "AvatarPortrait")]
            public MeshRenderer Avatar;
        }

        public Material AvatarMaterial;

        [AutoLink(GameObjectName = "Players", CollectionElementName = "Player")]
        readonly GroupedProperties[] PlayerCollection = { new GroupedProperties(), new GroupedProperties(), new GroupedProperties() };
        // You must decorate the collection with [AutoLink] attribute.
        // You can override the name of the collection container to search for by GameObjectName property of [AutoLink] attribute.

        [AutoLink(CollectionElementName = "Player")]
        readonly List<GroupedProperties> Players = new List<GroupedProperties> { new GroupedProperties(), new GroupedProperties(), new GroupedProperties() };

        void Start()
        {
            for (var I = 0; I < PlayerCollection.Length; I++)
            {
                PlayerCollection[I].Avatar.material = AvatarMaterial;
                PlayerCollection[I].UserName.text = "Player " + I;
            }

            print("Sub members in the collection were not null so we were able to assign their values in start event...");

            foreach (var Player in Players)
            {
                print(Player.UserName);
            }
        }
    }
}