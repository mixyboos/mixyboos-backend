using Microsoft.AspNetCore.Builder;
using MixyBoos.Api.Controllers.Hubs;

namespace MixyBoos.Api.Services.Startup;

public static class RealtimeStartup {
  public static WebApplication UseSignalRHubs(this WebApplication app) {
    app.MapHub<DebugHub>("/hubs/debug");
    app.MapHub<LiveHub>("/hubs/live");
    app.MapHub<ChatHub>("/hubs/chat");
    app.MapHub<UpdatesHub>("/hubs/updates");
    return app;
  }
}
