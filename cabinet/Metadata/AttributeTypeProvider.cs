using System;
using System.Reflection.Metadata;

namespace cabinet.Metadata;

/// <summary>
/// Provides the type of an attribute parameter.
/// </summary>
internal class AttributeTypeProvider : ICustomAttributeTypeProvider<Type>
{
  public Type GetPrimitiveType(PrimitiveTypeCode typeCode)
      => typeCode switch
      {
        PrimitiveTypeCode.Boolean => typeof(bool),
        PrimitiveTypeCode.Byte => typeof(byte),
        PrimitiveTypeCode.SByte => typeof(sbyte),
        PrimitiveTypeCode.Char => typeof(char),
        PrimitiveTypeCode.Int16 => typeof(short),
        PrimitiveTypeCode.UInt16 => typeof(ushort),
        PrimitiveTypeCode.Int32 => typeof(int),
        PrimitiveTypeCode.UInt32 => typeof(uint),
        PrimitiveTypeCode.Int64 => typeof(long),
        PrimitiveTypeCode.UInt64 => typeof(ulong),
        PrimitiveTypeCode.Single => typeof(float),
        PrimitiveTypeCode.Double => typeof(double),
        PrimitiveTypeCode.String => typeof(string),
        PrimitiveTypeCode.IntPtr => typeof(nint),
        PrimitiveTypeCode.UIntPtr => typeof(nuint),
        PrimitiveTypeCode.Object => typeof(object),
        PrimitiveTypeCode.Void => typeof(void),
        _ => throw new ArgumentException("Unknown primitive type.", nameof(typeCode))
      };


  public Type GetSystemType() => typeof(Type);

  public Type GetSZArrayType(Type elementType) => elementType.MakeArrayType();

  public Type GetTypeFromSerializedName(string name) => Type.GetType(name);

  public Type GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) => throw new NotImplementedException();

  public Type GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind) => throw new NotImplementedException();

  public PrimitiveTypeCode GetUnderlyingEnumType(Type type) => throw new NotImplementedException();

  public bool IsSystemType(Type type) => type == typeof(Type);
}
