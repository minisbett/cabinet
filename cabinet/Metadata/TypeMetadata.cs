using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;

namespace cabinet.Metadata;

internal class TypeMetadata(string @namespace, string name, string? baseType, FieldMetadata[] fields, bool isGeneric)
{
  public string Namespace => @namespace;

  public string Name => name;

  public string? BaseType => baseType;

  public FieldMetadata[] Fields => fields;

  public string FullName => $"{Namespace}.{Name}";

  public bool IsStruct => BaseType is "System.ValueType";

  public bool IsGeneric => isGeneric;

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
