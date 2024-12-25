using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Controllers.Hubs;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.DTO;
using MixyBoos.Api.Data.Models;
using Quartz;

namespace MixyBoos.Api.Services.Jobs;

public class LiveStreamNotFound(string message) : Exception(message);

public class CheckLiveStreamJob(IHubContext<LiveHub> hub, IHttpClientFactory httpClientFactory, MixyBoosContext context)
  : IJob {
  public async Task Execute(IJobExecutionContext jobContext) {
    var userEmail = jobContext.MergedJobDataMap
      .Where(r => r.Key.Equals("UserEmail"))
      .Select(r => r.Value.ToString())
      .FirstOrDefault();
    var showId = jobContext.MergedJobDataMap
      .Where(r => r.Key.Equals("ShowId"))
      .Select(r => r.Value.ToString())
      .FirstOrDefault();
    var show = await context
      .LiveShows
      .Where(r => r.Id.Equals(Guid.Parse(showId)))
      .FirstOrDefaultAsync();

    if (show is null || string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(showId)) {
      throw new LiveStreamNotFound($"Unable to find show in db context {showId}");
    }

    await hub.Clients.User(userEmail).SendAsync(
      "StreamStarted",
      show.Adapt<LiveShowDTO>());
    using var httpClient = httpClientFactory.CreateClient("RTMP");
    var response = await httpClient.GetAsync($"/hls/{showId}/index.m3u8");

    if (response.IsSuccessStatusCode) {
      show.Status = ShowStatus.InProgress;
      await context.SaveChangesAsync();
      await hub.Clients.User(userEmail).SendAsync(
        "StreamReady",
        show.Adapt<LiveShowDTO>());
    }
  }
}
