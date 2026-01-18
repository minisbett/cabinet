namespace cabinet;

/// <summary>
/// Provides utility extension methods.
/// </summary>
internal static class ExtensionMethods
{
  /// <summary>
  /// Returns the specified string in camelCase.
  /// </summary>
  public static string ToCamelCase(this string str) => str.Substring(0, 1).ToLower() + str.Substring(1);

  /// <summary>
  /// Returns the specified string in PascalCase.
  /// </summary>
  public static string ToPascalCase(this string str) => str.Substring(0, 1).ToUpper() + str.Substring(1);
}
