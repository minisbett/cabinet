using System.Linq;
using System.Reflection.Metadata;

namespace cabinet.Metadata;

/// <summary>
/// Represents the metadata of a .NET field.
/// </summary>
internal class FieldMetadata(string type, string name, bool isPointerType, bool isNullableType, bool isGenericType, bool isGenericParameterType)
{
  /// <summary>
  /// The name of the type of this field.
  /// </summary>
  public string Type => type;

  /// <summary>
  /// The name of this field.
  /// </summary>
  public string Name => name;

  /// <summary>
  /// Bool whether this field is a pointer. If true, the pointer marker (*) will be omitted from <see cref="Type"/>.
  /// </summary>
  public bool IsPointerType => isPointerType;

  /// <summary>
  /// Bool whether this field is nullable. If true, the nullable shell (System.Nullable) will be omitted from <see cref="Type"/>.
  /// </summary>
  public bool IsNullableType => isNullableType;

  /// <summary>
  /// Bool whether the type of this field is a generic parameter of the declaring type.
  /// </summary>
  public bool IsGenericParameterType => isGenericParameterType;

  /// <summary>
  /// Bool whether the type of this field is generic. If true, the generic suffix (eg. `1) will be omitted from <see cref="Type"/>.
  /// </summary>
  public bool IsGenericType => isGenericType;

  /// <summary>
  /// Resolves the specified <see cref="FieldDefinitionHandle"/> into a <see cref="FieldMetadata"/> object.
  /// </summary>
  public static FieldMetadata FromHandle(MetadataReader reader, FieldDefinitionHandle handle)
  {
    FieldDefinition definition = reader.GetFieldDefinition(handle);
    string type = definition.DecodeSignature(new TypeNameProvider(), null);

    bool isPointer = false;
    bool isNullable = false;
    bool isGenericParameter = false;
    bool isGeneric = false;

    // NOTE: It is important to unpack pointers first, followed by nullables.
    //       Otherwise this can cause complications for types such as 'int?*'.
    if (type.StartsWith(TypeNameProvider.POINTER_IDENTIFIER))
    {
      type = type.Substring(TypeNameProvider.POINTER_IDENTIFIER.Length);
      isPointer = true;
    }
    if (type.StartsWith("System.Nullable"))
    {
      type = type.Substring(18).TrimEnd('>'); // omit System.Nullable<...>
      isNullable = true;
    }
    if (type.Contains("`"))
    {
      type = type.Split('`')[0]; // omit `X
      isGeneric = true;
    }
    if(type.StartsWith(TypeNameProvider.GENERIC_PARAMETER_IDENTIFIER))
    {
      type = type.Substring(TypeNameProvider.GENERIC_PARAMETER_IDENTIFIER.Length);
      isGenericParameter = true;
    }

    return new FieldMetadata(type, reader.GetString(definition.Name), isPointer, isNullable, isGeneric, isGenericParameter);
  }
}