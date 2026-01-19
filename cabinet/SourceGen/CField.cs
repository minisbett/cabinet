namespace cabinet.SourceGen;

/// <summary>
/// Represents all information required to generate the source for a C-field.
/// </summary>
internal class CField(string type, string name, bool isPointer = false)
{
  /// <summary>
  /// The name of the type of this field.
  /// </summary>
  public string Type => type;

  /// <summary>
  /// The name of the name of this field.
  /// </summary>
  public string Name => name;

  /// <summary>
  /// Returns the source code for this field.
  /// </summary>
  public override string ToString() => $"{Type}{(isPointer ? "*" : "")} {Name};";
}
