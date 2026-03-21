using System.Diagnostics;
using System.IO;
using cabinet.CodeGen;
using Microsoft.Build.Framework;

namespace cabinet;

public class CabinetTask : Microsoft.Build.Utilities.Task
{
  /// <summary>
  /// The $(PublishDir) MSBuild variable. Represents the output directory of the compilation.
  /// </summary>
  [Required]
  public string PublishDir { get; set; } = null!;

  /// <summary>
  /// The $(TargetPath) MSBuild variable. Represents the path to the compiled binary.
  /// </summary>
  [Required]
  public string TargetPath { get; set; } = null!;

  /// <summary>
  /// Represents the header filepath, relative to <see cref="PublishDir"/>.
  /// </summary>
  public string HeaderFile { get; set; } = null!;

  public override bool Execute()
  {
    if (!File.Exists(TargetPath))
    {
      Log.LogError($"The target assembly ('{TargetPath}') could not be found. Please make sure the Cabinet task is executed post-build.");
      return false;
    }

    Cabinet cabinet = Cabinet.FromAssemblyFile(TargetPath);
    CabinetFileWriter.Write(Path.Combine(PublishDir, HeaderFile), cabinet, TargetPath);

    Log.LogMessage("Cabinet header file generated successfully.");

    return true;
  }
}
