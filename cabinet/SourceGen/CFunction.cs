using System.Linq;

namespace cabinet.SourceGen;

/// <summary>
/// Represents all information required to generate the source for a C-function.
/// </summary>
internal class CFunction(string returnType, string name, bool isPointerReturnType, (string Name, (string Name, bool IsPointer) Type)[] parameters)
{
  /// <summary>
  /// Returns the source code for this function.
  /// </summary>
  public override string ToString()
  {
    string paramsStr = string.Join(", ", parameters.Select(x => $"{x.Type.Name}{(x.Type.IsPointer ? "*" : "")} {x.Name}"));
    return $"const {returnType}{(isPointerReturnType ? "*" : "")} {name}({paramsStr});";
  }
}