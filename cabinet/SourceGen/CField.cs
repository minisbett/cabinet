namespace cabinet.SourceGen;

/// <summary>
/// Represents all information required to generate the source for a C-field.
/// </summary>
internal class CField(string type, string name)
{
  /// <summary>
  /// The name of the type of this field.
  /// </summary>
  public string Type { get; set; } = type;

  /// <summary>
  /// The name of the name of this field.
  /// </summary>
  public string Name { get; set; } = name;

  /// <summary>
  /// Returns the source code for this field.
  /// </summary>
  public override string ToString() => $"{Type} {Name};";
}
