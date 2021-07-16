using System.Threading.Tasks;

namespace MixyBoos.Api.Services.Helpers.Audio {
    public class AudioFileConverter : IAudioFileConverter {
        public async Task<string> ConvertFileToMp3(string fileName) {
            return await Task.FromResult("Hello, Sailor!");
        }
    }
}
