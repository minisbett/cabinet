using System;
using System.Linq;
using System.Reflection.Metadata;

namespace cabinet.Metadata;

/// <summary>
/// Represents a .NET method exported via [UnmanagedCallersOnly].
/// </summary>
internal class ExportedMethodMetadata(string entryPoint, SignatureTypeMetadata returnType, (string, SignatureTypeMetadata)[] parameters)
{
  /// <summary>
  /// The return type of this method.
  /// </summary>
  public SignatureTypeMetadata ReturnType => returnType;

  /// <summary>
  /// The unmanaged entry point of this method.
  /// </summary>
  public string EntryPoint => entryPoint;

  /// <summary>
  /// The parameters of this method.
  /// </summary>
  public (string Name, SignatureTypeMetadata Type)[] Parameters => parameters;

  /// <summary>
  /// Tries to resolve the spcified method definition handle into an exported method. This will return null if the method is not exported.
  /// </summary>
  public static bool TryFromHandle(MetadataReader reader, MethodDefinitionHandle handle, out ExportedMethodMetadata exportedMethod)
  {
    MethodDefinition definition = reader.GetMethodDefinition(handle);
    TypeDefinition type = reader.GetTypeDefinition(definition.GetDeclaringType());

    foreach (CustomAttribute attribute in definition.GetCustomAttributes().Select(reader.GetCustomAttribute)
          .Where(x => x.Constructor.Kind is HandleKind.MemberReference))
    {
      MemberReference ctorReference = reader.GetMemberReference((MemberReferenceHandle)attribute.Constructor);
      TypeReference attributeReference = reader.GetTypeReference((TypeReferenceHandle)ctorReference.Parent);
      string attributeNamespace = reader.GetString(attributeReference.Namespace);
      string attributeName = reader.GetString(attributeReference.Name);

      // Make sure the method has the [UnmanagedCallersOnly] attribute, indicating an exported method.
      if (attributeNamespace is not "System.Runtime.InteropServices" || attributeName is not "UnmanagedCallersOnlyAttribute")
        continue;

      CustomAttributeValue<Type> value = attribute.DecodeValue(new AttributeTypeProvider());

      // If no EntryPoint is specified, default to the name of the method.
      string entryPoint = reader.GetString(definition.Name);
      foreach (CustomAttributeNamedArgument<Type> arg in value.NamedArguments)
        if (arg.Name is "EntryPoint" && arg.Value is string str)
          entryPoint = str;

      MethodSignature<string> signature = definition.DecodeSignature(new TypeNameProvider(), null);
      string[] parameterNames = [.. definition.GetParameters().Select(x => reader.GetString(reader.GetParameter(x).Name))];
      SignatureTypeMetadata[] parameterTypes = [.. signature.ParameterTypes.Select(SignatureTypeMetadata.FromTypeNameProvider)];
      (string, SignatureTypeMetadata)[] parameters = [.. parameterNames.Zip(parameterTypes, (name, type) => (name, type))];
      
      exportedMethod = new(entryPoint, SignatureTypeMetadata.FromTypeNameProvider(signature.ReturnType), parameters);
      return true;
    }

    exportedMethod = null!;
    return false;
  }
}

