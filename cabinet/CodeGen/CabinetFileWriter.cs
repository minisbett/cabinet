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
    public static void Write(string filePath, CStruct[] structs, CFunction[] functions)
    {
        List<string> elements = [.. structs.Select(x => x.ToString()), .. functions.Select(x => x.ToString())];

        string content = string.Format(SHELL, string.Join("\n\n", elements));

        File.WriteAllText(filePath, content);
    }
}
