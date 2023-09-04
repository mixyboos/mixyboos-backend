using System;
using System.IO;

namespace MixyBoos.Api.Services.Helpers.IO;

public class PathHelpers {
  public static string GetScopedTempPath() {
    var path = Path.Combine(Path.GetTempPath(), "podnoms/");
    if (!File.Exists(path)) {
      Directory.CreateDirectory(path);
    }

    return path;
  }

  public static string GetScopedTempFile(string extension) =>
    $"{Path.Combine(GetScopedTempPath(), $"{Guid.NewGuid()}.{extension.TrimStart('.')}")}";
}
