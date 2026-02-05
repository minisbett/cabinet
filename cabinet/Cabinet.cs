using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using cabinet.CodeGen;
using cabinet.Metadata;

namespace cabinet;

/// <summary>
/// Processes a .NET assembly into the C-header elements.
/// </summary>
internal class Cabinet
{
  private readonly List<CEnum> _enums = [];
  private readonly List<CStruct> _structs = [];
  private readonly List<CFunction> _functions = [];

  /// <summary>
  /// The C-enums in this cabinet.
  /// </summary>
  public IReadOnlyList<CEnum> Enums => _enums.AsReadOnly();

  /// <summary>
  /// The C-structs in this cabinet.
  /// </summary>
  public IReadOnlyList<CStruct> Structs => _structs.AsReadOnly();

  /// <summary>
  /// The C-functions in this cabinet.
  /// </summary>
  public IReadOnlyList<CFunction> Functions => _functions.AsReadOnly();

  /// <summary>
  /// Creates a new cabinet by processing the specified assembly file.
  /// </summary>
  public static Cabinet FromAssemblyFile(string assemblyFile) => new(assemblyFile);

  private Cabinet(string assemblyFile)
  {
    using FileStream fs = File.OpenRead(assemblyFile);
    using PEReader peReader = new(fs);
    MetadataReader reader = peReader.GetMetadataReader();
    TypeMetadata[] types = [.. reader.TypeDefinitions.Select(x => TypeMetadata.FromHandle(reader, x))];

    foreach (TypeMetadata @enum in types.Where(x => x.IsEnum))
      ProcessEnum(@enum);

    // 1st pass: Ignore the struct if it contains any fields that are of generic type or a generic parameter.
    static bool isValid(FieldMetadata field) => !field.Type.IsGenericParameter && !field.Type.IsGeneric;
    TypeMetadata[] validStructs = [.. types.Where(x => x.IsStruct && x.Fields.All(isValid))];
    // 2nd pass: Apply the same constraints as in the first pass, but allow fields with a type that is included in the first pass.
    //           This effectively allows fields with generic struct types that are included in the first pass (has no field with generic parameters).
    validStructs = [.. types.Where(x => x.IsStruct && x.Fields.All(j => isValid(j) || validStructs.Any(k => k.FullName == j.Type.FullName)))];
    foreach (TypeMetadata @struct in validStructs)
      ProcessStruct(@struct);

    foreach (ExportedMethodMetadata method in types.SelectMany(x => x.ExportedMethods))
      ProcessMethod(method);
  }

  /// <summary>
  /// Processes the specified enum type.
  /// </summary>
  private void ProcessEnum(TypeMetadata @enum)
  {
    _enums.Add(new(@enum.Name, [.. @enum.Fields.Select(x => (Regex.Replace(x.Name, @"([a-z0-9])([A-Z])|[\s\-]+", "$1_$2").ToUpper(), x.DefaultValue!))]));
  }

  /// <summary>
  /// Processes the specified struct type.
  /// </summary>
  private void ProcessStruct(TypeMetadata @struct)
  {
    List<CField> fields = [];
    foreach (FieldMetadata field in @struct.Fields)
    {
      string fieldType = MapToCType(field.Type.Name);
      if (field.Type.IsNullable)
        fieldType = EnsureNullableStruct(fieldType);

      fields.Add(new(fieldType, field.Name.Substring(0, 1).ToLower() + field.Name.Substring(1) /* camelCase */, field.Type.IsPointer));
    }

    _structs.Add(new(@struct.Name, [.. fields]));
  }

  /// <summary>
  /// Processes the specified method.
  /// </summary>
  private void ProcessMethod(ExportedMethodMetadata method)
  {
    string returnType = MapToCType(method.ReturnType.Name);
    if (method.ReturnType.IsNullable)
      returnType = EnsureNullableStruct(returnType);

    List<(string, (string, bool))> parameters = [];
    foreach ((string name, SignatureTypeMetadata type) in method.Parameters)
    {
      string typeName = MapToCType(type.Name);
      if (type.IsNullable)
        typeName = EnsureNullableStruct(typeName);

      parameters.Add((name, (typeName, type.IsPointer)));
    }

    _functions.Add(new(returnType, method.EntryPoint, method.ReturnType.IsPointer, [.. parameters]));
  }

  /// <summary>
  /// Creates a nullable helper struct for the specified field type, if it does not already exist, and returns the structs' name.
  /// </summary>
  private string EnsureNullableStruct(string fieldType)
  {
    string structName = $"Cabinet__Nullable_{fieldType}";
    if (!_structs.Any(x => x.Name == structName))
      _structs.Add(new(structName, [new("bool", "hasValue"), new(fieldType, "value")]));

    return structName;
  }

  /// <summary>
  /// Maps the specified C# type name into the C-equivalent. If none exists, the specified type name is returned as-is.
  /// </summary>
  private static string MapToCType(string typeName) => _cTypeMap.TryGetValue(typeName, out string cType) ? cType : typeName;

  /// <summary>
  /// A map for the string representation of C#-types into C-types.
  /// </summary>

  private static readonly Dictionary<string, string> _cTypeMap = new()
  {
    ["Boolean"] = "bool",
    ["Char"] = "uint16_t", // char16_t would be C11 standard
    ["SByte"] = "int8_t",
    ["Byte"] = "uint8_t",
    ["Int16"] = "int16_t",
    ["UInt16"] = "uint16_t",
    ["Int32"] = "int32_t",
    ["UInt32"] = "uint32_t",
    ["Int64"] = "int64_t",
    ["UInt64"] = "uint64_t",
    ["Single"] = "float",
    ["Double"] = "double"
  };
}
