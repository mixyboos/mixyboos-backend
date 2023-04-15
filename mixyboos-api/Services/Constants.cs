using System.IO;

namespace MixyBoos.Api.Services;

public static class Constants {
    public static string TempFolder {
        get {
            var dir = Path.Combine(Path.GetTempPath(), "mixyboos");
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }
    }
}
