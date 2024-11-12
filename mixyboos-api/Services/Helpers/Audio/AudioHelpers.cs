using System;
using System.Threading.Tasks;
using FFMpegCore;

namespace MixyBoos.Api.Services.Helpers.Audio;

public static class AudioHelpers {
  public static async Task<TimeSpan> GetAudioDuration(string file) {
    var reader = await FFProbe.AnalyseAsync(file);
    var duration = reader.Duration;
    return duration;
  }
}
