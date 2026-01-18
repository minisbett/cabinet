using System.Linq;
using System.Reflection.Metadata;

namespace cabinet.Metadata;

/// <summary>
/// Represents the metadata of a .NET type.
/// </summary>
internal class TypeMetadata(string @namespace, string name, string? baseType, FieldMetadata[] fields, bool isGeneric)
{
  /// <summary>
  /// The namespace this type is declared in.
  /// </summary>
  public string Namespace => @namespace;

  /// <summary>
  /// The name of this type.
  /// </summary>
  public string Name => name;

  /// <summary>
  /// The name of the base type of this type. This will be null if the type has no base type metadata.
  /// </summary>
  public string? BaseType => baseType;

  /// <summary>
  /// The metadata of the fields declared by this type.
  /// </summary>
  public FieldMetadata[] Fields => fields;

  /// <summary>
  /// The full name of this type, including namespace and name.
  /// </summary>
  public string FullName => $"{Namespace}.{Name}";

  /// <summary>
  /// Bool whether this type is a struct.
  /// </summary>
  public bool IsStruct => BaseType is "System.ValueType";

  /// <summary>
  /// Bool whether this type is generic.
  /// </summary>
  public bool IsGeneric => isGeneric;

  /// <summary>
  /// Resolves the specified <see cref="FieldDefinitionHandle"/> into a <see cref="FieldMetadata"/> object.
  /// </summary>
  public static TypeMetadata FromHandle(MetadataReader reader, TypeDefinitionHandle handle)
  {
    TypeDefinition definition = reader.GetTypeDefinition(handle);
    TypeReference? baseType = definition.BaseType.Kind is HandleKind.TypeReference
                            ? reader.GetTypeReference((TypeReferenceHandle)definition.BaseType)
                            : null;

    return new TypeMetadata(
      reader.GetString(definition.Namespace),
      reader.GetString(definition.Name),
      baseType is null ? null : $"{reader.GetString(baseType.Value.Namespace)}.{reader.GetString(baseType.Value.Name)}",
      [.. definition.GetFields().Select(x => FieldMetadata.FromHandle(reader, x))],
      definition.GetGenericParameters().Count > 0);
  }
}
