using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cabinet.CodeGen;

/// <summary>
/// Contains logic for writing a cabinet file.
/// </summary>
internal static class CabinetFileWriter
{
  private const string SHELL =
    """
    #include <stdint.h>
    #include <stdbool.h>

    {0}
    """;

  /// <summary>
  /// Generates the source for the specified objects and writes it to the specified file.
  /// </summary>
  public static void Write(string filePath, CEnum[] enums, CStruct[] structs, CFunction[] functions)
  {
    string enumsStr = string.Join("\n\n", enums.Select(x => x.ToString()));
    string structsStr = string.Join("\n\n", structs.Select(x => x.ToString()));
    string functionsStr = string.Join("\n", functions.Select(x => x.ToString()));

    string content = string.Format(SHELL, string.Join("\n\n", [enumsStr, structsStr, functionsStr]));
    File.WriteAllText(filePath, content);
  }
}
