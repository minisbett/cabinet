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
    /// A prefix for the string representation of generic type parameters, allowing to determine whether a type 
    /// is a generic parameter by comparing the string representation to this constant.
    /// 
    /// This constant should not be an allowed type identifier, as it poses the risk of conflict with actual type names.
    /// </summary>
    public const string GENERIC_PARAMETER_IDENTIFIER = "<generic>";

    /// <summary>
    /// A prefix for the string representation of pointers, allowing to determine whether a type 
    /// is a pointer by comparing the string representation to this constant.
    /// 
    /// This constant should not be an allowed type identifier, as it poses the risk of conflict with actual type names.
    /// </summary>
    public const string POINTER_IDENTIFIER = "<pointer>";

    public string GetPrimitiveType(PrimitiveTypeCode typeCode) => typeCode.ToString();

    public string GetPointerType(string elementType) => $"{POINTER_IDENTIFIER}{elementType}";

    public string GetSZArrayType(string elementType) => $"{elementType}[]";

    public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments)
      => genericType + "<" + string.Join(",", typeArguments) + ">";

    public string GetGenericMethodParameter(object? genericContext, int index) => $"{GENERIC_PARAMETER_IDENTIFIER}T{index}";

    public string GetGenericTypeParameter(object? genericContext, int index) => $"{GENERIC_PARAMETER_IDENTIFIER}T{index}";

    public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
    {
        TypeDefinition definition = reader.GetTypeDefinition(handle);
        return $"{reader.GetString(definition.Namespace)}.{reader.GetString(definition.Name)}";
    }

    public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
    {
        TypeReference definition = reader.GetTypeReference(handle);
        return $"{reader.GetString(definition.Namespace)}.{reader.GetString(definition.Name)}";
    }

    public string GetArrayType(string elementType, ArrayShape shape) => throw new NotImplementedException();

    public string GetByReferenceType(string elementType) => throw new NotImplementedException();

    public string GetFunctionPointerType(MethodSignature<string> signature) => throw new NotImplementedException();

    public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired) => unmodifiedType;

    public string GetPinnedType(string elementType) => throw new NotImplementedException();

    public string GetTypeFromSpecification(MetadataReader reader, object? genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
      => throw new NotImplementedException();
}
