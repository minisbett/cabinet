namespace cabinet.SourceGen;

/// <summary>
/// Represents all information required to generate the source for a C-field.
/// </summary>
internal class CField(string type, string name, bool isPointer = false)
{
  /// <summary>
  /// Returns the source code for this field.
  /// </summary>
  public override string ToString() => $"{type}{(isPointer ? "*" : "")} {name};";
}
