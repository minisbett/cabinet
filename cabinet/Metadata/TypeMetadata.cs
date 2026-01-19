using System.Linq;
using System.Reflection.Metadata;

namespace cabinet.Metadata;

/// <summary>
/// Represents the metadata of the definition a .NET type.
/// </summary>
internal class TypeMetadata(string @namespace, string name, FieldMetadata[] fields, bool isStruct, bool isGeneric)
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
  public bool IsStruct => isStruct;

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

    // If the base type is System.ValueTuple, this type is a struct.
    bool isStruct = false;
    if(definition.BaseType.Kind is HandleKind.TypeReference)
    {
      TypeReference reference = reader.GetTypeReference((TypeReferenceHandle)definition.BaseType);
      isStruct = reader.GetString(reference.Namespace) is "System" && reader.GetString(reference.Name) is "ValueType";
    }

    return new TypeMetadata(
      reader.GetString(definition.Namespace),
      reader.GetString(definition.Name),
      [.. definition.GetFields().Select(x => FieldMetadata.FromHandle(reader, x))],
      isStruct,
      definition.GetGenericParameters().Count > 0);
  }
}
