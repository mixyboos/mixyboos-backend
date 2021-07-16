using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Services.Helpers.Audio;

namespace MixyBoos.Api.Services.Workers {
    public class UploadFileProcessor : BackgroundService {
        private readonly IAudioFileConverter _converter;
        private readonly ILogger<UploadFileProcessor> _logger;

        public UploadFileProcessor(IAudioFileConverter converter, ILogger<UploadFileProcessor> logger) {
            _converter = converter;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            _logger.LogDebug($"UploadFileProcessor is starting.");

            stoppingToken.Register(() =>
                _logger.LogDebug($" GracePeriod background task is stopping."));

            while (!stoppingToken.IsCancellationRequested) {
                _logger.LogDebug($"GracePeriod task doing background work.");

                await _converter.ConvertFileToMp3("askjdhas");
            }

            _logger.LogDebug($"GracePeriod background task is stopping.");
        }
    }
}
