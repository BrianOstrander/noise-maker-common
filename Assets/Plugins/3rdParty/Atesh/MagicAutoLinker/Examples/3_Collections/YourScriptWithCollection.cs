// ReSharper disable UnusedMember.Local
#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;

namespace Atesh.MagicAutoLinker.Examples
{
    // Don't forget to add the AutoLinker component to the same object along with this script.
    [RequireComponent(typeof(AutoLinker))]
    sealed class YourScriptWithCollection : MonoBehaviour
    {
        // You must decorate the collection with [AutoLink] attribute.
        // Collections must have null items. They can't be empty.

        // ReSharper disable CollectionNeverUpdated.Local
        [AutoLink]
        readonly TextMesh[] LabelsArray = new TextMesh[3];

        [AutoLink(CollectionElementName = "Label")]
        readonly List<TextMesh> LabelsList = new List<TextMesh> { null, null, null };

        [AutoLink(CollectionElementName = "Label")]
        readonly List<TextMesh> EmptyList = new List<TextMesh>();

        [AutoLink]
        readonly List<TextMesh> EmptyList2 = new List<TextMesh>();
        // ReSharper restore CollectionNeverUpdated.Local

        void Start()
        {
            for (var I = 0; I < LabelsList.Count; I++)
            {
                LabelsList[I].text = "Label " + I;
            }

            print("Label items in the collection were not null so we were able to assign their values in start event...");

            foreach (var Label in LabelsArray)
            {
                print(Label);
            }

            foreach (var Label in EmptyList)
            {
                print(Label);
            }

            foreach (var Label in EmptyList2)
            {
                print(Label);
            }
        }
    }
}