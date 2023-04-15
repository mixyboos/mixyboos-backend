using System.Text;

namespace MixyBoos.Api.Services.Extensions;

public static class StringExtensions {
    public static string RemoveAccent(this string txt) {
        var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
        return Encoding.ASCII.GetString(bytes);
    }

    public static string TrimStartString(this string target, string trimString) {
        if (string.IsNullOrEmpty(trimString)) return target;

        string result = target;
        while (result.StartsWith(trimString)) {
            result = result.Substring(trimString.Length);
        }

        return result;
    }
}
