using System.Linq;
using System.Reflection.Metadata;

namespace cabinet.Metadata;

/// <summary>
/// Represents the metadata of a .NET field.
/// </summary>
internal class FieldMetadata(string type, string name, bool isNullableType, bool isPointerType)
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
  /// Bool whether the type of this field is a generic parameter of the declaring type.
  /// </summary>
  public bool IsGenericParameterType => type is TypeNameProvider.GENERIC_TYPE_IDENTIFIER;

  /// <summary>
  /// Bool whether the type of this field is generic.
  /// </summary>
  public bool IsGenericType => type.Contains("`"); // Foo`1 -> generic

  /// <summary>
  /// Bool whether this field is nullable. If true, the nullable shell (System.Nullable) will be omitted from <see cref="Type"/>.
  /// </summary>
  public bool IsNullableType => isNullableType;

  /// <summary>
  /// Bool whether this field is a pointer. If true, the pointer marker (*) will be omitted from <see cref="Type"/>.
  /// </summary>
  public bool IsPointerType => isPointerType;

  /// <summary>
  /// Resolves the specified <see cref="FieldDefinitionHandle"/> into a <see cref="FieldMetadata"/> object.
  /// </summary>
  public static FieldMetadata FromHandle(MetadataReader reader, FieldDefinitionHandle handle)
  {
    FieldDefinition definition = reader.GetFieldDefinition(handle);
    string type = definition.DecodeSignature(new TypeNameProvider(), null);

    bool isNullable = false;
    bool isPointer = false;

    // If nullable, we omit the System.Nullable<...>.
    if (type.StartsWith("System.Nullable"))
    {
      type = type.Substring(18).TrimEnd('>');
      isNullable = true;
    }
    // If a pointer, we omit the '*'.
    else if(type.EndsWith("*"))
    {
      type = type.TrimEnd('*');
      isPointer = true;
    }

    return new FieldMetadata(type, reader.GetString(definition.Name), isNullable, isPointer);
  }
}