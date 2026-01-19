using System.Linq;

namespace cabinet.SourceGen;

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
  /// The fields declared by this struct.
  /// </summary>
  public CField[] Fields => fields;

  /// <summary>
  /// Returns the source code for this struct.
  /// </summary>
  public override string ToString()
  => $$"""
     struct {{Name}}
     {
         {{string.Join("\n    ", Fields.Select(x => x.ToString()))}}
     };
     """;
}
