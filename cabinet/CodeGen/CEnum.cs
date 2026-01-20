using System.Linq;
using System.Text.RegularExpressions;

namespace cabinet.CodeGen;

/// <summary>
/// Represents all information required to generate the source for a C-enum.
/// </summary>
internal class CEnum(string name, (string Name, object Value)[] values)
{
  /// <summary>
  /// Returns the source code for this enum.
  /// </summary>
  public override string ToString()
    => $$"""
       typedef enum {{name}}
       {
           {{string.Join(",\n    ", values.Select(x => $"{Regex.Replace(x.Name, @"([a-z0-9])([A-Z])|[\s\-]+", "$1_$2").ToUpper()} = {x.Value}"))}}
       } {{name}};
       """;
}