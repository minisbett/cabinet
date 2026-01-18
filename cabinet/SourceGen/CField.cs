namespace cabinet.SourceGen;

internal class CField(string type, string name)
{
  public string Type { get; set; } = type;

  public string Name { get; set; } = name;

  public override string ToString() => $"{Type} {Name};";
}
