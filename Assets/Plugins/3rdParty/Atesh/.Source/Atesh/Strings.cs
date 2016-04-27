// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

namespace Atesh
{
    public static class Strings
    {
        public const string ArgumentCantBeNullOrEmpty = "Argument cannot be null or empty.";

        public static string ValueCantBeLessThan(string Min) => $"Value can't be less than {Min}.";
        public static string ValueCantBeGreaterThan(string Max) => $"Value can't be greater than {Max}.";
        public static string ValueMustBeGreaterThan(string Min) => $"Value must be greater than {Min}.";
        public static string ValueMustBeLessThan(string Max) => $"Value must be less than {Max}.";
        public static string ValueMustBeBetween(string Min, string Max) => $"Value must be between {Min} and {Max}.";
        public static string ClassMustProvideValueForField(string ClassName, string FieldName) => $"{ClassName} class must provide a value for {FieldName} field.";
        public static string MethodIsOnlyAllowedInPlayMode(string Name) => $"{Name} method is only allowed in play mode.";
        public static string AlreadyActive(string Name) => $"{Name} is already active!";
        public static string NotActive(string Name) => $"{Name} is not active!";
        public static string CouldntFindResourceData(string Name) => $"Couldn't find {Name} data. Recreating it in Resources folder.";
        public static string ParentIsNotRectTransform(string Name) => $"Parent transform of game object '{Name}' is not a RectTransform.";
        public static string MethodDoesntHaveValidatedParameter(string Name) => $"None of the parameters of method '{Name}' have a validation attribute.";
        public static string TypeMismatchBetweenParameterAndValidation(string ParameterName, string AttributeName) => $"Type mismatch between parameter '{ParameterName}' and attribute '{AttributeName}'.";
    }
}