using System.Reflection.Metadata;

namespace cabinet.Metadata;

/// <summary>
/// Represents the metadata of a .NET field.
/// </summary>
internal class FieldMetadata(SignatureTypeMetadata type, string name)
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
    /// Resolves the specified <see cref="FieldDefinitionHandle"/> into a <see cref="FieldMetadata"/> object.
    /// </summary>
    public static FieldMetadata FromHandle(MetadataReader reader, FieldDefinitionHandle handle)
    {
        FieldDefinition definition = reader.GetFieldDefinition(handle);

        string typeName = definition.DecodeSignature(new TypeNameProvider(), null);
        SignatureTypeMetadata type = SignatureTypeMetadata.FromTypeNameProvider(typeName);

        return new FieldMetadata(type, reader.GetString(definition.Name));
    }
}