using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MixyBoos.Api.Services.Extensions;

public static class CommandExtensions {
  private static string _findCommandPath(string command) {
    // For Windows
    if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
      // Use 'where' command on Windows
      using (var process = new Process()) {
        process.StartInfo.FileName = "where";
        process.StartInfo.Arguments = command;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;

        var output = new StringBuilder();
        process.OutputDataReceived += (sender, args) => {
          if (!string.IsNullOrEmpty(args.Data))
            output.AppendLine(args.Data);
        };

        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();

        return output.ToString().Trim();
      }
    } else {
      // Use 'which' command on Unix/Linux/macOS
      using var process = new Process();
      process.StartInfo.FileName = "which";
      process.StartInfo.Arguments = command;
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.CreateNoWindow = true;

      process.Start();
      string output = process.StandardOutput.ReadToEnd();
      process.WaitForExit();

      return output.Trim();
    }
  }

  public static bool ValidateCommand(this string command, bool throwOnError = false) {
    var commandPath = _findCommandPath(command);
    var result = !string.IsNullOrEmpty(commandPath) && File.Exists(commandPath);

    if (!result && throwOnError) {
      throw new FileNotFoundException($"Command '{command}' not found.");
    }

    return result;
  }
}
