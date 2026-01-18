using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using cabinet.Metadata;
using cabinet.SourceGen;
using Microsoft.Build.Framework;

namespace cabinet;

public class Cabinet : Microsoft.Build.Utilities.Task
{
  [Required]
  public string OutDir { get; set; } = null!;

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

    TypeMetadata[] types = [..reader.TypeDefinitions.Select(x => TypeMetadata.FromHandle(reader, x))];

    // -----------------------------
    // -          Structs          -
    // -----------------------------
    List<CStruct> structs = [];
    foreach (TypeMetadata type in types.Where(x => x.IsStruct))
    {
      // Regardless of how we work with thie struct, we will want to strip off the generic suffix.
      string typeName = type.Name.Split('`')[0]; // Foo`1 -> Foo

      // If the struct has fields whose types are either 1. generic type (Foo<T>) or 2. generic parameter (T), ignore this type.
      // If a struct is generic, but has no fields with a generic parameter as type, the struct is safe to pass.
      if (type.Fields.Any(x => x.IsGenericType || x.IsGenericParameterType))
          continue;

      List<CField> fields = [];
      foreach (FieldMetadata field in type.Fields)
        fields.Add(new(_cTypeMap.TryGetValue(field.Type, out string cType) ? cType : field.Type, field.Name));

      structs.Add(new(typeName, [.. fields]));
    }

    CabinetFileWriter.Write(Path.Combine(OutDir, "cabinet.h"), [.. structs]);

    return true;
  }


  private static readonly Dictionary<string, string> _cTypeMap = new()
  {
    ["Boolean"] = "bool",
    ["Char"] = "char16_t",
    ["SByte"] = "int8_t",
    ["Byte"] = "uint8_t",
    ["Int16"] = "int16_t",
    ["UInt16"] = "uint16_t",
    ["Int32"] = "int32_t",
    ["UInt32"] = "uint32_t",
    ["Int64"] = "int64_t",
    ["UInt64"] = "uint64_t",
    ["Single"] = "float",
    ["Double"] = "double",
    ["String"] = "char*"
  };
}
