using System.IO;
using System.Linq;

namespace cabinet.SourceGen;

internal static class CabinetFileWriter
{
  private const string SHELL =
    """
    #include <stdint.h>

    {0}
    """;

  public static void Write(string filePath, CStruct[] structs)
  {
    string structsStr = string.Join("\n\n", structs.Select(x => x.ToString()));

    string content = string.Format(SHELL, structsStr);

    File.WriteAllText(filePath, content);
  }
}
