using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Controllers.Hubs;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.DTO;
using Quartz;

namespace MixyBoos.Api.Services.Jobs;

public class LiveStreamNotFound : Exception {
    public LiveStreamNotFound(string message) : base(message) { }
}

public class CheckLiveStreamJob : IJob {
    private readonly IHubContext<LiveHub> _hub;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly MixyBoosContext _context;

    public CheckLiveStreamJob(IHubContext<LiveHub> hub, IHttpClientFactory httpClientFactory, MixyBoosContext context) {
        _hub = hub;
        _httpClientFactory = httpClientFactory;
        _context = context;
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
        var show = await _context
            .LiveShows
            .Where(r => r.Id.Equals(Guid.Parse(showId)))
            .FirstOrDefaultAsync();

        if (show is null || string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(showId)) {
            throw new LiveStreamNotFound($"Unable to find show in db context {showId}");
        }

        await _hub.Clients.User(userEmail).SendAsync(
            "StreamStarted",
            show.Adapt<LiveShowDTO>());
        using var httpClient = _httpClientFactory.CreateClient("RTMP");
        var response = await httpClient.GetAsync($"/hls/{showId}/index.m3u8");

        if (response.IsSuccessStatusCode) {
            await _hub.Clients.User(userEmail).SendAsync(
                "StreamReady",
                show.Adapt<LiveShowDTO>());
        }
    }
}
