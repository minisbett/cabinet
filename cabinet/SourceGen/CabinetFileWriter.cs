using System.IO;
using System.Linq;

namespace cabinet.SourceGen;

/// <summary>
/// Contains logic for writing a cabinet file.
/// </summary>
internal static class CabinetFileWriter
{
  private const string SHELL =
    """
    #include <stdint.h>

    {0}
    """;

  /// <summary>
  /// Generates the source for the specified objects and writes it to the specified file.
  /// </summary>
  public static void Write(string filePath, CStruct[] structs)
  {
    string structsStr = string.Join("\n\n", structs.Select(x => x.ToString()));

    string content = string.Format(SHELL, structsStr);

    File.WriteAllText(filePath, content);
  }
}
