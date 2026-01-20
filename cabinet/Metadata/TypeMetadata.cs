using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace cabinet.Metadata;

/// <summary>
/// Represents the metadata of the definition a .NET type.
/// </summary>
internal class TypeMetadata(string @namespace, string name, FieldMetadata[] fields, ExportedMethodMetadata[] exportedMethods, bool isStruct)
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
    /// Resolves the specified <see cref="FieldDefinitionHandle"/> into a <see cref="FieldMetadata"/> object.
    /// </summary>
    public static TypeMetadata FromHandle(MetadataReader reader, TypeDefinitionHandle handle)
    {
        TypeDefinition definition = reader.GetTypeDefinition(handle);

        // Structs always inherit from the base type 'System.ValueType'.
        bool isStruct = false;
        if (definition.BaseType.Kind is HandleKind.TypeReference)
        {
            TypeReference reference = reader.GetTypeReference((TypeReferenceHandle)definition.BaseType);
            isStruct = reader.GetString(reference.Namespace) is "System" && reader.GetString(reference.Name) is "ValueType";
        }

        List<ExportedMethodMetadata> exportedMethods = [];
        foreach (MethodDefinitionHandle methodHandle in definition.GetMethods())
            if (ExportedMethodMetadata.TryFromHandle(reader, methodHandle, out ExportedMethodMetadata method))
                exportedMethods.Add(method);

        return new TypeMetadata(
          reader.GetString(definition.Namespace),
          reader.GetString(definition.Name).Split('`')[0], // omit the '`' generic suffix (Foo`1 -> Foo)
          [.. definition.GetFields().Select(x => FieldMetadata.FromHandle(reader, x))],
          [.. exportedMethods],
          isStruct);
    }
}
