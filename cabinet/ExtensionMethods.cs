namespace cabinet;

internal static class ExtensionMethods
{
  public static string ToCamelCase(this string str) => str.Substring(0, 1).ToLower() + str.Substring(1);

  public static string ToPascalCase(this string str) => str.Substring(0, 1).ToUpper() + str.Substring(1);
}
