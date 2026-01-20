using System.Collections.Generic;
using System.Reflection.Metadata;

namespace cabinet.Metadata;

/// <summary>
/// Represents the metadata of the definition a .NET type.
/// </summary>
internal class TypeMetadata(string @namespace, string name, FieldMetadata[] fields, ExportedMethodMetadata[] exportedMethods, bool isStruct, bool isEnum)
{
  /// <summary>
  /// The name of this type.
  /// </summary>
  public string Name => name;

  /// <summary>
  /// The full name of this type.
  /// </summary>
  public string FullName => $"{@namespace}.{name}";

  /// <summary>
  /// The metadata of the fields declared by this type.
  /// </summary>
  public FieldMetadata[] Fields => fields;

  /// <summary>
  /// The metadata of the UnmanagedCallersOnly methods declared by this type.
  /// </summary>
  public ExportedMethodMetadata[] ExportedMethods => exportedMethods;

  /// <summary>
  /// Bool whether this type is a struct.
  /// </summary>
  public bool IsStruct => isStruct;

  /// <summary>
  /// Bool whether this type if an enum.
  /// </summary>
  public bool IsEnum => isEnum;

  /// <summary>
  /// Resolves the specified <see cref="FieldDefinitionHandle"/> into a <see cref="FieldMetadata"/> object.
  /// </summary>
  public static TypeMetadata FromHandle(MetadataReader reader, TypeDefinitionHandle handle)
  {
    TypeDefinition definition = reader.GetTypeDefinition(handle);

    // Structs always inherit from the base type 'System.ValueType'.
    bool isStruct = false;
    bool isEnum = false;
    if (definition.BaseType.Kind is HandleKind.TypeReference)
    {
      TypeReference reference = reader.GetTypeReference((TypeReferenceHandle)definition.BaseType);
      string fullName = $"{reader.GetString(reference.Namespace)}.{reader.GetString(reference.Name)}";
      isStruct = fullName is "System.ValueType";
      isEnum = fullName is "System.Enum";
    }

    List<FieldMetadata> fields = [];
    foreach (FieldDefinitionHandle fieldHandle in definition.GetFields())
      if (!isEnum || reader.GetString(reader.GetFieldDefinition(fieldHandle).Name) is not "value__") // omit the 'value__' field if this is an enum
        fields.Add(FieldMetadata.FromHandle(reader, fieldHandle));

    List<ExportedMethodMetadata> exportedMethods = [];
    foreach (MethodDefinitionHandle methodHandle in definition.GetMethods())
      if (ExportedMethodMetadata.TryFromHandle(reader, methodHandle, out ExportedMethodMetadata method))
        exportedMethods.Add(method);

    return new TypeMetadata(
      reader.GetString(definition.Namespace),
      reader.GetString(definition.Name).Split('`')[0], // omit the '`' generic suffix (Foo`1 -> Foo)
      [.. fields],
      [.. exportedMethods],
      isStruct,
      isEnum);
  }
}
