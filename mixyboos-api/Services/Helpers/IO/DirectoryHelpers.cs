using System.IO;

namespace MixyBoos.Api.Services.Helpers.IO;

public static class DirectoryHelpers {
  public static bool ValidateDirectory(string path, bool create = true) {
    if (string.IsNullOrEmpty(path)) {
      return false;
    }

    if (create && !Directory.Exists(path)) {
      Directory.CreateDirectory(path);
    }

    return Directory.Exists(path);
  }
}
