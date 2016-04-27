// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System; 
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Atesh.WeNeedCreatedMessage;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Atesh.MagicAutoLinker
{
    [DisallowMultipleComponent]
    public class AutoLinker : CreatedMessageReceiver
    {
        #region Inspector
        public bool SelfDestroy;
        #endregion

        #region For Editor
#pragma warning disable 649
        // ReSharper disable EventNeverSubscribedTo.Local
        static bool Preview;
        static event Action<Component> RegisterComponent;
        static event Action<MemberInfo> RegisterMember;
        static event Action<Object> RegisterValue;
        static event Action BeginRegisteringSubMembers;
        static event Action EndRegisteringSubMembers;
        static event Action RegisterCollection;
        static event Func<Object, bool> CollectionContains;
        static event Action<string, bool> RegisterError;
        // ReSharper restore EventNeverSubscribedTo.Local
#pragma warning restore 649
        #endregion

        protected override void Created(bool OnSceneLoad)
        {
            if (!OnSceneLoad && gameObject.activeInHierarchy) print(Strings.AwakeAndStartWereSentBeforeAutoLinker(name));

            ProcessAllComponents();

            if (SelfDestroy) Destroy(this);
        }

        public void ProcessAllComponents()
        {
            var AllComponentsIgnored = true;

            foreach (var Component in GetComponents<Component>().Where(Component => !(Component is Transform)))
            {
                if (Preview) RegisterComponent(Component);

                bool ObjectIgnored;
                Process(Component, transform, out ObjectIgnored);

                if (!ObjectIgnored) AllComponentsIgnored = false;
            }

            if (AllComponentsIgnored)
            {
                if (Preview) RegisterMember(null);
                LogError(Strings.RedundantAutoLinker(name), true);
            }
        }

        static void Process(object Object, Transform SearchRoot, out bool ObjectIgnored)
        {
            ObjectIgnored = true;

            foreach (var Member in Object.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                bool MemberIgnored;
                Process(Member, Object, SearchRoot, out MemberIgnored);

                if (!MemberIgnored) ObjectIgnored = false;
            }
        }

        // ReSharper disable once FunctionComplexityOverflow
        static void Process(MemberInfo Member, object Owner, Transform SearchRoot, out bool MemberIgnored)
        {
            MemberIgnored = false;

            var SelfAttribute = (AutoLinkSelfAttribute)Member.GetCustomAttributes(typeof(AutoLinkSelfAttribute), true).FirstOrDefault();
            var IsAutoLinkSelf = SelfAttribute != null;

            var Attribute = IsAutoLinkSelf ? null : (AutoLinkAttribute)Member.GetCustomAttributes(typeof (AutoLinkAttribute), true).FirstOrDefault();

            // Ignore the members without AutoLink attribute silently.
            if (Attribute == null && !IsAutoLinkSelf)
            {
                MemberIgnored = true;
                return;
            }

            if (Preview) RegisterMember(Member);

            PropertyInfo Property;
            FieldInfo Field;
            var ClassAndMemberName = $"{Member.DeclaringType.Name}.{Member.Name}";
            var MemberType = GetMemberType(Member, out Property, out Field);
            var MemberTypeIsGameObject = MemberType == typeof(GameObject);
            var MemberTypeIsComponent = MemberType.IsSubclassOf(typeof(Component));

            if (MemberType.IsValueType)
            {
                LogError(Strings.ValueTypeMemberCantHaveAutoLink(ClassAndMemberName), true);
                return;
            }

            var MemberNameForSearch = IsAutoLinkSelf ? null : Attribute.GameObjectName ?? Member.Name;
            var MemberValue = GetMemberValue(Owner, Property, Field);

            if (MemberTypeIsGameObject || MemberTypeIsComponent)
            {
                // Ignore the members which aren't null.
                if (MemberValue != null && ((Object)MemberValue).GetInstanceID() != 0) return;

                var Value = IsAutoLinkSelf ?
                    // Get the component on self.
                    SearchRoot.GetComponent(MemberType) :
                    // Check if the candidate matches the member type.
                    Search(MemberNameForSearch, SearchRoot).Select(X => MemberTypeIsGameObject ? (Object)X.gameObject : MemberTypeIsComponent ? X.GetComponent(MemberType) : null).FirstOrDefault(X => X);

                if (Value) SetMemberValue(Owner, Property, Field, Value);
                else LogError(Strings.CouldntFindComponentOrObjectForMember(ClassAndMemberName, SearchRoot.name, IsAutoLinkSelf ? SelfAttribute.Description : Attribute.Description));
            }
            else
            {
                // If the member is not a game object or a component, it must a be a custom class or a collection.
                // So it should be processed recursively for its sub members or elements.

                // AutoLinkSelf isn't supported for custom classes and collections.
                if (IsAutoLinkSelf)
                {
                    LogError(Strings.AutoLinkSelfIsNotSupportedForCustomClassesAndCollections(ClassAndMemberName));
                    return;
                }

                // Ignore the members which is null.
                if (MemberValue == null)
                {
                    LogError(Strings.MemberIsNullAndItCantBeProcessed(ClassAndMemberName), true);
                    return;
                }

                var SubRoot = SearchExactName(SearchRoot.name + MemberNameForSearch, SearchRoot).FirstOrDefault();
                if (!SubRoot) SubRoot = SearchExactName(MemberNameForSearch, SearchRoot).FirstOrDefault();

                if (MemberValue is ICollection)
                {
                    var ElementType = MemberType.GetElementType() ?? MemberType.GetGenericArguments().Single();

                    if (ElementType.IsValueType)
                    {
                        LogError(Strings.ValueTypeCollectionCantHaveAutoLink(ClassAndMemberName), true);
                        return;
                    }

                    if (Preview)
                    {
                        RegisterCollection();
                        BeginRegisteringSubMembers();
                    }

                    var AllElementsIgnored = true;
                    var Collection = (ICollection)MemberValue;
                    var ListIsEmpty = Collection is IList && Collection.Count == 0;

                    for (var I = 0; ListIsEmpty || I < Collection.Count; I++)
                    {
                        bool ElementIgnored;
                        bool EmptyListFilled;
                        Process(I, ElementType, Member, MemberNameForSearch, Collection, Attribute, ListIsEmpty, SubRoot ?? SearchRoot, out ElementIgnored, out EmptyListFilled);

                        if (!ElementIgnored) AllElementsIgnored = false;

                        if (EmptyListFilled) break;
                    }

                    if (AllElementsIgnored) MemberIgnored = true;
                }
                else
                {
                    if (Preview) BeginRegisteringSubMembers();

                    bool ObjectIgnored;
                    Process(MemberValue, SubRoot ?? SearchRoot, out ObjectIgnored);
                    MemberIgnored = ObjectIgnored;
                }

                if (Preview) EndRegisteringSubMembers();
            }
        }

        static void Process(int ElementIndex, Type ElementType, MemberInfo OwnerInfo, string OwnerNameForSearch, ICollection Owner, AutoLinkAttribute OwnerAttribute, bool ListIsEmpty, Transform SearchRoot, out bool ElementIgnored, out bool EmptyListFilled)
        {
            ElementIgnored = false;
            EmptyListFilled = false;

            var ClassAndMemberName = $"{OwnerInfo.DeclaringType.Name}.{OwnerInfo.Name}";
            var ElementTypeIsGameObject = ElementType == typeof(GameObject);
            var ElementTypeIsComponent = ElementType.IsSubclassOf(typeof(Component));

            var ElementValue = ListIsEmpty ? null : GetElementValue(ElementIndex, Owner);

            if (ElementTypeIsGameObject || ElementTypeIsComponent)
            {
                // Ignore the elements which aren't null.
                if (ElementValue != null && ((Object)ElementValue).GetInstanceID() != 0) return;

                var SubRoot = SearchExactName(SearchRoot.name + OwnerNameForSearch, SearchRoot).FirstOrDefault();
                if (!SubRoot) SubRoot = SearchExactName(OwnerNameForSearch, SearchRoot).FirstOrDefault();

                // Choose search method according to OwnerAttribute.CollectionElementName
                var Values = string.IsNullOrEmpty(OwnerAttribute.CollectionElementName) ? (SubRoot ?? SearchRoot).Descendants() : Search(OwnerAttribute.CollectionElementName + (ElementIndex + OwnerAttribute.IndexOffsetForCollectionElement).ToString($"D{OwnerAttribute.NumberOfDigitsForCollectionElement}"), SearchRoot);

                // Check if the candidate matches the element type
                var Value = Values.Select(X => ElementTypeIsGameObject ? (Object)X.gameObject : ElementTypeIsComponent ? X.GetComponent(ElementType) : null).FirstOrDefault(X => X && CollectionDoesntContain(Owner, X));

                if (Value) SetElementValue(ElementIndex, Owner, Value, ListIsEmpty);
                else
                {
                    if (ListIsEmpty) EmptyListFilled = true;
                    else
                    {
                        if (Preview) RegisterValue(null);
                        LogError(Strings.CouldntFindChildObjectForElement(ClassAndMemberName, ElementIndex, SearchRoot.name, OwnerAttribute.Description));
                    }
                }
            }
            else
            {
                // If the element is not a game object or a component, it must a be a custom class.
                // So it should be processed recursively for its sub members.

                if (ListIsEmpty)
                {
                    EmptyListFilled = true;

                    if (Preview) RegisterMember(null);
                    LogError(Strings.ListOfCustomClassCantBeEmpty(ClassAndMemberName));
                    return;
                }

                // Ignore the elements which is null.
                if (ElementValue == null)
                {
                    if (Preview) RegisterMember(null);
                    LogError(Strings.ElementIsNullAndItCantBeProcessed(ClassAndMemberName, ElementIndex), true);
                    return;
                }

                if (ElementValue is ICollection)
                {
                    if (Preview) RegisterValue(null);                            
                    LogError(Strings.MultiDimensionalCollectionsArentSupported(ClassAndMemberName), true);
                    return;
                }

                var SubRoot = SearchExactName(SearchRoot.name + OwnerNameForSearch, SearchRoot).FirstOrDefault();
                if (!SubRoot) SubRoot = SearchExactName(OwnerNameForSearch, SearchRoot).FirstOrDefault();

                // Choose search root according to OwnerAttribute.CollectionElementName
                if (!string.IsNullOrEmpty(OwnerAttribute.CollectionElementName)) SubRoot = SearchExactName(OwnerAttribute.CollectionElementName + (ElementIndex + OwnerAttribute.IndexOffsetForCollectionElement).ToString($"D{OwnerAttribute.NumberOfDigitsForCollectionElement}"), SearchRoot).FirstOrDefault();

                if (Preview)
                {
                    RegisterMember(null);
                    BeginRegisteringSubMembers();
                }

                bool ObjectIgnored;
                Process(ElementValue, SubRoot ?? SearchRoot, out ObjectIgnored);
                ElementIgnored = ObjectIgnored;

                if (Preview) EndRegisteringSubMembers();
            }
        }

        static void SetMemberValue(object MemberOwner, PropertyInfo Property, FieldInfo Field, Object Value)
        {
            if (Preview) RegisterValue(Value);
            else
            {
                if (Field != null) Field.SetValue(MemberOwner, Value);
                else
                {
                    try
                    {
                        Property.SetValue(MemberOwner, Value, null);
                    }
                    catch (Exception E)
                    {
                        LogError(Strings.ExceptionInPropertySetter($"{Property.DeclaringType.Name}.{Property.Name}", E));
                    }
                }
            }
        }

        static object GetMemberValue(object MemberOwner, PropertyInfo Property, FieldInfo Field)
        {
            if (Field != null) return Field.GetValue(MemberOwner);

            try
            {
                return Property.GetValue(MemberOwner, null);
            }
            catch (Exception E)
            {
                LogError(Strings.ExceptionInPropertyGetter($"{Property.DeclaringType.Name}.{Property.Name}", E));
                return null;
            }
        }

        static object GetElementValue(int ElementIndex, ICollection ElementOwner) => ElementOwner is Array ? ((Array)ElementOwner).GetValue(ElementIndex) : ((IList)ElementOwner)[ElementIndex];

        static void SetElementValue(int ElementIndex, ICollection ElementOwner, Object Value, bool ListIsEmpty)
        {
            if (ListIsEmpty) ((IList)ElementOwner).Add(null);

            if (Preview) RegisterValue(Value);
            else
            {
                if (ElementOwner is Array) ((Array)ElementOwner).SetValue(Value, ElementIndex);
                else ((IList)ElementOwner)[ElementIndex] = Value;
            }
        }

        static bool CollectionDoesntContain(IEnumerable Collection, Object Item)
        {
            if (Preview) return !CollectionContains(Item);
            return !Collection.Cast<object>().Contains(Item);
        }

        static IEnumerable<Transform> Search(string Name, Transform Root)
        {
            var NameParts = $"{Root.name} {ObjectNames.NicifyVariableName(Name)}".Split(' ');
            return SearchNameCombinations(NameParts, Root, true);
        }

        static IEnumerable<Transform> SearchNameCombinations(IList<string> NameParts, Transform Root, bool RootNameIsIncluded)
        {
            for (var FirstNameIndex = 0; FirstNameIndex < NameParts.Count; FirstNameIndex++)
            {
                // Create name combination to search
                var NameCombination = CombineNameParts(NameParts, FirstNameIndex, NameParts.Count - 1);

                // First, search for the exact name
                foreach (var Result in SearchExactName(NameCombination, Root))
                {
                    yield return Result;
                }

                // Then search without root name (recursive)
                if (RootNameIsIncluded)
                {
                    var RootNameParts = Root.name.Split(' ');
                    var NamePartsWithoutRoot = NameParts.Skip(RootNameParts.Length).ToArray();

                    foreach (var Result in SearchNameCombinations(NamePartsWithoutRoot, Root, false))
                    {
                        yield return Result;
                    }
                }

                // Then search in a nested way (recursive)
                for (var SeparatorIndex = 0; SeparatorIndex < NameParts.Count - 1; SeparatorIndex++)
                {
                    var SubRootName = CombineNameParts(NameParts, 0, SeparatorIndex);
                    var SubNameParts = NameParts.Skip(SeparatorIndex + 1).ToArray();

                    foreach (var Result in SearchExactName(SubRootName, Root).SelectMany(SubRoot => SearchNameCombinations(SubNameParts, SubRoot, false)))
                    {
                        yield return Result;
                    }
                }
            }
        }

        static string CombineNameParts(IList<string> NameParts, int FirstNameIndex, int LastNameIndex)
        {
            var Result = "";
            for (var I = FirstNameIndex; I <= LastNameIndex; I++)
            {
                Result += NameParts[I];
            }

            return Result;
        }

        static IEnumerable<Transform> SearchExactName(string Name, Transform Root) => Root.Descendants().Where(X => X.name.Replace(" ", "") == Name);

        static Type GetMemberType(MemberInfo Member, out PropertyInfo Property, out FieldInfo Field)
        {
            Property = null;
            Field = null;
            Type MemberType = null;

            switch (Member.MemberType)
            {
            case MemberTypes.Field:
                Field = (FieldInfo)Member;
                MemberType = Field.FieldType;
                break;
            case MemberTypes.Property:
                Property = (PropertyInfo)Member;
                MemberType = Property.PropertyType;
                break;
            }

            return MemberType;
        }

        static void LogError(string Text, bool IsWarning = false)
        {
            if (Preview) RegisterError(Text, IsWarning);
            else
            {
                if (IsWarning) Debug.LogWarning(Text);
                else Debug.LogError(Text);
            }
        }
    }
}