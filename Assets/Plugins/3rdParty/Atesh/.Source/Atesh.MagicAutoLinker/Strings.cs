// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;

namespace Atesh.MagicAutoLinker
{
    public static class Strings
    {
        public static readonly string AutoLinkerMustBeFirstInExecutionOrder = $"{nameof(AutoLinker)} must be the first in created message execution order!";

        internal static string MultiDimensionalCollectionsArentSupported(string MemberName) => $"Multi dimensional collections aren't supported by {nameof(AutoLinker)}. Member name is {MemberName}.";
        internal static string AutoLinkSelfIsNotSupportedForCustomClassesAndCollections(string MemberName) => $"AutoLinkSelf attribute isn't supported for custom classes and collections. Member name is {MemberName}.";
        internal static string AwakeAndStartWereSentBeforeAutoLinker(string GameObjectName) => $"Game object '{GameObjectName}' was instantiated as active so Awake and Start messages were sent before {nameof(AutoLinker)} process.\nYou may want to instantiate the object as inactive then activate it in next frame in order for {nameof(AutoLinker)} to run before Awake and Start messages.";
        internal static string ValueTypeMemberCantHaveAutoLink(string Name) => $"Value type member '{Name}' can't have AutoLink attribute.";
        internal static string ValueTypeCollectionCantHaveAutoLink(string Name) => $"Collection with value type elements '{Name}' can't have AutoLink attribute.";
        internal static string CouldntFindComponentOrObjectForMember(string MemberName, string GameObjectName, string Description) => $"Couldn't find a component or object for member '{MemberName}' for game object '{GameObjectName}'. Its value is untouched.{(string.IsNullOrEmpty(Description) ? "" : $"\nDescription: {Description}")}";
        internal static string CouldntFindChildObjectForElement(string CollectionName, int ElementIndex, string GameObjectName, string Description) => $"Couldn't find a child object for element '{CollectionName}[{ElementIndex}]' for game object '{GameObjectName}'. Its value is untouched.{(string.IsNullOrEmpty(Description) ? "" : $"\nDescription: {Description}")}";
        internal static string MemberIsNullAndItCantBeProcessed(string Name) => $"{Name} is null. Its members or elements can't be processed. Make sure it's initialized before Awake (in constructor).";
        internal static string ElementIsNullAndItCantBeProcessed(string CollectionName, int ElementIndex) => $"{CollectionName}[{ElementIndex}] is null. Its members or elements can't be processed. Make sure it's initialized before Awake (in constructor).";
        internal static string ExceptionInPropertyGetter(string PropertyName, Exception E) => $"Exception in property getter '{PropertyName}'. {nameof(AutoLinker)} will still attempt to process the member.\n{E}";
        internal static string ExceptionInPropertySetter(string PropertyName, Exception E) => $"Exception in property setter '{PropertyName}'. {nameof(AutoLinker)} is processing the other members.\n{E}";
        internal static string RedundantAutoLinker(string Name) => $"Redundant {nameof(AutoLinker)} component on game object '{Name}'. No auto link attribute was found in any component.";
        internal static string ListOfCustomClassCantBeEmpty(string MemberName) => $"List of custom class can't be empty. Member name is {MemberName}.";
    }
}