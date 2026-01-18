using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace cabinet.Metadata;

/// <summary>
/// Provides a string representation of a type in signature metadata.
/// </summary>
internal class TypeNameProvider : ISignatureTypeProvider<string, object?>
{
  /// <summary>
  /// The string representation of generic type parameters, allowing to determine whether a type 
  /// is a generic parameter by comparing the string representation to this constant.
  /// 
  /// This constant should not be an allowed type identifier, as it poses the risk of conflict with actual types.
  /// </summary>
  public const string GENERIC_TYPE_IDENTIFIER = "<generic>";

  public string GetPrimitiveType(PrimitiveTypeCode typeCode) => typeCode.ToString();

  public string GetPointerType(string elementType) => $"{elementType}*";

  public string GetSZArrayType(string elementType) => $"{elementType}[]";

  public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments)
    => genericType + "<" + string.Join(",", typeArguments) + ">";

  public string GetGenericMethodParameter(object? genericContext, int index) => GENERIC_TYPE_IDENTIFIER;

  public string GetGenericTypeParameter(object? genericContext, int index) => GENERIC_TYPE_IDENTIFIER;

  public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
  {
    TypeDefinition definition = reader.GetTypeDefinition(handle);
    return reader.GetString(definition.Name); // The namespace is purposefully omitted here: Foo<Namespace.Bar> -> Foo<Bar>
  }

  public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
  {
    TypeReference definition = reader.GetTypeReference(handle);
    return reader.GetString(definition.Namespace) + "." + reader.GetString(definition.Name);
  }

  public string GetArrayType(string elementType, ArrayShape shape) => throw new NotImplementedException();

  public string GetByReferenceType(string elementType) => throw new NotImplementedException();

  public string GetFunctionPointerType(MethodSignature<string> signature) => throw new NotImplementedException();

  public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired) => unmodifiedType;

  public string GetPinnedType(string elementType) => throw new NotImplementedException();

  public string GetTypeFromSpecification(MetadataReader reader, object? genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
    => throw new NotImplementedException();
}
