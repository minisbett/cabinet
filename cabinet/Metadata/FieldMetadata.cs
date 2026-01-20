using System;
using System.Reflection.Metadata;

namespace cabinet.Metadata;

/// <summary>
/// Represents the metadata of a .NET field.
/// </summary>
internal class FieldMetadata(SignatureTypeMetadata type, string name, object? defaultValue)
{
  /// <summary>
  /// The type of this field.
  /// </summary>
  public SignatureTypeMetadata Type => type;

  /// <summary>
  /// The name of this field.
  /// </summary>
  public string Name => name;

  /// <summary>
  /// The default value of this field. This will be null if this field has no *constant* default value.
  /// </summary>
  public object? DefaultValue => defaultValue;

  /// <summary>
  /// Resolves the specified <see cref="FieldDefinitionHandle"/> into a <see cref="FieldMetadata"/> object.
  /// </summary>
  public static FieldMetadata FromHandle(MetadataReader reader, FieldDefinitionHandle handle)
  {
    FieldDefinition definition = reader.GetFieldDefinition(handle);

    string typeName = definition.DecodeSignature(new TypeNameProvider(), null);
    SignatureTypeMetadata type = SignatureTypeMetadata.FromTypeNameProvider(typeName);

    object? defaultValue = null;
    ConstantHandle defaultValueConstantHandle = definition.GetDefaultValue();
    if (!defaultValueConstantHandle.IsNil)
    {
      Constant defaultValueConstant = reader.GetConstant(definition.GetDefaultValue());
      byte[] bytes = reader.GetBlobBytes(defaultValueConstant.Value);
      defaultValue = defaultValueConstant.TypeCode switch
      {
        ConstantTypeCode.Boolean => BitConverter.ToBoolean(bytes, 0),
        ConstantTypeCode.Char => BitConverter.ToChar(bytes, 0),
        ConstantTypeCode.SByte => (sbyte)bytes[0],
        ConstantTypeCode.Byte => bytes[0],
        ConstantTypeCode.Int16 => BitConverter.ToInt16(bytes, 0),
        ConstantTypeCode.UInt16 => BitConverter.ToUInt16(bytes, 0),
        ConstantTypeCode.Int32 => BitConverter.ToInt32(bytes, 0),
        ConstantTypeCode.UInt32 => BitConverter.ToUInt32(bytes, 0),
        ConstantTypeCode.Int64 => BitConverter.ToInt64(bytes, 0),
        ConstantTypeCode.UInt64 => BitConverter.ToUInt64(bytes, 0),
        ConstantTypeCode.Single => BitConverter.ToSingle(bytes, 0),
        ConstantTypeCode.Double => BitConverter.ToDouble(bytes, 0),
        ConstantTypeCode.String => BitConverter.ToString(bytes, 0),
        _ => null
      };
    }

    return new FieldMetadata(type, reader.GetString(definition.Name), defaultValue);
  }
}