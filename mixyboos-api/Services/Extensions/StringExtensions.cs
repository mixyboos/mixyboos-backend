using System.Text;

namespace MixyBoos.Api.Services.Extensions;

public static class StringExtensions {
    public static string RemoveAccent(this string txt) {
        var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
        return Encoding.ASCII.GetString(bytes);
    }
}
