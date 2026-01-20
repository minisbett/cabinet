using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using cabinet.CodeGen;
using cabinet.Metadata;
using Microsoft.Build.Framework;

namespace cabinet;

public class Cabinet : Microsoft.Build.Utilities.Task
{
  /// <summary>
  /// The $(OutDir) MSBuild variable. Represents the output directory of the compilation.
  /// </summary>
  [Required]
  public string OutDir { get; set; } = null!;

  /// <summary>
  /// The $(TargetPath) MSBuild variable. Represents the path to the compiled binary.
  /// </summary>
  [Required]
  public string TargetPath { get; set; } = null!;

  public override bool Execute()
  {
    if (!File.Exists(TargetPath))
    {
      Log.LogError($"The target assembly ('{TargetPath}') could not be found. Please make sure the Cabinet task is executed post-build.");
      return false;
    }

    using FileStream fs = File.OpenRead(TargetPath);
    using PEReader peReader = new(fs);
    MetadataReader reader = peReader.GetMetadataReader();

    TypeMetadata[] types = [.. reader.TypeDefinitions.Select(x => TypeMetadata.FromHandle(reader, x))];
    TypeMetadata[] enums = [.. types.Where(x => x.IsEnum)];
    TypeMetadata[] structs = [.. types.Where(x => x.IsStruct)];
    ExportedMethodMetadata[] methods = [.. types.SelectMany(x => x.ExportedMethods)];

    List<CEnum> cEnums = [];
    List<CStruct> cStructs = [];
    List<CFunction> cFunctions = [];

    string EnsureNullableHelperStruct(string fieldType)
    {
      string structName = $"Cabinet__Nullable_{fieldType}";
      if (!structs.Any(x => x.Name == structName))
        cStructs.Add(new(structName, [new("bool", "hasValue"), new(fieldType, "value")]));

      return structName;
    }

    // -----------------------------
    // -           Enums           -
    // -----------------------------
    foreach (TypeMetadata @enum in enums)
      cEnums.Add(new(@enum.Name, [.. @enum.Fields.Select(x => (x.Name, x.DefaultValue))]));

    // -----------------------------
    // -          Structs          -
    // -----------------------------
    // 1st pass: Ignore the struct if it contains any fields that are of generic type or a generic parameter.
    bool isValid(FieldMetadata field) => !field.Type.IsGenericParameter && !field.Type.IsGeneric;
    TypeMetadata[] validStructs = [.. structs.Where(x => x.Fields.All(isValid))];
    // 2nd pass: Apply the same constraints as in the first pass, but allow fields with a type that is included in the first pass.
    //           This effectively allows fields with generic struct types that are included in the first pass (has no field with generic parameters).
    validStructs = [.. structs.Where(x => x.Fields.All(j => isValid(j) || validStructs.Any(k => k.FullName == j.Type.FullName)))];
    foreach (TypeMetadata type in validStructs)
    {
      List<CField> fields = [];
      foreach (FieldMetadata field in type.Fields)
      {
        string fieldType = MapToCType(field.Type.Name);
        if (field.Type.IsNullable)
          fieldType = EnsureNullableHelperStruct(fieldType);

        fields.Add(new(fieldType, field.Name.Substring(0, 1).ToLower() + field.Name.Substring(1), field.Type.IsPointer));
      }

      cStructs.Add(new(type.Name, [.. fields]));
    }

    // -----------------------------
    // -         Functions         -
    // -----------------------------
    foreach (ExportedMethodMetadata method in methods)
    {
      string returnType = MapToCType(method.ReturnType.Name);
      if (method.ReturnType.IsNullable)
        returnType = EnsureNullableHelperStruct(returnType);

      List<(string, (string, bool))> parameters = [];
      foreach ((string name, SignatureTypeMetadata type) in method.Parameters)
      {
        string typeName = MapToCType(type.Name);
        if (type.IsNullable)
          typeName = EnsureNullableHelperStruct(typeName);

        parameters.Add((name, (typeName, type.IsPointer)));
      }

      cFunctions.Add(new(returnType, method.EntryPoint, method.ReturnType.IsPointer, [.. parameters]));
    }

    CabinetFileWriter.Write(Path.Combine(OutDir, "cabinet.h"), [.. cEnums], [.. cStructs], [.. cFunctions]);

    return true;
  }

  private static string MapToCType(string typeName) => _cTypeMap.TryGetValue(typeName, out string cType) ? cType : typeName;

  /// <summary>
  /// A map for the string representation of C#-types into C-types.
  /// </summary>

  private static readonly Dictionary<string, string> _cTypeMap = new()
  {
    ["Boolean"] = "bool",
    ["Char"] = "int16_t", // char16_t would be C11 standard
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
