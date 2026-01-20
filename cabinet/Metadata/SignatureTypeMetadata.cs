using System.Linq;

namespace cabinet.Metadata;

/// <summary>
/// Represents the metadata of a .NET type in a signature.
/// </summary>
internal class SignatureTypeMetadata(string? @namespace, string name, bool isPointer, bool isNullable, bool isGenericParameter, bool isGeneric)
{
    /// <summary>
    /// The name of this type.
    /// </summary>
    public string Name => name;

    /// <summary>
    /// The full name of this type.
    /// </summary>
    public string FullName => $"{(@namespace is null ? "" : $"{@namespace}.")}{Name}";

    /// <summary>
    /// Bool whether this type is a pointer. If true, the pointer marker (*) will be omitted from <see cref="Name"/>.
    /// </summary>
    public bool IsPointer => isPointer;

    /// <summary>
    /// Bool whether this type is nullable. If true, the nullable shell (System.Nullable) will be omitted from <see cref="Name"/>.
    /// </summary>
    public bool IsNullable => isNullable;

    /// <summary>
    /// Bool whether this type is a generic parameter of the declaring type.
    /// </summary>
    public bool IsGenericParameter => isGenericParameter;

    /// <summary>
    /// Bool whether this type is generic. If true, the generic suffix (eg. `1) will be omitted from <see cref="Name"/>.
    /// </summary>
    public bool IsGeneric => isGeneric;

    /// <summary>
    /// Resolves the specified type name into a <see cref="SignatureTypeMetadata"/> object.
    /// The specified type name is expected to be provided via <see cref="TypeNameProvider"/>.
    /// </summary>
    public static SignatureTypeMetadata FromTypeNameProvider(string typeName)
    {
        bool isPointer = false;
        bool isNullable = false;
        bool isGenericParameter = false;
        bool isGeneric = false;

        // NOTE: It is important to unpack pointers first, followed by nullables.
        //       Otherwise this can cause complications for types such as 'int?*'.
        if (typeName.StartsWith(TypeNameProvider.POINTER_IDENTIFIER))
        {
            typeName = typeName.Substring(TypeNameProvider.POINTER_IDENTIFIER.Length);
            isPointer = true;
        }
        if (typeName.StartsWith("System.Nullable"))
        {
            typeName = typeName.Substring(18).TrimEnd('>'); // omit System.Nullable<...>
            isNullable = true;
        }
        if (typeName.Contains("`"))
        {
            typeName = typeName.Split('`')[0]; // omit `X, along with potential generic parameters that come after
            isGeneric = true;
        }
        if (typeName.StartsWith(TypeNameProvider.GENERIC_PARAMETER_IDENTIFIER))
        {
            typeName = typeName.Substring(TypeNameProvider.GENERIC_PARAMETER_IDENTIFIER.Length);
            isGenericParameter = true;
        }

        string? @namespace = null;
        if (typeName.Contains('.'))
        {
            string[] split = typeName.Split('.');
            @namespace = string.Join(".", split.Take(split.Length - 1));
            typeName = split.Last();
        }

        return new(@namespace, typeName, isPointer, isNullable, isGenericParameter, isGeneric);
    }
}
