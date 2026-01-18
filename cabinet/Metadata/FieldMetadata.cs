using System.Linq;
using System.Reflection.Metadata;

namespace cabinet.Metadata;

internal class FieldMetadata(string type, string name, bool isGenericParameterType, bool isGenericType)
{
  public string Type => type;

  public string Name => name;

  public bool IsGenericParameterType => isGenericParameterType;

  public bool IsGenericType => isGenericType;

  public static FieldMetadata FromHandle(MetadataReader reader, FieldDefinitionHandle handle)
  {
    FieldDefinition definition = reader.GetFieldDefinition(handle);
    TypeDefinition typeDefinition = reader.GetTypeDefinition(definition.GetDeclaringType());
    string type = definition.DecodeSignature(new TypeNameProvider(), null);
    return new FieldMetadata(
      type,
      reader.GetString(definition.Name),
      Enumerable.Range(0, typeDefinition.GetGenericParameters().Count).Select(x => $"T{x}").Contains(type),
      type.Contains("`"));
  }
}