using System.Linq;

namespace cabinet.SourceGen;

internal class CStruct(string name, CField[] fields)
{
  private string Name => name;

  private CField[] Fields => fields;

  public override string ToString()
  => $$"""
     struct {{Name}}
     {
         {{string.Join("\n    ", Fields.Select(x => x.ToString()))}}
     };
     """;
}
