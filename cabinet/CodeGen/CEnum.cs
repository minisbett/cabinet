using System.Linq;

namespace cabinet.CodeGen;

/// <summary>
/// Represents all information required to generate the source for a C-enum.
/// </summary>
internal class CEnum(string name, (string Name, int Value)[] values)
{
  /// <summary>
  /// Returns the source code for this enum.
  /// </summary>
  public override string ToString()
  => $$"""
     typedef enum {{name}}
     {
         {{string.Join(",\n    ", values.Select(x => $"{x.Name} = {x.Value}"))}}
     } {{name}};
     """;
}
