using System.Linq;

namespace cabinet.CodeGen;

/// <summary>
/// Represents all information required to generate the source for a C-struct.
/// </summary>
internal class CStruct(string name, CField[] fields)
{
  /// <summary>
  /// The name of the type of this struct.
  /// </summary>
  public string Name => name;

  /// <summary>
  /// Returns the source code for this struct.
  /// </summary>
  public override string ToString()
  => $$"""
     typedef struct {{Name}}
     {
         {{string.Join("\n    ", fields.Select(x => x.ToString()))}}
     } {{Name}};
     """;
}
