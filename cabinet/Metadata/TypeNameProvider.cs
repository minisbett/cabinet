using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace cabinet.Metadata;

internal class TypeNameProvider : ISignatureTypeProvider<string, object?>
{
  public string GetArrayType(string elementType, ArrayShape shape) 
    => $"{elementType}[]";

  public string GetByReferenceType(string elementType) 
    => $"{elementType}&";

  public string GetFunctionPointerType(MethodSignature<string> signature)
    => throw new NotImplementedException();

  public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments) 
    => genericType + "<" + string.Join(",", typeArguments) + ">";

  public string GetGenericMethodParameter(object? genericContext, int index)
    => $"T{index}";

  public string GetGenericTypeParameter(object? genericContext, int index)
    => $"T{index}";

  public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired)
    => unmodifiedType;

  public string GetPinnedType(string elementType)
    => elementType;

  public string GetPointerType(string elementType)
    => $"{elementType}*";

  public string GetPrimitiveType(PrimitiveTypeCode typeCode)
    => typeCode.ToString();

  public string GetSZArrayType(string elementType)
    => $"{elementType}[]";

  public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
  {
    TypeDefinition definition = reader.GetTypeDefinition(handle);
    return reader.GetString(definition.Name);
  }

  public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
  {
    TypeReference reference = reader.GetTypeReference(handle);
    return reader.GetString(reference.Namespace) + "." + reader.GetString(reference.Name);
  }

  public string GetTypeFromSpecification(MetadataReader reader, object? genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
    => throw new NotImplementedException();
}
