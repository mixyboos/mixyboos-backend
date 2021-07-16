using System.Threading.Tasks;

namespace MixyBoos.Api.Services.Helpers.Audio {
    public interface IAudioFileConverter {
        public Task<string> ConvertFileToMp3(string fileName);
    }
}
