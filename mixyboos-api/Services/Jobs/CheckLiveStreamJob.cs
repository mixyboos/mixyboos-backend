using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MixyBoos.Api.Controllers.Hubs;
using Quartz;

namespace MixyBoos.Api.Services.Jobs;

public class CheckLiveStreamJob : IJob {
    private readonly IHubContext<LiveHub> _hub;
    private readonly IHttpClientFactory _httpClientFactory;

    public CheckLiveStreamJob(IHubContext<LiveHub> hub, IHttpClientFactory httpClientFactory) {
        _hub = hub;
        _httpClientFactory = httpClientFactory;
    }

    public async Task Execute(IJobExecutionContext context) {
        var userEmail = context.MergedJobDataMap
            .Where(r => r.Key.Equals("UserEmail"))
            .Select(r => r.Value.ToString())
            .FirstOrDefault();
        var showId = context.MergedJobDataMap
            .Where(r => r.Key.Equals("ShowId"))
            .Select(r => r.Value.ToString())
            .FirstOrDefault();
        if (!string.IsNullOrEmpty(userEmail) && !string.IsNullOrEmpty(showId)) {
            using var httpClient = _httpClientFactory.CreateClient("RTMP");
            var response = await httpClient.GetAsync($"/hls/{showId}/index.m3u8");

            if (response.IsSuccessStatusCode) {
                await _hub.Clients.User(userEmail).SendAsync("StreamStarted", showId);
            }
        }
    }
}
